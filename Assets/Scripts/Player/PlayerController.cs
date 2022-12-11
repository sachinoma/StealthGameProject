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

    void Awake()
    {
        TryGetComponent(out _input);
    }

    #region SetUp
    void OnEnable()
    {
        _input.actions["Action"].started += OnAction;
        _input.actions["Shout"].performed += OnShoutBtnDown;
        _input.actions["Shout"].canceled += OnShoutBtnUp;
        _input.actions["Move"].canceled += OnMoveStop;
        _input.actions["Move"].performed += OnMove;
    }

    void OnDisable()
    {
        _input.actions["Action"].started -= OnAction;
        _input.actions["Shout"].performed -= OnShoutBtnDown;
        _input.actions["Shout"].canceled -= OnShoutBtnUp;
        _input.actions["Move"].canceled -= OnMoveStop;
        _input.actions["Move"].performed -= OnMove;
    }
    #endregion

    #region Action
    private void OnAction(InputAction.CallbackContext obj)
    {
        //Attackを押した瞬間
        _playerModel.DoAction();
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

    #region Shout

    private void OnShoutBtnDown(InputAction.CallbackContext obj)
    {
        _playerModel.StartShoutCharge();
    }

    private void OnShoutBtnUp(InputAction.CallbackContext obj)
    {
        _playerModel.Shout();
    }

    #endregion
}
