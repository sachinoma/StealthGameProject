using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DummyContoroller : MonoBehaviour, PlayerInputAction.IPlayerActions
{
    //Dummyの移動速度
    [SerializeField, Range(1.0f, 10.0f), Tooltip("Dummyの移動速度")]
    private float _moveSpeed = 5f;

    //水平移動の入力
    private Vector2 _moveInput = Vector2.zero;

    //アイテムを投げる関数を持つコンポーネント
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
        //カメラの向きからY軸の向きを取得
        var horizontalRotation = Quaternion.AngleAxis(Camera.main.transform.eulerAngles.y, Vector3.up);
        //カメラの向く正面方向への軸を作成
        var lookFront = horizontalRotation * new Vector3(0, 0, 1);

        //正面へ向かせる
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
