using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HintType
{
    Trap,
    OperationTerminal,
    Card,
    Enemy,
}

public static class Hint
{
    public static void TriggerHintIfRemain(object sender, HintType type)
    {
        if(GameProgress.CheckIsHintRemain(type))
        {
            GameProgress.SetHintDone(type);
            HintEventArgs eventArgs = new HintEventArgs(type);
            MainSceneEventManager.TriggerEvent(MainSceneEventManager.HintTriggered.EventId, sender, eventArgs);
        }
    }
}
