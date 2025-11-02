using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class DataSender : MonoBehaviour
{
    // 服务器基础路径 / Base server path
    private const string BASE = "https://citmalumnes.upc.es/~yunqiana/";
    private const string URL_PLAYER = BASE + "insert_player_data.php";
    private const string URL_SESSION = BASE + "insert_session_data.php";
    private const string URL_SESSION_END = BASE + "insert_end_session_data.php";
    private const string URL_PURCHASE = BASE + "insert_purchase_data.php";

    // 玩家和会话 ID / Player and session IDs
    [HideInInspector] public uint playerId;
    [HideInInspector] public uint sessionId;

    // 上传玩家数据 / Upload player data
    public IEnumerator UploadNewPlayer(string name, string country, int age, float gender, string date)
    {
        WWWForm form = new WWWForm();
        form.AddField("name", name);
        form.AddField("country", country);
        form.AddField("age", age);
        form.AddField("gender", gender.ToString());
        form.AddField("date", date);

        Debug.Log("上传玩家数据 / Sending player data to " + URL_PLAYER);

        using (UnityWebRequest www = UnityWebRequest.Post(URL_PLAYER, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("上传玩家数据失败 / Player data upload failed: " + www.error);
            }
            else
            {
                Debug.Log("上传成功 / Player upload success: " + www.downloadHandler.text);
                try
                {
                    PlayerIdResponse response = JsonUtility.FromJson<PlayerIdResponse>(www.downloadHandler.text);
                    playerId = response.playerId;
                }
                catch
                {
                    Debug.LogWarning("无法解析玩家ID / Cannot parse player ID");
                }
            }
        }
    }

    // 上传会话开始数据 / Upload session start data
    public IEnumerator UploadSessionStart(uint playerId, string startTime)
    {
        WWWForm form = new WWWForm();
        form.AddField("playerId", playerId.ToString());
        form.AddField("sessionStartTime", startTime);

        Debug.Log("开始会话上传 / Sending session start data to " + URL_SESSION);

        using (UnityWebRequest www = UnityWebRequest.Post(URL_SESSION, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("会话开始数据上传失败 / Session start upload failed: " + www.error);
            }
            else
            {
                Debug.Log("会话开始上传成功 / Session start upload success: " + www.downloadHandler.text);
                try
                {
                    SessionIdResponse response = JsonUtility.FromJson<SessionIdResponse>(www.downloadHandler.text);
                    sessionId = response.sessionId;
                }
                catch
                {
                    Debug.LogWarning("无法解析会话ID / Cannot parse session ID");
                }
            }
        }
    }

    // 上传会话结束数据 / Upload session end data
    public IEnumerator UploadSessionEnd(uint sessionId, string endTime)
    {
        WWWForm form = new WWWForm();
        form.AddField("sessionId", sessionId.ToString());
        form.AddField("sessionEndTime", endTime);

        Debug.Log("结束会话上传 / Sending session end data to " + URL_SESSION_END);

        using (UnityWebRequest www = UnityWebRequest.Post(URL_SESSION_END, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("会话结束上传失败 / Session end upload failed: " + www.error);
            }
            else
            {
                Debug.Log("会话结束上传成功 / Session end upload success: " + www.downloadHandler.text);
            }
        }
    }

    // 上传购买数据 / Upload purchase data
    public IEnumerator UploadPurchase(uint playerId, uint itemId, string purchaseTime)
    {
        WWWForm form = new WWWForm();
        form.AddField("playerId", playerId.ToString());
        form.AddField("itemId", itemId.ToString());
        form.AddField("purchaseDate", purchaseTime);

        Debug.Log("上传购买数据 / Sending purchase data to " + URL_PURCHASE);

        using (UnityWebRequest www = UnityWebRequest.Post(URL_PURCHASE, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("购买数据上传失败 / Purchase upload failed: " + www.error);
            }
            else
            {
                Debug.Log("购买数据上传成功 / Purchase upload success: " + www.downloadHandler.text);
            }
        }
    }

    // JSON 数据解析结构 / JSON response data structure
    [Serializable]
    private class PlayerIdResponse
    {
        public uint playerId;
    }

    [Serializable]
    private class SessionIdResponse
    {
        public uint sessionId;
    }
}
