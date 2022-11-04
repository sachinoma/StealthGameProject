using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.InputSystem;

public class PlayerModel : MonoBehaviour
{
    //Animator
    [SerializeField]
    private Animator _animator;

    //�X�s�[�h
    [SerializeField]
    private float _basicMoveSpeed = 10.0f;
    private float _speed;
    private float _speedAnimatorParameter = 0.0f;
    private bool isMove = false;
    //��]
    [SerializeField]
    private bool _isRotate = true; //��]�ł��邩�ǂ����̔���
    //HP
    [SerializeField]
    private float _life = 10.0f;
    //�W�����v
    [SerializeField]
    private float _upForce = 200f; //������ɂ������
    private bool  _isGround; //���n���Ă��邩�ǂ����̔���

    private float _inputHorizontal;
    private float _inputVertical;
    private Rigidbody _rb;


    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _speed = _basicMoveSpeed;
    }

    void Update()
    {

    }

    void FixedUpdate()
    {
        // �J�����̕�������AX-Z���ʂ̒P�ʃx�N�g�����擾
        Vector3 cameraForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;

        // �����L�[�̓��͒l�ƃJ�����̌�������A�ړ�����������
        Vector3 moveForward = cameraForward * _inputVertical + Camera.main.transform.right * _inputHorizontal;

        // �ړ������ɃX�s�[�h���|����B�W�����v�◎��������ꍇ�́A�ʓrY�������̑��x�x�N�g���𑫂��B
        _rb.velocity = moveForward * _speed + new Vector3(0, _rb.velocity.y, 0);

        

        // �L�����N�^�[�̌�����i�s������
        if(_isRotate)
        {
            if(moveForward != Vector3.zero)
            {
                //��]�ɕ�Ԃ���
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(moveForward), Time.deltaTime * 15.0f);
                //��Ԃ��Ȃ�
                //transform.rotation = Quaternion.LookRotation(moveForward);
            }
        }

        //�ړ�animator�̕\��
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

    public void Jump()
    {
        if(_isGround)
        {
            if(CheckAnimatorState("BasicMovement"))
            {
                _isGround = false;//  isGround��false�ɂ���
                _rb.AddForce(new Vector3(0, _upForce, 0)); //��Ɍ������ė͂�������
                _animator.SetTrigger("Jump");
                _animator.SetBool("isGround", false);
            }
        }
    }


    void OnCollisionEnter(Collision other) //�n�ʂɐG�ꂽ���̏���
    {
        if(other.gameObject.tag == "Ground") //Ground�^�O�̃I�u�W�F�N�g�ɐG�ꂽ�Ƃ�
        {
            _isGround = true; //isGround��true�ɂ���
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
}
