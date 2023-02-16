using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapBoardUI : MonoBehaviour
{
    [SerializeField]
    private Transform[] nowPos;

    [SerializeField]
    private Transform nowPosUI;

    public void SetNowPos(int posNum)
    {
        nowPosUI.position = nowPos[posNum-1].position;
        nowPosUI.rotation = nowPos[posNum - 1].rotation;
    }
}
