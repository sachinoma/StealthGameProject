using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PrototypeSceneDirector : MonoBehaviour
{
    private PlayerModel _playerModel;

    [Header("カーソルの表示")]
    [SerializeField]
    bool InvisibleCursor = true;

    private void Start()
    {
        _playerModel = FindObjectOfType<PlayerModel>();
        if(_playerModel != null)
        {
            _playerModel.Died += PlayerDiedHandler;
        }

        if(InvisibleCursor)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    private void OnDestroy()
    {
        if(_playerModel != null)
        {
            _playerModel.Died -= PlayerDiedHandler;
        }
    }

    private void Update()
    {
        if(Keyboard.current.mKey.wasPressedThisFrame)
        {
            MicRange micRange = FindObjectOfType<MicRange>();
            if(micRange != null)
            {
                micRange.SwitchMicMode();
                FindObjectOfType<PrototypeSceneUIManager>()?.UpdateInputModeText(micRange.IsMicMode);
            }
        }
        else if(Gamepad.current.bButton.wasPressedThisFrame)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else if(Gamepad.current.aButton.wasPressedThisFrame)
        {
            SceneManager.LoadScene("Title");
        }
    }

    private void PlayerDiedHandler()
    {
        Debug.LogWarning("Game Over!!");

        //PrototypeMessageEvent.Invoke($"やられました！");
        StartCoroutine(RestartGame(7.0f));
    }

    private IEnumerator RestartGame(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
