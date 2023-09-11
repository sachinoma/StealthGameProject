using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MapBoard : ReactableBase
{
    private bool isOpen = false;
    [SerializeField]
    private int posNum;

    public GameObject UI;

    public void MapBoardFunc()
    {
        isOpen = !isOpen;
        if(isOpen)
        {
            UI.SetActive(true);
            UI.GetComponent<MapBoardUI>().SetNowPos(posNum);
            _billboardUIController?.Hide();
            Time.timeScale = 0f;
        }
        else
        {
            UI.SetActive(false);
            _billboardUIController?.Show();
            Time.timeScale = 1f;
        }

    }

    #region ReactableBase

    public override ReactableType GetReactableType()
    {
        return ReactableType.MapBoard;
    }

    #endregion
}
