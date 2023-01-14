using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpItem : ReactableBase
{
    public enum ItemType
    {
        Card_White,
        Card_Red,
        Card_Blue,
        Card_Yellow,
    }

    [SerializeField] private ItemType _itemType;

    public ItemType GetItemType()
    {
        return _itemType;
    }

    #region ReactableBase

    public override ReactableType GetReactableType()
    {
        return ReactableType.PickUpItem;
    }

    #endregion
}
