using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private PlayerCamera _playerCamera;
    [SerializeField] private CharacterController _characterController;

    [SerializeField] private float _maxSpeed = 3.0f;
    [SerializeField] private float _acceleration = 10.0f;
    [SerializeField] private float _rotateSpeed = 15.0f;

    [SerializeField, Range(0.0f, 1.0f), Tooltip("小さすぎる移動を防ぐ")]
    private float _minMovementThreshold = 0.1f;
    private float _minMovementThresholdSquare;  // _minMovementThresholdの平方

    private Vector2 _targetMovement;        // 希望な移動。カメラ空間座標(2Dのみ)。magnitudeの範囲 : [0, 1]
    private Vector3 _inputVelocity;         // _targetMovementによって計算後の入力速度。実際の速度ではない(壁をぶつけるとか、実際の速度が変わる場合もある)

    private bool _isMovementActive = true;

    private void Awake()
    {
        UpdateMinMovementThresholdSquare();
    }

    private void OnValidate()
    {
        UpdateMinMovementThresholdSquare();
    }

    private void Update()
    {
        if(!_isMovementActive)
        {
            return;
        }

        Vector3 targetMovement_WorldSpace = ConvertToWorldSpaceTargetMovement(_targetMovement);
        Move(targetMovement_WorldSpace);
        Rotate(_inputVelocity);
    }

    private void UpdateMinMovementThresholdSquare()
    {
        _minMovementThresholdSquare = _minMovementThreshold * _minMovementThreshold;
    }

    private Vector3 ConvertToWorldSpaceTargetMovement(Vector2 targetMovement_2DCameraSpace)
    {
        if(targetMovement_2DCameraSpace == Vector2.zero)
        {
            return Vector3.zero;
        }

        if(targetMovement_2DCameraSpace.sqrMagnitude < _minMovementThresholdSquare)
        {
            return Vector3.zero;
        }

        Vector3 cameraForward = Vector3.Scale(_playerCamera.Cam.transform.forward, new Vector3(1, 0, 1)).normalized;
        return _playerCamera.Cam.transform.right * targetMovement_2DCameraSpace.x + cameraForward * targetMovement_2DCameraSpace.y;
    }

    private void Move(Vector3 targetMovement_WorldSpace)
    {
        if(targetMovement_WorldSpace != Vector3.zero || _inputVelocity != Vector3.zero)
        {
            Vector3 targetVelocity = targetMovement_WorldSpace * _maxSpeed;
            _inputVelocity = Vector3.RotateTowards(_inputVelocity, targetVelocity, _rotateSpeed * Time.deltaTime, _acceleration * Time.deltaTime);
        }

        // 重力を模擬するために、たとえ _inputVelocity = Vector3.zero でも SimpleMove を呼び出す必要がある
        _characterController.SimpleMove(_inputVelocity);
    }

    private void Rotate(Vector3 forward)
    {
        if (forward != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(forward);
        }
    }

    /// <param name="targetMovement">カメラ空間座標(2Dのみ)<br />magnitudeの範囲 : [0, 1]</param>
    public void SetTargetMovement(Vector2 targetMovement)
    {
        _targetMovement = targetMovement;
    }

    public float GetNormalized01InputSpeed()
    {
        if(_isMovementActive)
        {
            return _inputVelocity.magnitude / _maxSpeed;
        }
        else
        {
            return 0;
        }
    }

    public void SetMovementActive(bool isActive)
    {
        _isMovementActive = isActive;
        if(!isActive)
        {
            _targetMovement = Vector2.zero;
            _inputVelocity = Vector3.zero;
        }
    }
}
