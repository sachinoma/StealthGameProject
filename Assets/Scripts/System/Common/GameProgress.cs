using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class GameProgress
{
    private static Vector3 DefaultGameStartPos = new Vector3(13.665f, 0.0f, -38.775f);
    private static Vector3 DefaultGameStartRot = new Vector3(0.0f, -18.26f, 0.0f);

    public static Vector3 GameStartPos { get; private set; } = DefaultGameStartPos;
    public static Vector3 GameStartRot { get; private set; } = DefaultGameStartRot;
    public static List<CardType> ObtainedItems { get; private set; } = new List<CardType>();

    private static List<HintType> RemainingHintsLazy = null;
    private static List<HintType> RemainingHints
    {
        get
        {
            if(RemainingHintsLazy == null)
            {
                RemainingHintsLazy = new List<HintType>();
                RemainingHintsLazy.AddRange(Enum.GetValues(typeof(HintType)).Cast<HintType>());
            }
            return RemainingHintsLazy;
        }
    }

    public static bool IsTutorialRoomOpened { get; private set; } = false;

    public static void SaveObtainedItem(CardType obtainedItem, Transform respawnPointRef)
    {
        GameStartPos = respawnPointRef.position;
        GameStartRot = respawnPointRef.eulerAngles;

        if(!ObtainedItems.Contains(obtainedItem))
        {
            ObtainedItems.Add(obtainedItem);
        }
    }

    public static void SaveTutorialDone()
    {
        IsTutorialRoomOpened = true;
    }

    public static void Reset()
    {
        GameStartPos = DefaultGameStartPos;
        GameStartRot = DefaultGameStartRot;
        ObtainedItems.Clear();
        RemainingHintsLazy = null;
        IsTutorialRoomOpened = false;
    }

    #region ヒント

    public static bool CheckIsHintRemain(HintType hintType)
    {
        return RemainingHints.Contains(hintType);
    }

    public static bool SetHintDone(HintType hintType)
    {
        return RemainingHints.Remove(hintType);
    }

    #endregion

}
