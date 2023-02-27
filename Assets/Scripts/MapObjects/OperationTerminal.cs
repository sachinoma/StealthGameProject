using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OperationTerminal : ReactableBase
{
    [SerializeField] private CardType _cardType;
    [SerializeField] private Transform _operateRefTransform;
    [SerializeField] private AudioSource _audioSource;

    private bool _isActivated = false;

    private void Awake()
    {
        // チュートリアルルームのターミナル
        if(_cardType == GameProgress.GetTutorialCardType())
        {
            _isActivated = GameProgress.IsTutorialRoomOpened;
        }
    }

    public CardType GetCardType()
    {
        return _cardType;
    }

    public Transform GetOperateRefTransform()
    {
        return _operateRefTransform;
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
        return Vector3.Dot(_operateRefTransform.forward, operatorForwardDir) > 0;
    }

    public void Operate()
    {
        _isActivated = true;

        _audioSource.Play();

        TerminalOperatedEventArgs eventArgs = new TerminalOperatedEventArgs(_cardType);
        MainSceneEventManager.TerminalOperated.Invoke(this, eventArgs);
    }

    #region ReactableBase

    public override ReactableType GetReactableType()
    {
        return ReactableType.OperationTerminal;
    }

    #endregion
}
