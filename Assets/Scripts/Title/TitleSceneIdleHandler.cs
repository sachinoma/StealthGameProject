using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TitleSceneIdleHandler : MonoBehaviour
{
    [SerializeField] private PlayerInput _input;

    [SerializeField] private float _idleTimeToDemoScene;
    private float _startIdleTime;

    // Start is called before the first frame update
    void Start()
    {
        if (_input != null) {
            _input.actions["Navigate"].started += OnInputStarted;
            _input.actions["Submit"].started += OnInputStarted;
        }

        _startIdleTime = Time.time;
    }

    private void OnDestroy()
    {
        if (_input != null) {
            _input.actions["Navigate"].started -= OnInputStarted;
            _input.actions["Submit"].started -= OnInputStarted;
        }
    }

    void Update()
    {
        if (Time.time - _startIdleTime > _idleTimeToDemoScene) {
            SceneControl.ChangeScene(SceneControl.DemoSceneName);
            enabled = false;
        }
    }

    private void OnInputStarted(InputAction.CallbackContext obj)
    {
        _startIdleTime = Time.time;
    }
}
