using System;
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

    private Vector3 _initialPos;
    private Quaternion _initialRot;

    private PlayerState _state = PlayerState.Moving;

    private List<PickUpItem.ItemType> _obtainedItems = new List<PickUpItem.ItemType>();

    private HidingPlace _currentHidingPlace = null;

    public event Action DamageTaken = null;
    public event Action Died = null;

    public event Action<PickUpItem.ItemType> PickedUp = null;

    void Awake()
    {
        ResetStatus();

        _initialPos = transform.position;
        _initialRot = transform.rotation;
    }

    public void Respawn()
    {
        ResetStatus();

        transform.position = _initialPos;
        transform.rotation = _initialRot;

        _playerCamera.ResetRotation();

        _animator.SetTrigger(ResetAnimTrigger);
    }

    private void ResetStatus()
    {
        CurrentLife = _life;
        SetState(PlayerState.Moving);
        _obtainedItems.Clear();
        _currentHidingPlace = null;

        ResetShout();
    }

    void Update()
    {
        _animator.SetFloat(MoveAnimFloat, _movement.GetNormalized01InputSpeed());
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

        ReactableBase reactable = _reactableDetector.PopReactable();
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
                PickUp(reactable);
                break;
            case ReactableType.HidingPlace:
                Hide(reactable);
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

        // TODO : アニメションのタイミングに合わせてアイテムを取る？
        PickUpItem.ItemType itemType = item.GetItemType();
        print($"アイテムを取った：{itemType}");
        _obtainedItems.Add(itemType);
        Destroy(item.gameObject);
        PickedUp?.Invoke(itemType);

        // Prototypeのためのコード
        if(itemType == PickUpItem.ItemType.Key)
        {
            PrototypeMessageEvent.Invoke("鍵を取りました！");
        }

        _animator.SetTrigger(PickUpAnimTrigger);
        SetState(PlayerState.Acting);
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

        // Prototypeのためのコード
        PrototypeMessageEvent.Invoke("隠しました");

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
        // Prototypeのためのコード
        PrototypeMessageEvent.Invoke("現しました");

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
        DamageTaken?.Invoke();

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

    private void Die()
    {
        print("死亡");
        _animator.SetTrigger(DieAnimTrigger);
        SetState(PlayerState.Died);
        Died?.Invoke();
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

    #region AnimationEvent
    private void AnimSoundVolumeSmall()
    {
        _micRange.AnimSetVolumeRate(_audioRangeVolume[0]);
    }

    private void AnimSoundVolumeBig()
    {
        _micRange.AnimSetVolumeRate(_audioRangeVolume[1]);
    }

    

    #endregion
}
