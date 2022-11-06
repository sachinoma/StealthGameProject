using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DummyContoroller : MonoBehaviour, PlayerInputAction.IPlayerActions
{
    //Dummy�̈ړ����x
    [SerializeField, Range(1.0f, 10.0f), Tooltip("Dummy�̈ړ����x")]
    private float _moveSpeed = 5f;

    //�����ړ��̓���
    private Vector2 _moveInput = Vector2.zero;

    //�A�C�e���𓊂���֐������R���|�[�l���g
    private ThrowController _throwController;

    private void Start()
    {
        PlayerInputAction userinput = new PlayerInputAction();
        userinput.Player.SetCallbacks(this);
        userinput.Player.Enable();

        _throwController = gameObject.GetComponent<ThrowController>();
    }

    private void Update()
    {
        var keybard = Keyboard.current;

        if(keybard.spaceKey.wasPressedThisFrame)
            _throwController.ShootItem();

        Move();
    }

    private void Move()
    {
        var moveVector = Vector3.zero;
        moveVector += transform.forward * _moveInput.y * _moveSpeed * Time.deltaTime;
        moveVector += transform.right * _moveInput.x * _moveSpeed * Time.deltaTime;

        transform.position += moveVector;
    }

    #region PlayerInputAction.IPlayerActions

    public void OnMove(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        //�J�����̌�������Y���̌������擾
        var horizontalRotation = Quaternion.AngleAxis(Camera.main.transform.eulerAngles.y, Vector3.up);
        //�J�����̌������ʕ����ւ̎����쐬
        var lookFront = horizontalRotation * new Vector3(0, 0, 1);

        //���ʂ֌�������
        transform.rotation = Quaternion.LookRotation(lookFront, Vector3.up);
    }

    public void OnFire(InputAction.CallbackContext context)
    {
    }

    public void OnJump(InputAction.CallbackContext context)
    {
    }

    #endregion
}
