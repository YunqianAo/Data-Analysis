using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class DataSender : MonoBehaviour
{
    private const string BASE = "https://citmalumnes.upc.es/~yunqiana/";
    private const string URL_PLAYER = BASE + "insert_player_data.php";
    private const string URL_SESSION = BASE + "insert_session_data.php";
    private const string URL_SESSION_END = BASE + "insert_end_session_data.php";
    private const string URL_PURCHASE = BASE + "insert_purchase_data.php";

    [HideInInspector] public uint playerId;
    [HideInInspector] public uint sessionId;

    public IEnumerator UploadNewPlayer(string name, string country, int age, float gender, string date)
    {
        WWWForm form = new WWWForm();
        form.AddField("name", name);
        form.AddField("country", country);
        form.AddField("age", age);
        form.AddField("gender", gender.ToString());
        form.AddField("date", date);

        using (UnityWebRequest www = UnityWebRequest.Post(URL_PLAYER, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("上传玩家数据失败 / Player data upload failed: " + www.error);
            }
            else
            {
                try
                {
                    PlayerIdResponse resp = JsonUtility.FromJson<PlayerIdResponse>(www.downloadHandler.text);
                    playerId = resp.playerId;
                    // 重要：通知模拟器“玩家已创建” / Important: notify simulator
                    CallbackEvents.OnAddPlayerCallback?.Invoke(playerId);
                }
                catch { Debug.LogWarning("无法解析玩家ID / Cannot parse player ID: " + www.downloadHandler.text); }
            }
        }
    }

    public IEnumerator UploadSessionStart(uint playerId, string startTime)
    {
        WWWForm form = new WWWForm();
        form.AddField("playerId", playerId.ToString());
        form.AddField("sessionStartTime", startTime);

        using (UnityWebRequest www = UnityWebRequest.Post(URL_SESSION, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("会话开始数据上传失败 / Session start upload failed: " + www.error);
            }
            else
            {
                try
                {
                    SessionIdResponse resp = JsonUtility.FromJson<SessionIdResponse>(www.downloadHandler.text);
                    sessionId = resp.sessionId;
                    // 重要：通知“会话已创建”，以便 NewPlayer 建立映射 / Notify session created to build mapping
                    CallbackEvents.OnNewSessionCallback?.Invoke(sessionId);
                }
                catch { Debug.LogWarning("无法解析会话ID / Cannot parse session ID: " + www.downloadHandler.text); }
            }
        }
    }

    public IEnumerator UploadSessionEnd(uint sessionId, string endTime)
    {
        WWWForm form = new WWWForm();
        form.AddField("sessionId", sessionId.ToString());
        form.AddField("sessionEndTime", endTime);

        using (UnityWebRequest www = UnityWebRequest.Post(URL_SESSION_END, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("会话结束上传失败 / Session end upload failed: " + www.error);
            }
            else
            {
                // 可选：成功后通知（如果你的逻辑需要）/ Optional: notify on success
                CallbackEvents.OnEndSessionCallback?.Invoke(sessionId);
            }
        }
    }

    public IEnumerator UploadPurchase(uint playerId, uint itemId, string purchaseTime)
    {
        WWWForm form = new WWWForm();
        form.AddField("playerId", playerId.ToString());
        form.AddField("itemId", itemId.ToString());
        form.AddField("purchaseDate", purchaseTime);

        using (UnityWebRequest www = UnityWebRequest.Post(URL_PURCHASE, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("购买数据上传失败 / Purchase upload failed: " + www.error);
            }
            else
            {
                // 可选：购买成功的回调 / Optional: purchase event
                CallbackEvents.OnItemBuyCallback?.Invoke(sessionId);
            }
        }
    }

    [Serializable] private class PlayerIdResponse { public uint playerId; }
    [Serializable] private class SessionIdResponse { public uint sessionId; }
}
