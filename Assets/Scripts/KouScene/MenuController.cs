using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    PlayerInput _input;

    [SerializeField]
    private string _sceneName;

    //現在のアクションマップの取得
    private InputActionMap map;

    void Awake()
    {
        TryGetComponent(out _input);
    }

    void OnEnable()
    {
        var normal = _input.actions.FindActionMap("Normal");
        var crouched = _input.actions.FindActionMap("Crouched");
        normal["OpenMenu"].started += OnOpenMenu;
        crouched["OpenMenu"].started += OnOpenMenu;
        _input.actions["CloseMenu"].started += OnCloseMenu;
    }

    void OnDisable()
    {
        var normal = _input.actions.FindActionMap("Normal");
        var crouched = _input.actions.FindActionMap("Crouched");
        normal["OpenMenu"].started -= OnOpenMenu;
        crouched["OpenMenu"].started -= OnOpenMenu;
        _input.actions["CloseMenu"].started -= OnCloseMenu;
    }

    private void OnCloseMenu(InputAction.CallbackContext obj)
    {
        SceneManager.UnloadSceneAsync(_sceneName);
        _input.currentActionMap = map;
    }

    private void OnOpenMenu(InputAction.CallbackContext obj)
    {
        SceneManager.LoadSceneAsync(_sceneName, LoadSceneMode.Additive);
        map = _input.currentActionMap;
        _input.SwitchCurrentActionMap("UI");
    }
}
