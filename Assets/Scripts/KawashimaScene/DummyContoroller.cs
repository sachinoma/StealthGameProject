using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DummyContoroller : MonoBehaviour
{
    //Dummyの移動速度
    [SerializeField, Range(1.0f, 10.0f), Tooltip("Dummyの移動速度")]
    private float _moveSpeed = 5f;

    //水平移動の入力
    private Vector2 _moveInput = Vector2.zero;

    //アイテムを投げる関数を持つコンポーネント
    private ThrowController _throwController;

    private PlayerInput _input;

    void Awake()
    {
        TryGetComponent(out _input);
    }

    private void Start()
    {
        _throwController = gameObject.GetComponent<ThrowController>();

        InputActionMap normal = _input.actions.FindActionMap("Normal");
        normal["Move"].performed += OnMove;
        normal["Move"].canceled += OnMoveStop;
        normal["Look"].performed += OnLook;
    }

    private void OnDestroy()
    {
        InputActionMap normal = _input.actions.FindActionMap("Normal");
        normal["Move"].performed -= OnMove;
        normal["Move"].canceled -= OnMoveStop;
        normal["Look"].performed -= OnLook;
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        var moveVector = Vector3.zero;
        moveVector += transform.forward * _moveInput.y * _moveSpeed * Time.deltaTime;
        moveVector += transform.right * _moveInput.x * _moveSpeed * Time.deltaTime;

        transform.position += moveVector;
    }

    #region User Input

    private void OnMove(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }

    private void OnMoveStop(InputAction.CallbackContext context)
    {
        _moveInput = Vector2.zero;
    }

    private void OnLook(InputAction.CallbackContext context)
    {
        //カメラの向きからY軸の向きを取得
        var horizontalRotation = Quaternion.AngleAxis(Camera.main.transform.eulerAngles.y, Vector3.up);
        //カメラの向く正面方向への軸を作成
        var lookFront = horizontalRotation * new Vector3(0, 0, 1);

        //正面へ向かせる
        transform.rotation = Quaternion.LookRotation(lookFront, Vector3.up);
    }

    private void OnAttack(InputAction.CallbackContext context)
    {
        _throwController.ShootItem();
    }

    #endregion
}
