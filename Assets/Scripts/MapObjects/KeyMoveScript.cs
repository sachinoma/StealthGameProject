using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyMoveScript : MonoBehaviour
{
    [Header("動作確認時のみ切り替え：移動切り替え")]
    [SerializeField]
    private bool _isMoving = false;

    private enum MoveState { MoveUp, MoveDown }
    private MoveState _state = MoveState.MoveUp;
    private Transform _startTransform;
    [Header("最高移動量　移動速度")]
    [SerializeField]
    private float _maxMovePos = 1f;
    private float _nowMovePos = 0f;
    [SerializeField]
    private float _moveSpeed = 0.7f;

    [Header("回転スイッチ")]
    [SerializeField]
    private bool _isRot = false;
    [Header("回転速度")]
    [SerializeField]
    private float _rotSpeed = 20f;

    public void Moving(bool onoff)
    { _isMoving = onoff; }

    public void Rotate(bool onoff)
    { _isRot = onoff; }

    void Start()
    {
        _startTransform = transform;
    }

    private void StateChange()
    {
        if(_state == MoveState.MoveUp)
            _state = MoveState.MoveDown;
        else
            _state = MoveState.MoveUp;

        StateReset();
    }

    private void StateReset()
    {
        _nowMovePos = 0;
    }

    private void PositionReset()
    {
        transform.position = _startTransform.position;
        StateReset();
    }

    private void RotReset()
    {
        transform.rotation = _startTransform.rotation;
    }

    void Update()
    {
        if(_isMoving)
        {
            Vector3 Direct = _state == MoveState.MoveUp ? Vector3.up : Vector3.down;

            transform.position += Direct * _moveSpeed * Time.deltaTime;
            _nowMovePos += _moveSpeed * Time.deltaTime;

            if(_nowMovePos >= _maxMovePos)
                StateChange();
        }
        else
            PositionReset();

        if(_isRot)
        {
            transform.Rotate(Vector3.up * _rotSpeed * Time.deltaTime);
        }
        else
            RotReset();
    }
}