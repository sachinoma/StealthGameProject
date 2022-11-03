using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerModel : MonoBehaviour
{
    private float _inputHorizontal;
    private float _inputVertical;
    private Rigidbody _rb;
    [SerializeField]
    private float _moveSpeed = 3f;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
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
        _rb.velocity = moveForward * _moveSpeed + new Vector3(0, _rb.velocity.y, 0);

        // キャラクターの向きを進行方向に
        if(moveForward != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(moveForward);
        }
    }

    public void SetMovement(Vector3 direction)
    {
        _inputHorizontal = direction.x;
        _inputVertical = direction.z;
    }

    public void SetDash(bool isDash)
    {

    }

    public void Jump()
    {
        Debug.Log("Jump!");
    }


}
