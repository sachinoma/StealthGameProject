using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainSceneEventManager
{
    #region イベント

    public static MainSceneEvent PlayerBrokeOut     { get; } = new MainSceneEvent("PlayerBrokeOut");
    public static MainSceneEvent PlayerDied         { get; } = new MainSceneEvent("PlayerDied");
    public static MainSceneEvent ItemGot            { get; } = new MainSceneEvent("ItemGot");
    public static MainSceneEvent GameClear          { get; } = new MainSceneEvent("GameClear");
    public static MainSceneEvent TerminalOperated   { get; } = new MainSceneEvent("TerminalOperated");
    public static MainSceneEvent HintTriggered      { get; } = new MainSceneEvent("HintTriggered");

    private static MainSceneEvent[] RegisteredEvents =
    {
        PlayerBrokeOut,
        PlayerDied,
        ItemGot,
        GameClear,
        TerminalOperated,
        HintTriggered,
    };

    #endregion

    private static MainSceneEvent GetEvent(string eventId)
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

    public static void TriggerEvent(string eventId, object sender, EventArgs e = null)
    {
        GetEvent(eventId)?.Invoke(sender, e);
    }

    #endregion
}