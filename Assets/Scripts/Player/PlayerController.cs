using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    /*
    [SerializeField]
    private Text _mode;
    */

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
        var normal = _input.actions.FindActionMap("Normal");
        var battle = _input.actions.FindActionMap("Battle");
        battle["ModeChange"].started += ToNormalMode;
        normal["ModeChange"].started += ToBattleMode;
        battle["Move"].performed += OnMove;
        battle["Move"].canceled += OnMoveStop;
        normal["Move"].performed += OnMove;
        normal["Move"].canceled += OnMoveStop;
        battle["Look"].performed += OnLook;
        battle["Look"].canceled += OnLookStop;
        normal["Look"].performed += OnLook;
        normal["Look"].canceled += OnLookStop;
    }

    void OnDisable()
    {
        _input.actions["Jump"].started   -= OnJump;
        _input.actions["Dash"].started   -= OnDash;
        _input.actions["Dash"].canceled  -= OnDash;
        var normal = _input.actions.FindActionMap("Normal");
        var battle = _input.actions.FindActionMap("Battle");
        battle["ModeChange"].started -= ToNormalMode;
        normal["ModeChange"].started -= ToBattleMode;
        battle["Move"].performed -= OnMove;
        battle["Move"].canceled -= OnMoveStop;
        normal["Move"].performed -= OnMove;
        normal["Move"].canceled -= OnMoveStop;
        battle["Look"].performed -= OnLook;
        battle["Look"].canceled -= OnLookStop;
        normal["Look"].performed -= OnLook;
        normal["Look"].canceled -= OnLookStop;
    }
    #endregion

    #region Mode
    private void ToBattleMode(InputAction.CallbackContext obj)
    {
        _input.SwitchCurrentActionMap("Battle");
        //_mode.text = "Battle";
    }

    private void ToNormalMode(InputAction.CallbackContext obj)
    {
        _input.SwitchCurrentActionMap("Normal");
        //_mode.text = "Normal";
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
