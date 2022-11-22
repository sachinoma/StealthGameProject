using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModel : MonoBehaviour
{
    [Header("コンポーネント")]
    [SerializeField] private Animator _animator;
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private ReactableDetector _reactableDetector;

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
    //ジャンプ
    [SerializeField] private float _upForce = 200f; //上方向にかける力
    private bool  _isGround; //着地しているかどうかの判定

    private float _inputHorizontal;
    private float _inputVertical;

    // TODO : Enum で Mode/State を表す
    private bool _isNormalMode = true;

    public static event Action Died = null;

    void Start()
    {
        _currentLife = _life;
        _speed = _basicMoveSpeed;
    }

    void Update()
    {

    }

    void FixedUpdate()
    {
        // カメラの方向から、X-Z平面の単位ベクトルを取得
        Vector3 cameraForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;

        // 方向キーの入力値とカメラの向きから、移動方向を決定
        Vector3 moveForward = cameraForward * _inputVertical + Camera.main.transform.right * _inputHorizontal;

        // 移動方向にスピードを掛ける。ジャンプや落下がある場合は、別途Y軸方向の速度ベクトルを足す。
        _rb.velocity = moveForward * _speed + new Vector3(0, _rb.velocity.y, 0);

        

        // キャラクターの向きを進行方向に
        if(_isRotate)
        {
            if(moveForward != Vector3.zero)
            {
                //回転に補間する
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(moveForward), Time.deltaTime * 15.0f);
                //補間しない
                //transform.rotation = Quaternion.LookRotation(moveForward);
            }
        }

        //移動animatorの表現
        if(isMove)
        {
            _animator.SetFloat("Move", _speedAnimatorParameter);
        }
        else
        {
            _speedAnimatorParameter = Mathf.Max(_speedAnimatorParameter - 0.1f, 0);
            _animator.SetFloat("Move", _speedAnimatorParameter);
        }
    }



    public void SetMovement(Vector3 direction)
    {
        // TODO : _isNormalModeについての制御は一時的なコードだけだ
        if(!_isNormalMode)
        {
            _inputHorizontal = 0;
            _inputVertical = 0;
            return;
        }

        isMove = direction.magnitude > 0.0f;
        _inputHorizontal = direction.x;
        _inputVertical = direction.z;
        if(isMove)
        {
            MovementAnimation(direction.magnitude);
        }
    }

    public void SetCrouched(bool isCrouched)
    {
        _animator.SetBool("isCrouched", isCrouched);
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
            _animator.SetTrigger("Attack");
            SetSpeed(0.0f);
        } 
    }

    public void DoAction()
    {
        ReactableType type = _reactableDetector.TriggerReactable();
        // TODO
        switch(type)
        {
            case ReactableType.None:
                print("何もアクションできない");
                break;
            case ReactableType.Key:
                print("鍵を取った！");
                break;
            case ReactableType.HidingPlace:
                _isNormalMode = !_isNormalMode;
                print(_isNormalMode ? "現れる" : "隠れる");
                SkinnedMeshRenderer[] renderers = GetComponentsInChildren<SkinnedMeshRenderer>();
                foreach(SkinnedMeshRenderer renderer in renderers)
                {
                    renderer.enabled = _isNormalMode;
                }
                
                break;
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
                _animator.SetTrigger("Jump");
                _animator.SetBool("isGround", false);
            }
        }
    }


    void OnCollisionEnter(Collision other) //地面に触れた時の処理
    {
        if(other.gameObject.tag == "Ground") //Groundタグのオブジェクトに触れたとき
        {
            _isGround = true; //isGroundをtrueにする
            _animator.SetBool("isGround", true);
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

    #region 攻撃を受ける

    public void Damage(float damage)
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
    }

    private void Die()
    {
        // TODO
        print("死亡");
        Died?.Invoke();
    }

    #endregion
}
