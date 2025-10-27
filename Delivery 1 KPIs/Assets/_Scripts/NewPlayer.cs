using System;
using UnityEngine;

public class NewPlayer : MonoBehaviour
{
    private DataSender dataSender;

    private void Start()
    {
        dataSender = FindObjectOfType<DataSender>();
    }

    private void OnEnable()
    {
        Simulator.OnNewPlayer += Subscribe;
        Simulator.OnNewSession += NovaSession;
        Simulator.OnEndSession += FinalSession;
        Simulator.OnBuyItem += Buying;
    }

    private void OnDisable()
    {
        Simulator.OnNewPlayer -= Subscribe;
        Simulator.OnNewSession -= NovaSession;
        Simulator.OnEndSession -= FinalSession;
        Simulator.OnBuyItem -= Buying;
    }

    public void Subscribe(string playerName, string playerCountry, int playerAge, float playerGender, DateTime playerDate)
    {
        Debug.Log($"Jugador agregado: {playerName}, /, {playerCountry}, /, {playerAge}, /, {playerGender}, /, {playerDate}");

        string dateString = playerDate.ToString("yyyy-MM-dd HH:mm:ss");
        dataSender.SendData(playerName, playerCountry, playerAge, playerGender, dateString);
    }

    private void NovaSession(DateTime startDate, uint playerId)
    {
        Debug.Log($"[Unity] Nueva sesión iniciada el {startDate.ToString("yyyy-MM-dd HH:mm:ss")} para el jugador con ID: {playerId}");

        string startTime = startDate.ToString("yyyy-MM-dd HH:mm:ss");
        dataSender.SendSessionStart(playerId, startTime);

        CallbackEvents.OnNewSessionCallback?.Invoke(playerId);
    }

    private void FinalSession(DateTime endDate, uint sessionId)
    {
        Debug.Log($"[Unity] Sesión finalizada el {endDate.ToString("yyyy-MM-dd HH:mm:ss")} para la sesión con ID: {sessionId}");

        string endTime = endDate.ToString("yyyy-MM-dd HH:mm:ss");
        dataSender.SendSessionEnd(sessionId, endTime);

        CallbackEvents.OnEndSessionCallback?.Invoke(sessionId);
    }

    private void Buying(int itemId, DateTime purchaseDate, uint sessionId)
    {
        Debug.Log($"[Unity] Compra realizada del artículo ID: {itemId} el {purchaseDate.ToString("yyyy-MM-dd HH:mm:ss")} en la sesión con ID: {sessionId}");

        string purchaseDateString = purchaseDate.ToString("yyyy-MM-dd HH:mm:ss");
        dataSender.SendPurchase(sessionId, itemId, purchaseDateString);

        CallbackEvents.OnItemBuyCallback?.Invoke(sessionId);
    }
}
