using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpItem : ReactableBase
{
    [SerializeField] private CardType _cardType;

    public CardType GetCardType()
    {
        return _cardType;
    }

    #region ReactableBase

    public override ReactableType GetReactableType()
    {
        return ReactableType.PickUpItem;
    }

    #endregion
}
