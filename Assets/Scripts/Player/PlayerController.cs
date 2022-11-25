﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private PlayerInput _input;
    [SerializeField]
    private PlayerModel _playerModel;

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
        //_input.actions["Attack"].started += OnAttack;
        _input.actions["Action"].started += OnAction;
        var normal = _input.actions.FindActionMap("Normal");
        var crouched = _input.actions.FindActionMap("Crouched");
        crouched["Move"].canceled += OnMoveStop;
        normal["Move"].canceled += OnMoveStop;
        crouched["Move"].performed += OnMove;    
        normal["Move"].performed += OnMove;      
        crouched["ModeChange"].started += ToNormalMode;
        normal["ModeChange"].started += ToCrouchedMode;
    }

    void OnDisable()
    {
        _input.actions["Jump"].started   -= OnJump;
        _input.actions["Dash"].started   -= OnDash;
        _input.actions["Dash"].canceled  -= OnDash;
        //_input.actions["Attack"].started -= OnAttack;
        _input.actions["Action"].started -= OnAction;
        var normal = _input.actions.FindActionMap("Normal");
        var crouched = _input.actions.FindActionMap("Crouched");
        crouched["Move"].canceled -= OnMoveStop;
        normal["Move"].canceled -= OnMoveStop;
        crouched["Move"].performed -= OnMove;
        normal["Move"].performed -= OnMove;     
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
        //Attackを押した瞬間
        _playerModel.Attack();
    }
    #endregion
    #region Action
    private void OnAction(InputAction.CallbackContext obj)
    {
        //Attackを押した瞬間
        _playerModel.DoAction();
    }
    #endregion
    #region Dash
    private void OnDash(InputAction.CallbackContext obj)
    {
        //Dashを押した瞬間
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
        //Moveを押した瞬間
        _playerModel.SetMovement(Vector3.zero);
    }

    private void OnMove(InputAction.CallbackContext obj)
    {
        //Moveを押した瞬間
        var value = obj.ReadValue<Vector2>();
        var direction = new Vector3(value.x, 0, value.y);
        _playerModel.SetMovement(direction);

    }

    private void OnMoveKeyboard(InputAction.CallbackContext obj)
    {
        //Moveを押した瞬間
        var value = obj.ReadValue<Vector2>();
        var direction = new Vector3(value.x, 0, value.y);
        _playerModel.SetMovement(direction);

    }
    #endregion

    #region Jump
    private void OnJump(InputAction.CallbackContext obj)
    {
        //Jumpを押した瞬間
        _playerModel.Jump();
    }
    #endregion
}
