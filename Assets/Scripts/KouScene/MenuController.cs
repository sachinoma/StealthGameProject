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

    void Awake()
    {
        TryGetComponent(out _input);
    }

    void OnEnable()
    {
        var normal = _input.actions.FindActionMap("Normal");
        var battle = _input.actions.FindActionMap("Battle");
        normal["OpenMenu"].started += OnOpenMenu;
        battle["OpenMenu"].started += OnOpenMenu;
        _input.actions["CloseMenu"].started += OnCloseMenu;
    }

    void OnDisable()
    {
        var normal = _input.actions.FindActionMap("Normal");
        var battle = _input.actions.FindActionMap("Battle");
        normal["OpenMenu"].started -= OnOpenMenu;
        battle["OpenMenu"].started -= OnOpenMenu;
        _input.actions["CloseMenu"].started -= OnCloseMenu;
    }

    private void OnCloseMenu(InputAction.CallbackContext obj)
    {
        SceneManager.UnloadSceneAsync(_sceneName);
        _input.SwitchCurrentActionMap("Normal");
    }

    private void OnOpenMenu(InputAction.CallbackContext obj)
    {
        SceneManager.LoadSceneAsync(_sceneName, LoadSceneMode.Additive);
        _input.SwitchCurrentActionMap("UI");
    }
}
