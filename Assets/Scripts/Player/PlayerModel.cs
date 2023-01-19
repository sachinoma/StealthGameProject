﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModel : MonoBehaviour
{
    #region Animation Parameters

    private const string ResetAnimTrigger = "Reset";
    private const string MoveAnimFloat = "Move";
    private const string PickUpAnimTrigger = "PickUp";
    private const string UselessActionAnimTrigger = "UselessAction";
    private const string TakeDamageAnimTrigger = "TakeDamage";
    private const string DieAnimTrigger = "Die";
    private const string OperateAnimTrigger = "Operate";

    #endregion

    [Header("コンポーネント")]
    [SerializeField] private Animator _animator;
    [SerializeField] private ReactableDetector _reactableDetector;
    [SerializeField] private MicRange _micRange;
    [SerializeField] private PlayerMovement _movement;

    [Header("カメラ")]
    [SerializeField] private PlayerCamera _playerCamera;

    [Header("基本")]
    //HP
    [SerializeField] private float _life = 10.0f;
    public float CurrentLife { get; private set; }

    [Header("音声")]
    [SerializeField] private float[] _audioRangeVolume;
    [SerializeField] private float _maxShoutChargeTime = 1.0f;
    private bool _isShoutCharging;
    private float _startShoutChargingTime;
    [SerializeField] private AudioSource _footstepAudioSource;

    [Header("コライダー")]
    [SerializeField] private GameObject _playerCollider;

    //Trap
    [Header("罠")]
    [SerializeField] private bool _isTrap = false;
    private bool _isTrapIdleWaiting = false;

    [Header("カードキー")]
    [SerializeField] private MeshRenderer _cardMeshForAnim;
    #region Class For _cardMeshForAnim
    [Serializable]
    private class CardMeshMaterial
    {
        public CardType type;
        public Material material;
    }
    #endregion
    [SerializeField] private CardMeshMaterial[] _cardMeshMaterials;
    private CardType _cardTypeToOperate;
    private OperationTerminal _operatingTerminal = null;
    private Transform _operatingRefTransform = null;

    private PlayerState _state = PlayerState.Moving;

    public List<CardType> obtainedItems { get; private set; } = new List<CardType>();

    private HidingPlace _currentHidingPlace = null;

    void Awake()
    {
        Init();
    }

    private void Init()
    {
        transform.position = GameProgress.GameStartPos;
        transform.eulerAngles = GameProgress.GameStartRot;

        CurrentLife = _life;
        SetState(PlayerState.Moving);

        if(GameProgress.ObtainedItems != null)
        {
            obtainedItems.AddRange(GameProgress.ObtainedItems);
        }

        _currentHidingPlace = null;

        ResetShout();
    }

    void Update()
    {
        _animator.SetFloat(MoveAnimFloat, _movement.GetNormalized01InputSpeed());

        TrapIdleCheck();
    }

    public void SetMovement(Vector2 movement)
    {
        _movement.SetTargetMovement(movement);
    }

    #region PlayerState

    private void SetState(PlayerState newState)
    {
        if(_state == newState)
        {
            return;
        }

        if(_state == PlayerState.Moving)
        {
            _movement.SetMovementActive(false);
            _animator.SetFloat(MoveAnimFloat, 0);
        }
        else if(newState == PlayerState.Moving)
        {
            _movement.SetMovementActive(true);
        }


        _state = newState;
    }

    public void SetMovingState()
    {
        SetState(PlayerState.Moving);
    }

    #endregion

    #region Action

    private bool CanDoAction()
    {
        return _state == PlayerState.Moving || _state == PlayerState.Hiding;
    }

    public void DoAction()
    {
        if(!CanDoAction())
        {
            print("今の状態でアクションできない");
            return;
        }

        if(_state == PlayerState.Hiding)
        {
            Appear();
            return;
        }

        ReactableBase reactable = _reactableDetector.GetReactable();
        if(reactable == null)
        {
            DoUselessAction();
            return;
        }

        //print($"反応しているオブジェクト：{reactable.name}");
        ReactableType type = reactable.GetReactableType();

        switch(type)
        {
            case ReactableType.PickUpItem:
                _reactableDetector.PopReactable();
                PickUp(reactable);
                break;
            case ReactableType.HidingPlace:
                _reactableDetector.PopReactable();
                Hide(reactable);
                break;
            case ReactableType.OperationTerminal:
                OperateTerminal(reactable);
                break;
        }
    }

    private void DoUselessAction()
    {
        _animator.SetTrigger(UselessActionAnimTrigger);
        SetState(PlayerState.Acting);
    }

    private void PickUp(ReactableBase reactable)
    {
        if(reactable is not PickUpItem item)
        {
            Debug.LogWarning("PickUpItem ではない reactable が PickUp 関数へ渡された。");
            DoUselessAction();
            return;
        }

        CardType cardType = item.GetCardType();
        print($"アイテムを取った：{cardType}");
        obtainedItems.Add(cardType);

        _animator.SetTrigger(PickUpAnimTrigger);
        SetState(PlayerState.Acting);

        SaveCurrentProgress();

        // TODO : アニメションのタイミングに合わせてアイテムを取る？
        Destroy(item.gameObject);
        ItemGotEventArgs eventArgs = new ItemGotEventArgs(cardType);
        MainSceneEventManager.TriggerEvent(MainSceneEventManager.ItemGot.EventId, this, eventArgs);
    }

    Vector3 _posBeforeHide;    // TODO : 一時的なコード

    private void Hide(ReactableBase reactable)
    {
        if(reactable is not HidingPlace hidingPlace)
        {
            Debug.LogWarning("HidingPlace ではない reactable が Hide 関数へ渡された。");
            DoUselessAction();
            return;
        }

        _currentHidingPlace = hidingPlace;

        // TODO : アニメション / 実際のプレイヤー処理
        print("隠す");

        _posBeforeHide = transform.position;
        hidingPlace.GetComponent<Collider>().isTrigger = true;
        Vector3 hidingPlacePos = hidingPlace.transform.position;
        transform.position = new Vector3(hidingPlacePos.x, 0, hidingPlacePos.z);

        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach(Collider collider in colliders)
        {
            if(collider.CompareTag(Tag.Sounds))
            {
                collider.enabled = false;
            }
        }


        SetState(PlayerState.Hiding);
    }

    private void OperateTerminal(ReactableBase reactable)
    {
        if(reactable is not OperationTerminal terminal)
        {
            Debug.LogWarning("OperationTerminal ではない reactable が OperateTerminal 関数へ渡された。");
            DoUselessAction();
            return;
        }

        if(terminal.CheckCanOperate(obtainedItems, transform.forward))
        {
            _cardTypeToOperate = terminal.GetCardType();
            _operatingRefTransform = terminal.GetOperateRefTransform();
            _operatingTerminal = terminal;

            _animator.SetTrigger(OperateAnimTrigger);
            SetState(PlayerState.Operating);
        }
        else
        {
            Debug.Log("操作できない。");
            DoUselessAction();
            return;
        }
    }

    public Transform GetOperateRefTransform()
    {
        return _operatingRefTransform;
    }

    private void Appear()
    {
        if(_currentHidingPlace == null)
        {
            Debug.LogWarning("_currentHidingPlace == null。直接に PlayerState.Moving に変える。");
            SetState(PlayerState.Moving);
            return;
        }

        // TODO : アニメション / 実際のプレイヤー処理
        print("現す");

        transform.position = _posBeforeHide;
        _currentHidingPlace.GetComponent<Collider>().isTrigger = false;

        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach(Collider collider in colliders)
        {
            if(collider.CompareTag(Tag.Sounds))
            {
                collider.enabled = true;
            }
        }

        SetState(PlayerState.Moving);
    }

    #endregion

    #region TakeDamage
    public void TakeDamage(float damage)
    {
        if(CurrentLife <= 0)
        {
            print("すでに死亡した。");
            return;
        }

        print($"{damage} ダメージを受ける");
        CurrentLife = Mathf.Max(0, CurrentLife - damage);

        if(CurrentLife <= 0)
        {
            Die();
        }
        else
        {
            _animator.SetTrigger(TakeDamageAnimTrigger);
            SetState(PlayerState.TakingDamage);
        }
    }

    public void Die()
    {
        print("死亡");
        _playerCollider.SetActive(false);
        _animator.SetTrigger(DieAnimTrigger);
        SetState(PlayerState.Died);
        MainSceneEventManager.PlayerDied.Invoke(this, null);
    }

    #endregion

    #region Shout

    private void ResetShout()
    {
        _isShoutCharging = false;
        _startShoutChargingTime = float.MaxValue;
    }

    private bool CanShout()
    {
        return _micRange != null && !_micRange.IsMicMode;
    }

    public void StartShoutCharge()
    {
        if(CanShout())
        {
            _isShoutCharging = true;
            _startShoutChargingTime = Time.time;
        }
    }

    public void Shout()
    {
        if(_isShoutCharging && CanShout())
        {
            float volumeRate = Mathf.InverseLerp(0, _maxShoutChargeTime, Time.time - _startShoutChargingTime);
            print($"叫ぶ：volumeRate = {volumeRate}");
            _micRange?.SetVolumeRate(volumeRate);
        }

        ResetShout();
    }

    #endregion

    #region Footstep

    private void PlayFootStepSound()
    {
        float pitch;
        if(_isTrap)
        {
            pitch = 0.5f;
        }
        else
        {
            pitch = UnityEngine.Random.Range(0.7f, 1.3f);
        }

        _footstepAudioSource.pitch = pitch;
        _footstepAudioSource.Play();
    }

    private void SetFootStepVisibleVolume()
    {
        if(!_isTrap)
        {
            _micRange.AnimSetVolumeRate(_audioRangeVolume[0]);
        }
        else
        {
            _micRange.AnimSetVolumeRate(_audioRangeVolume[1]);
        }
    }

    #endregion

    #region AnimationEvent
    public void AnimFootStep()
    {
        PlayFootStepSound();
        SetFootStepVisibleVolume();
    }

    public void AnimCardAppear()
    {
        Material materialToUse = null;
        foreach(CardMeshMaterial cardMeshMaterial in _cardMeshMaterials)
        {
            if(cardMeshMaterial.type == _cardTypeToOperate)
            {
                materialToUse = cardMeshMaterial.material;
                break;
            }
        }
        if (materialToUse == null)
        {
            Debug.LogError($"materialToUseが探せない。_cardTypeToOperate：{_cardTypeToOperate}");
            return;
        }

        _cardMeshForAnim.material = materialToUse;
        _cardMeshForAnim.gameObject.SetActive(true);
    }

    public void AnimCardOperate()
    {
        _operatingTerminal?.Operate();
        _operatingTerminal = null;
    }
    public void AnimCardDisappear()
    {
        _cardMeshForAnim.gameObject.SetActive(false);
    }

    #endregion
    #region Trap

    private void TrapIdleCheck()
    {
        if(_isTrap)
        {
            if(!_isTrapIdleWaiting)
            {
                _isTrapIdleWaiting = true;
                SoundVolumeTrapIdle();
                Invoke("TrapIdleWaitingFalse", 0.3f);
            }
        }
    }

    private void SoundVolumeTrapIdle()
    {
        _micRange.AnimSetVolumeRate(_audioRangeVolume[2]);
    }

    private void TrapIdleWaitingFalse()
    {
        _isTrapIdleWaiting = false;
    }

    public void SetInTrap(bool isInTrap, float speedMultiplier)
    {
        _isTrap = isInTrap;
        _movement.SetSpeedMultiplier(speedMultiplier);
    }

    #endregion

    #region Save GameProgress

    private void SaveCurrentProgress()
    {
        GameProgress.Save(transform, obtainedItems);
    }

    #endregion
}
