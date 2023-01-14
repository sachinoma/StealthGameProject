using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OperationTerminal : ReactableBase
{
    [SerializeField] private CardType _cardType;
    [SerializeField] private Transform _operateRef;
    private bool _isActivated = false;

    public CardType GetCardType()
    {
        return _cardType;
    }

    public Transform GetOperateRef()
    {
        return _operateRef;
    }

    public bool CheckCanOperate(List<CardType> obtainedItems, Vector3 operatorForwardDir)
    {
        if(_isActivated)
        {
            return false;
        }

        if(obtainedItems == null)
        {
            return false;
        }

        foreach(CardType cardType in obtainedItems)
        {
            if(cardType == _cardType)
            {
                return CheckForwardDirection(operatorForwardDir);
            }
        }

        return false;
    }

    /// <summary>
    /// 向き方向と_operateRefと大きく違うと操作できない
    /// </summary>
    private bool CheckForwardDirection(Vector3 operatorForwardDir)
    {
        // 角度の差 < 90度
        print(Vector3.Dot(_operateRef.forward, operatorForwardDir));
        return Vector3.Dot(_operateRef.forward, operatorForwardDir) > 0;
    }

    public void Operate()
    {
        _isActivated = true;
    }

    #region ReactableBase

    public override ReactableType GetReactableType()
    {
        return ReactableType.OperationTerminal;
    }

    #endregion
}
