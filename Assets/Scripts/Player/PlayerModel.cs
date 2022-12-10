using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModel : MonoBehaviour
{
    #region Animation Parameters

    private const string ResetAnimTrigger = "Reset";
    private const string MoveAnimFloat = "Move";
    private const string AttackAnimTrigger = "Attack";
    private const string IsCrouchedAnimBool = "isCrouched";
    private const string JumpAnimTrigger = "Jump";
    private const string IsGroundAnimBool = "isGround";
    private const string PickUpAnimTrigger = "PickUp";
    private const string UselessActionAnimTrigger = "UselessAction";
    private const string TakeDamageAnimTrigger = "TakeDamage";
    private const string DieAnimTrigger = "Die";

    #endregion

    [Header("コンポーネント")]
    [SerializeField] private Animator _animator;
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private ReactableDetector _reactableDetector;
    [SerializeField] private MicRange _micRange;

    [Header("カメラ")]
    [SerializeField] private PlayerCamera _playerCameraTemplate;
    private PlayerCamera _playerCamera;

    [Header("基本")]
    //HP
    [SerializeField] private float _life = 10.0f;
    private float _currentLife;

    [Header("移動")]
    //スピード
    [SerializeField] private float _basicMoveSpeed = 10.0f;
    private float _speed;
    private float _speedAnimatorParameter = 0.0f;
    private bool isMove = false;
    //回転
    [SerializeField] private bool _isRotate = true; //回転できるかどうかの判定
    [SerializeField] private float _rotateSpeed = 15.0f;
    //ジャンプ
    [SerializeField] private float _upForce = 200f; //上方向にかける力
    private bool  _isGround; //着地しているかどうかの判定

    [Header("音声")]
    [SerializeField] private float _maxShoutChargeTime = 1.0f;
    private bool _isShoutCharging;
    private float _startShoutChargingTime;

    private float _inputHorizontal;
    private float _inputVertical;

    private Vector3 _initialPos;
    private Quaternion _initialRot;

    private PlayerState _state = PlayerState.Moving;

    private List<PickUpItem.ItemType> _pickedUpItems = new List<PickUpItem.ItemType>();
    private HidingPlace _currentHidingPlace = null;

    public event Action Died = null;

    void Start()
    {
        ResetStatus();

        _initialPos = transform.position;
        _initialRot = transform.rotation;

        _playerCamera = Instantiate(_playerCameraTemplate);
        _playerCamera.gameObject.name = "PlayerCamera";
        _playerCamera.Init(transform);
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
        _currentLife = _life;
        _speed = _basicMoveSpeed;
        _state = PlayerState.Moving;
        _pickedUpItems.Clear();
        _currentHidingPlace = null;

        ResetShout();
    }

    void Update()
    {

    }

    void FixedUpdate()
    {
        if(_state != PlayerState.Moving)
        {
            _rb.velocity = Vector3.zero;
            return;
        }

        // カメラの方向から、X-Z平面の単位ベクトルを取得
        Vector3 cameraForward = Vector3.Scale(_playerCamera.Cam.transform.forward, new Vector3(1, 0, 1)).normalized;

        // 方向キーの入力値とカメラの向きから、移動方向を決定
        Vector3 moveForward = cameraForward * _inputVertical + _playerCamera.Cam.transform.right * _inputHorizontal;

        // 移動方向にスピードを掛ける。ジャンプや落下がある場合は、別途Y軸方向の速度ベクトルを足す。
        _rb.velocity = moveForward * _speed + new Vector3(0, _rb.velocity.y, 0);

        // キャラクターの向きを進行方向に
        if(_isRotate)
        {
            if(moveForward != Vector3.zero)
            {
                //回転に補間する
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(moveForward), Time.deltaTime * _rotateSpeed);
                //補間しない
                //transform.rotation = Quaternion.LookRotation(moveForward);
            }
        }

        //移動animatorの表現
        if(isMove)
        {
            _animator.SetFloat(MoveAnimFloat, _speedAnimatorParameter);
        }
        else
        {
            _speedAnimatorParameter = Mathf.Max(_speedAnimatorParameter - 0.1f, 0);
            _animator.SetFloat(MoveAnimFloat, _speedAnimatorParameter);
        }
    }

    public void SetMovement(Vector3 direction)
    {
        _inputHorizontal = direction.x;
        _inputVertical = direction.z;

        float magnitude = direction.magnitude;
        isMove = magnitude > 0.0f;
        if(isMove)
        {
            MovementAnimation(magnitude);
        }
    }

    public void SetCrouched(bool isCrouched)
    {
        _animator.SetBool(IsCrouchedAnimBool, isCrouched);
        if(isCrouched) { SetSpeed(0.3f); }
        else { SetSpeed(1.0f); }
    }

    public void SetDash(bool isDash)
    {
        
    }

    public void Attack()
    {
        if(CheckAnimatorState("BasicMovement"))
        {
            _animator.SetTrigger(AttackAnimTrigger);
            SetSpeed(0.0f);
        } 
    }

    public void Jump()
    {
        if(_isGround)
        {
            if(CheckAnimatorState("BasicMovement"))
            {
                _isGround = false;//  isGroundをfalseにする
                _rb.AddForce(new Vector3(0, _upForce, 0)); //上に向かって力を加える
                _animator.SetTrigger(JumpAnimTrigger);
                _animator.SetBool(IsGroundAnimBool, false);
            }
        }
    }


    void OnCollisionEnter(Collision other) //地面に触れた時の処理
    {
        if(other.gameObject.tag == Tag.Ground) //Groundタグのオブジェクトに触れたとき
        {
            _isGround = true; //isGroundをtrueにする
            _animator.SetBool(IsGroundAnimBool, true);
        }
    }

    private void MovementAnimation(float num)
    {
        _speedAnimatorParameter = num;     
    }

    private bool CheckAnimatorState(string name)
    {
        return _animator.GetCurrentAnimatorStateInfo(0).IsName(name);
    }

    private void SetSpeed(float speed)
    {
        _speed = _basicMoveSpeed * speed;
    }
    private void SetRotate(int isRotate)
    {
        if(isRotate == 0)
        {
            _isRotate = true;
        }
        else
        {
            _isRotate = false;
        }
    }

    #region PlayerState

    public void SetMovingState()
    {
        _state = PlayerState.Moving;
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
        _state = PlayerState.Acting;
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
        print($"アイテムを取った：{item.GetItemType()}");
        _pickedUpItems.Add(item.GetItemType());
        Destroy(item.gameObject);

        _animator.SetTrigger(PickUpAnimTrigger);
        _state = PlayerState.Acting;
    }

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
        SkinnedMeshRenderer[] renderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach(SkinnedMeshRenderer renderer in renderers)
        {
            renderer.enabled = false;
        }
        _state = PlayerState.Hiding;
    }

    private void Appear()
    {
        if(_currentHidingPlace == null)
        {
            Debug.LogWarning("_currentHidingPlace == null。直接に PlayerState.Moving に変える。");
            _state = PlayerState.Moving;
            return;
        }

        // TODO : アニメション / 実際のプレイヤー処理
        print("現す");
        SkinnedMeshRenderer[] renderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach(SkinnedMeshRenderer renderer in renderers)
        {
            renderer.enabled = true;
        }
        _state = PlayerState.Moving;
    }

    #endregion

    #region TakeDamage

    public void TakeDamage(float damage)
    {
        if(_currentLife <= 0)
        {
            print("すでに死亡した。");
            return;
        }

        print($"{damage} ダメージを受ける");
        _currentLife = Mathf.Max(0, _currentLife - damage);

        if(_currentLife <= 0)
        {
            Die();
        }
        else
        {
            _animator.SetTrigger(TakeDamageAnimTrigger);
            _state = PlayerState.TakingDamage;
        }
    }

    private void Die()
    {
        print("死亡");
        _animator.SetTrigger(DieAnimTrigger);
        _state = PlayerState.Died;
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
}
