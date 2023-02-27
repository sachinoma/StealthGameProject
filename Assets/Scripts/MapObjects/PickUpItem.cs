using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpItem : ReactableBase
{
    [SerializeField] private Transform _respawnPointRef;
    [SerializeField] private CardType _cardType;

    public CardType GetCardType()
    {
        return _cardType;
    }

    public void PickUp()
    {
        GameProgress.SaveObtainedItem(_cardType, _respawnPointRef);
        Destroy(gameObject);
    }

    #region ReactableBase

    public override ReactableType GetReactableType()
    {
        return ReactableType.PickUpItem;
    }

    #endregion
}
