#define BACKDOOR_ENABLED

using System;
using System.Collections;
using UnityEngine;

#if BACKDOOR_ENABLED
using UnityEngine.InputSystem;
#endif

public class MainSceneDirector : MonoBehaviour
{
    [SerializeField] private MainSceneUIManager _uiManager;
    [SerializeField] private AnnouncementManager _announcementManagerTemplate;

    [Header("カーソルの表示")]
    [SerializeField] private bool _isCursorInvisible = true;

    private AnnouncementManager _announcementManager = null;

    private bool _isGameOver = false;

    private void Start()
    {
        if(_isCursorInvisible)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        MainSceneEventManager.PlayerBrokeOut.Handler += OnPlayerBrokeOut;
        MainSceneEventManager.PlayerDied.Handler += OnPlayerDied;
        MainSceneEventManager.GameClear.Handler += OnGameClear;
    }

    private void OnDestroy()
    {
        MainSceneEventManager.PlayerBrokeOut.Handler -= OnPlayerBrokeOut;
        MainSceneEventManager.PlayerDied.Handler -= OnPlayerDied;
        MainSceneEventManager.GameClear.Handler -= OnGameClear;
    }

#if BACKDOOR_ENABLED
    private void Update()
    {
        if(!_isGameOver && Gamepad.current != null)
        {
            if(Gamepad.current.bButton.wasPressedThisFrame)
            {
                SceneControl.ChangeScene(SceneControl.MainSceneName);
            }
            else if(Gamepad.current.aButton.wasPressedThisFrame)
            {
                SceneControl.ChangeScene(SceneControl.TitleSceneName);
            }
        }
    }
#endif

    private void OnPlayerBrokeOut(object sender, EventArgs e)
    {
        if(_announcementManager == null)
        {
            _announcementManager = Instantiate(_announcementManagerTemplate);
        }

        _announcementManager.PlayBreakOutAnnouncement();
    }

    private void OnPlayerDied(object sender, EventArgs e)
    {
        Debug.LogWarning("Game Over!!");
        _isGameOver = true;

        StartCoroutine(WaitAndGameOver(5.0f));
    }

    private void OnGameClear(object sender, EventArgs e)
    {
        Debug.LogWarning("Game Clear!!");
        _isGameOver = true;

        SceneControl.LoadUI(SceneControl.GameClearUISceneName);
    }
    
    private IEnumerator WaitAndGameOver(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        _announcementManager?.ClearCaption();
        SceneControl.LoadUI(SceneControl.GameOverUISceneName);
    }
}
