using System.Collections;
using System.Globalization;
using UnityEngine;
using UnityEngine.Networking;

public class DataSender : MonoBehaviour
{
    // Enviar datos del jugador a la tabla NewPlayers
    public void SendData(string name, string country, int age, float gender, string date)
    {
        StartCoroutine(UploadData(name, country, age, gender, date));
    }

    private IEnumerator UploadData(string name, string country, int age, float gender, string date)
    {
        WWWForm form = new WWWForm();
        form.AddField("name", name);
        form.AddField("country", country);
        form.AddField("age", age);
        form.AddField("gender", gender.ToString(CultureInfo.InvariantCulture));
        form.AddField("date", date);

        using (UnityWebRequest www = UnityWebRequest.Post("https://citmalumnes.upc.es/~yunqiana/insert_player_data.php", form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error al enviar datos: {www.error}");
            }
            else
            {
                // Obtener el ID de jugador desde la respuesta del servidor
                var jsonResponse = www.downloadHandler.text;
                var playerId = JsonUtility.FromJson<PlayerResponse>(jsonResponse).playerId;

                CallbackEvents.OnAddPlayerCallback?.Invoke(playerId);
                Debug.Log($"Respuesta del servidor: {jsonResponse}");
            }
        }
    }

    // Enviar inicio de sesión a la tabla Sessions
    public void SendSessionStart(uint playerId, string startTime)
    {
        StartCoroutine(UploadSessionStart(playerId, startTime));
    }

    private IEnumerator UploadSessionStart(uint playerId, string startTime)
    {
        WWWForm form = new WWWForm();
        form.AddField("playerId", playerId.ToString());
        form.AddField("sessionStartTime", startTime);

        using (UnityWebRequest www = UnityWebRequest.Post("https://citmalumnes.upc.es/~guillemaa/insert_session_data.php", form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error al enviar inicio de sesión: {www.error}");
            }
            else
            {
                Debug.Log($"Inicio de sesión registrado: {www.downloadHandler.text}");
            }
        }
    }

    // Enviar fin de sesión a la tabla Sessions
    public void SendSessionEnd(uint sessionId, string endTime)
    {
        StartCoroutine(UploadSessionEnd(sessionId, endTime));
    }

    private IEnumerator UploadSessionEnd(uint sessionId, string endTime)
    {
        WWWForm form = new WWWForm();
        form.AddField("sessionId", sessionId.ToString());
        form.AddField("sessionEndTime", endTime);

        using (UnityWebRequest www = UnityWebRequest.Post("https://citmalumnes.upc.es/~guillemaa/insert_end_session_data.php", form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error al enviar fin de sesión: {www.error}");
            }
            else
            {
                Debug.Log($"Fin de sesión registrado: {www.downloadHandler.text}");
            }
        }
    }

    // Enviar compra a la tabla Purchases
    public void SendPurchase(uint playerId, int itemId, string purchaseDate)
    {
        StartCoroutine(UploadPurchase(playerId, itemId, purchaseDate));
    }

    private IEnumerator UploadPurchase(uint playerId, int itemId, string purchaseDate)
    {
        WWWForm form = new WWWForm();
        form.AddField("playerId", playerId.ToString());
        form.AddField("itemId", itemId.ToString());
        form.AddField("purchaseDate", purchaseDate);

        using (UnityWebRequest www = UnityWebRequest.Post("https://citmalumnes.upc.es/~guillemaa/insert_purchase_data.php", form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error al registrar compra: {www.error}");
            }
            else
            {
                Debug.Log($"Compra registrada: {www.downloadHandler.text}");
            }
        }
    }

    // Clase para procesar respuesta JSON del servidor
    [System.Serializable]
    private class PlayerResponse
    {
        public uint playerId;
    }
}
