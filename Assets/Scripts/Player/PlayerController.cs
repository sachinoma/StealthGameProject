using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private PlayerInput _input;
    [SerializeField]
    private PlayerModel _playerModel;
    [SerializeField]
    private PlayerCameraTest _playerCameraTest;

    void Awake()
    {
        TryGetComponent(out _input);
    }
    #region SetUp
    void OnEnable()
    {
        _input.actions["Jump"].started   += OnJump;
        _input.actions["Dash"].started   += OnDash;
        _input.actions["Dash"].canceled  += OnDash;
        _input.actions["Attack"].started += OnAttack;
        var normal = _input.actions.FindActionMap("Normal");
        var crouched = _input.actions.FindActionMap("Crouched");
        crouched["Move"].canceled += OnMoveStop;
        normal["Move"].canceled += OnMoveStop;
        crouched["Move"].performed += OnMove;    
        normal["Move"].performed += OnMove;      
        crouched["Look"].performed += OnLook;
        crouched["Look"].canceled += OnLookStop;
        normal["Look"].performed += OnLook;
        normal["Look"].canceled += OnLookStop;
        crouched["ModeChange"].started += ToNormalMode;
        normal["ModeChange"].started += ToCrouchedMode;
    }

    void OnDisable()
    {
        _input.actions["Jump"].started   -= OnJump;
        _input.actions["Dash"].started   -= OnDash;
        _input.actions["Dash"].canceled  -= OnDash;
        _input.actions["Attack"].started -= OnAttack;
        var normal = _input.actions.FindActionMap("Normal");
        var crouched = _input.actions.FindActionMap("Crouched");
        crouched["Move"].canceled -= OnMoveStop;
        normal["Move"].canceled -= OnMoveStop;
        crouched["Move"].performed -= OnMove;
        normal["Move"].performed -= OnMove;     
        crouched["Look"].performed -= OnLook;
        crouched["Look"].canceled -= OnLookStop;
        normal["Look"].performed -= OnLook;
        normal["Look"].canceled -= OnLookStop;
        crouched["ModeChange"].started -= ToNormalMode;
        normal["ModeChange"].started -= ToCrouchedMode;
    }
    #endregion

    #region Mode
    private void ToCrouchedMode(InputAction.CallbackContext obj)
    {
        _input.SwitchCurrentActionMap("Crouched");
        _playerModel.SetCrouched(true);
    }

    private void ToNormalMode(InputAction.CallbackContext obj)
    {
        _input.SwitchCurrentActionMap("Normal");
        _playerModel.SetCrouched(false);
    }
    #endregion
    #region Attack
    private void OnAttack(InputAction.CallbackContext obj)
    {
        //AttackÇâüÇµÇΩèuä‘
        _playerModel.Attack();
    }
    #endregion
    #region Dash
    private void OnDash(InputAction.CallbackContext obj)
    {
        //DashÇâüÇµÇΩèuä‘
        switch (obj.phase)
        {
            case InputActionPhase.Started:
                _playerModel.SetDash(true);
                break;
            case InputActionPhase.Canceled:
                _playerModel.SetDash(false);
                break;
        }
    }
    #endregion
    #region Move
    private void OnMoveStop(InputAction.CallbackContext obj)
    {
        //MoveÇâüÇµÇΩèuä‘
        _playerModel.SetMovement(Vector3.zero);
    }

    private void OnMove(InputAction.CallbackContext obj)
    {
        //MoveÇâüÇµÇΩèuä‘
        var value = obj.ReadValue<Vector2>();
        var direction = new Vector3(value.x, 0, value.y);
        _playerModel.SetMovement(direction);

    }

    private void OnMoveKeyboard(InputAction.CallbackContext obj)
    {
        //MoveÇâüÇµÇΩèuä‘
        var value = obj.ReadValue<Vector2>();
        var direction = new Vector3(value.x, 0, value.y);
        _playerModel.SetMovement(direction);

    }
    #endregion
    #region Look
    private void OnLookStop(InputAction.CallbackContext obj)
    {
        //LookÇâüÇµÇΩèuä‘
        _playerCameraTest.SetMovement(Vector3.zero, 1.0f);
    }

    private void OnLook(InputAction.CallbackContext obj)
    {
        //LookÇâüÇµÇΩèuä‘
        float sensitivity = 1.0f;
        if(_input.currentControlScheme == "Keyboard")
        {
            sensitivity = 0.05f;
        }
        var value = obj.ReadValue<Vector2>();
        var direction = new Vector3(value.x, 0, value.y);
        _playerCameraTest.SetMovement(direction, sensitivity);
    }
    #endregion
    #region Jump
    private void OnJump(InputAction.CallbackContext obj)
    {
        //JumpÇâüÇµÇΩèuä‘
        _playerModel.Jump();
    }
    #endregion
}
