using System;
using System.Collections.Generic;
using UnityEngine;

public class NewPlayer : MonoBehaviour
{
    // 方式一：在 Inspector 里手动拖 DataSender
    [SerializeField] private DataSender dataSender;

    // 购买时需要把 sessionId 映射回 playerId（因为 SendPurchase 要用 playerId）
    private readonly Dictionary<uint, uint> sessionToPlayer = new();
    // 记录最近一次“准备开会话”的玩家，用来在 DataSender 返回 sessionId 时建立映射
    private uint pendingPlayerIdForSessionStart = 0;

    private void Awake()
    {
        // 兜底：如果没在 Inspector 拖引用，就自动找一次
        if (dataSender == null)
            dataSender = FindObjectOfType<DataSender>();

        if (dataSender == null)
            Debug.LogError("[NewPlayer] 找不到 DataSender。请在场景里放一个挂有 DataSender.cs 的对象。");
    }

    private void OnEnable()
    {
        Simulator.OnNewPlayer += Subscribe;            // 新玩家
        Simulator.OnNewSession += NovaSession;         // 开会话（传入的是 playerId）
        Simulator.OnEndSession += FinalSession;        // 结束会话（传入的是 sessionId）
        Simulator.OnBuyItem += Buying;                 // 购买（传入的是 sessionId）

        // 关键：等待 DataSender 在“创建会话成功”后回调真正的 sessionId
        CallbackEvents.OnNewSessionCallback += OnSessionCreatedFromServer;
    }

    private void OnDisable()
    {
        Simulator.OnNewPlayer -= Subscribe;
        Simulator.OnNewSession -= NovaSession;
        Simulator.OnEndSession -= FinalSession;
        Simulator.OnBuyItem -= Buying;

        CallbackEvents.OnNewSessionCallback -= OnSessionCreatedFromServer;
    }

    // 收到服务器返回的 sessionId；把它和刚才准备开会话的 playerId 建立映射
    private void OnSessionCreatedFromServer(uint sessionId)
    {
        if (pendingPlayerIdForSessionStart != 0)
        {
            sessionToPlayer[sessionId] = pendingPlayerIdForSessionStart;
            // 清掉 pending，避免串号
            pendingPlayerIdForSessionStart = 0;
            // （保持向 Simulator 的回调链：此回调已由 DataSender 触发，Simulator 会继续流程）
        }
        else
        {
            // 如果这里没有 pending，多半是流程调用顺序乱了也问题不大，只做提示
            Debug.LogWarning($"[NewPlayer] 收到 sessionId={sessionId}，但没有 pending 的 playerId 可映射。");
        }
    }

    // ---------- 事件实现 ----------

    // 新玩家 → 上传到 insert_player_data.php
    public void Subscribe(string playerName, string playerCountry, int playerAge, float playerGender, DateTime playerDate)
    {
        if (dataSender == null) { Debug.LogError("[NewPlayer] dataSender 为空"); return; }

        Debug.Log($"Jugador agregado: {playerName}, /, {playerCountry}, /, {playerAge}, /, {playerGender}, /, {playerDate}");
        string dateString = playerDate.ToString("yyyy-MM-dd HH:mm:ss");
        dataSender.SendData(playerName, playerCountry, playerAge, playerGender, dateString);
    }

    // 开会话（注意：这里传进来的是 playerId，不是 sessionId）
    private void NovaSession(DateTime startDate, uint playerId)
    {
        if (dataSender == null) { Debug.LogError("[NewPlayer] dataSender 为空"); return; }

        string startTime = startDate.ToString("yyyy-MM-dd HH:mm:ss");
        Debug.Log($"[Unity] Nueva sesión iniciada el {startTime} para el jugador con ID: {playerId}");

        // 记录这次是哪个玩家要开会话，等 DataSender 拿到 sessionId 再建立映射
        pendingPlayerIdForSessionStart = playerId;

        // 发起创建会话请求（DataSender 会在成功时解析 sessionId 并触发 OnNewSessionCallback(sessionId)）
        dataSender.SendSessionStart(playerId, startTime);

        // ❌ 别再把 playerId 当作 sessionId 误回调了，删除原来的这行：
        // CallbackEvents.OnNewSessionCallback?.Invoke(playerId);
    }

    // 结束会话（传入的就是 sessionId）
    private void FinalSession(DateTime endDate, uint sessionId)
    {
        if (dataSender == null) { Debug.LogError("[NewPlayer] dataSender 为空"); return; }

        string endTime = endDate.ToString("yyyy-MM-dd HH:mm:ss");
        Debug.Log($"[Unity] Sesión finalizada el {endTime} para la sesión con ID: {sessionId}");
        dataSender.SendSessionEnd(sessionId, endTime);

        CallbackEvents.OnEndSessionCallback?.Invoke(sessionId);
    }

    // 购买（传入的是 sessionId；SendPurchase 需要的是 playerId，因此做一次映射）
    private void Buying(int itemId, DateTime purchaseDate, uint sessionId)
    {
        if (dataSender == null) { Debug.LogError("[NewPlayer] dataSender 为空"); return; }

        string purchaseDateString = purchaseDate.ToString("yyyy-MM-dd HH:mm:ss");

        // 尝试通过 sessionId 找回 playerId
        if (!sessionToPlayer.TryGetValue(sessionId, out uint playerId))
        {
            // 如果映射还没建立，多半是还没等到服务器返回 sessionId 的回调
            Debug.LogWarning($"[NewPlayer] 还没有找到 sessionId={sessionId} 对应的 playerId，购买将延后或放弃一次。");
            return;
        }

        Debug.Log($"[Unity] Compra realizada del artículo ID: {itemId} el {purchaseDateString} en la sesión con ID: {sessionId} (playerId={playerId})");

        // 注意：DataSender.SendPurchase 的第一个参数是 playerId
        dataSender.SendPurchase(playerId, itemId, purchaseDateString);

        CallbackEvents.OnItemBuyCallback?.Invoke(sessionId);
    }
}
