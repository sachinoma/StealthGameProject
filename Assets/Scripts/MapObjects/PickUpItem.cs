using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpItem : ReactableBase
{
    public enum ItemType
    {
        Key,
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
