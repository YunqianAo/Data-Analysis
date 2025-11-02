using System;
using System.Collections.Generic;
using UnityEngine;

public class NewPlayer : MonoBehaviour
{
    // 与 DataSender 的引用 / Reference to DataSender
    [SerializeField] private DataSender dataSender;

    // sessionId -> playerId 映射（购买用）/ Map sessionId to playerId (for purchase)
    private readonly Dictionary<uint, uint> sessionToPlayer = new Dictionary<uint, uint>();

    // 暂存“准备开始会话”的玩家ID，等待服务器返回 sessionId 后建立映射
    // Store playerId for pending session start; map when server returns sessionId
    private uint pendingPlayerIdForSessionStart = 0;

    private void Awake()
    {
        if (dataSender == null) dataSender = FindObjectOfType<DataSender>();
        if (dataSender == null)
        {
            Debug.LogError("未找到 DataSender，请在场景中添加 / DataSender not found, please add one in the scene");
        }
    }

    private void OnEnable()
    {
        // 订阅 Simulator 事件 / Subscribe Simulator events
        Simulator.OnNewPlayer += OnNewPlayerGenerated;           // (string,string,int,float,DateTime)
        Simulator.OnNewSession += OnSessionStartRequested;        // (DateTime, uint playerId)
        Simulator.OnEndSession += OnSessionEndRequested;          // (DateTime, uint sessionId)
        Simulator.OnBuyItem += OnPurchaseRequested;            // (int itemId, DateTime, uint sessionId)

        // 订阅 DataSender 的“服务器已创建会话”回调 / Subscribe session-created callback
        if (CallbackEvents.OnNewSessionCallback != OnSessionCreatedFromServer)
            CallbackEvents.OnNewSessionCallback += OnSessionCreatedFromServer;
    }

    private void OnDisable()
    {
        // 取消订阅 / Unsubscribe
        Simulator.OnNewPlayer -= OnNewPlayerGenerated;
        Simulator.OnNewSession -= OnSessionStartRequested;
        Simulator.OnEndSession -= OnSessionEndRequested;
        Simulator.OnBuyItem -= OnPurchaseRequested;

        CallbackEvents.OnNewSessionCallback -= OnSessionCreatedFromServer;
    }

    // =============== 事件实现 / Event handlers ===============

    // 新玩家 → 调 PHP insert_player_data.php
    // New player → call PHP insert_player_data.php
    private void OnNewPlayerGenerated(string playerName, string playerCountry, int playerAge, float playerGender, DateTime playerDate)
    {
        if (dataSender == null) return;

        Debug.Log("添加新玩家 / Adding new player: " + playerName + " / Country: " + playerCountry +
                  " / Age: " + playerAge + " / Gender: " + playerGender + " / Date: " + playerDate);

        string dateString = playerDate.ToString("yyyy-MM-dd HH:mm:ss");
        StartCoroutine(dataSender.UploadNewPlayer(playerName, playerCountry, playerAge, playerGender, dateString));
    }

    // 请求开始会话（签名：DateTime, playerId）
    // Request to start session (signature: DateTime, playerId)
    private void OnSessionStartRequested(DateTime startTime, uint playerId)
    {
        if (dataSender == null) return;

        string startStr = startTime.ToString("yyyy-MM-dd HH:mm:ss");
        Debug.Log("开始新会话 / Starting new session: PlayerId = " + playerId + " / Time = " + startStr);

        // 暂存本次要开会话的玩家ID，等服务器返回 sessionId 后建立映射
        pendingPlayerIdForSessionStart = playerId;

        // PHP: playerId + sessionStartTime
        StartCoroutine(dataSender.UploadSessionStart(playerId, startStr));
    }

    // 服务器返回会话ID时建立映射：sessionId -> playerId
    // Build mapping sessionId -> playerId when server returns sessionId
    private void OnSessionCreatedFromServer(uint sessionId)
    {
        if (pendingPlayerIdForSessionStart != 0)
        {
            sessionToPlayer[sessionId] = pendingPlayerIdForSessionStart;
            Debug.Log("建立映射 / Mapping created: sessionId = " + sessionId +
                      " → playerId = " + pendingPlayerIdForSessionStart);

            dataSender.sessionId = sessionId; // 同步给 DataSender / sync to DataSender
            pendingPlayerIdForSessionStart = 0;
        }
        else
        {
            Debug.LogWarning("收到会话ID但没有待映射的玩家ID / SessionId received but no pending playerId to map.");
        }
    }

    // 请求结束会话（签名：DateTime, sessionId）
    // Request to end session (signature: DateTime, sessionId)
    private void OnSessionEndRequested(DateTime endTime, uint sessionId)
    {
        if (dataSender == null) return;

        string endStr = endTime.ToString("yyyy-MM-dd HH:mm:ss");
        Debug.Log("结束会话 / Ending session: SessionId = " + sessionId + " / Time = " + endStr);

        // PHP: sessionId + sessionEndTime
        StartCoroutine(dataSender.UploadSessionEnd(sessionId, endStr));
    }

    // 购买（签名：int itemId, DateTime time, uint sessionId）
    // Purchase (signature: int itemId, DateTime time, uint sessionId)
    private void OnPurchaseRequested(int itemId, DateTime purchaseTime, uint sessionId)
    {
        if (dataSender == null) return;

        // 购买需要 playerId，先通过 sessionId 映射回 playerId
        // Purchase needs playerId; map from sessionId
        if (!sessionToPlayer.TryGetValue(sessionId, out uint playerId))
        {
            Debug.LogWarning("未找到该会话对应玩家，延后或跳过一次购买 / No player mapped for this sessionId, purchase skipped once. sessionId=" + sessionId);
            return;
        }

        string purchaseStr = purchaseTime.ToString("yyyy-MM-dd HH:mm:ss");
        Debug.Log("购买物品 / Purchase: PlayerId = " + playerId + " / ItemId = " + itemId + " / Time = " + purchaseStr +
                  " (sessionId = " + sessionId + ")");

        // PHP: playerId + itemId + purchaseDate
        StartCoroutine(dataSender.UploadPurchase(playerId, (uint)itemId, purchaseStr));
    }
}
