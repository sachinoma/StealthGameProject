using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainSceneEventManager
{
    #region イベント

    public static MainSceneEvent PlayerBrokeOut { get; } = new MainSceneEvent(1);
    public static MainSceneEvent PlayerDied     { get; } = new MainSceneEvent(2);
    public static MainSceneEvent GotKey         { get; } = new MainSceneEvent(3);
    public static MainSceneEvent GotCard        { get; } = new MainSceneEvent(4);

    private static MainSceneEvent[] RegisteredEvents =
    {
        PlayerBrokeOut,
        PlayerDied,
        GotKey,
        GotCard,
    };

    #endregion

    private static MainSceneEvent GetEvent(int eventId)
    {
        foreach(MainSceneEvent registeredEvent in RegisteredEvents)
        {
            if (registeredEvent.EventId == eventId)
            {
                return registeredEvent;
            }
        }

        Debug.LogWarning($"MainSceneEventが探せない。eventId : {eventId}");
        return null;
    }

    #region TriggerEvent

    public static void TriggerEvent(int eventId, object sender, EventArgs e)
    {
        GetEvent(eventId)?.Invoke(sender, e);
    }

    #endregion
}