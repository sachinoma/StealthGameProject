#define BACKDOOR_ENABLED

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

#if BACKDOOR_ENABLED
using UnityEngine.InputSystem;
#endif

public class MainSceneDirector : MonoBehaviour
{
    [SerializeField] private MainSceneUIManager _uiManager;
    [SerializeField] private AnnouncementManager _announcementManagerTemplate;
    [SerializeField] private GameOver _gameOverTemplate;

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
    }

    private void OnDestroy()
    {
        MainSceneEventManager.PlayerBrokeOut.Handler -= OnPlayerBrokeOut;
        MainSceneEventManager.PlayerDied.Handler -= OnPlayerDied;
    }

#if BACKDOOR_ENABLED
    private void Update()
    {
        if(!_isGameOver && Gamepad.current != null)
        {
            if(Gamepad.current.bButton.wasPressedThisFrame)
            {
                Retry();
            }
            else if(Gamepad.current.aButton.wasPressedThisFrame)
            {
                BackToTitle();
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

        StartCoroutine(WaitAndGameOver(7.0f));
    }

    private IEnumerator WaitAndGameOver(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        _announcementManager?.ClearCaption();
        GameOver gameOver = Instantiate(_gameOverTemplate);
        gameOver.Show(Retry, BackToTitle);
    }

    private void Retry()
    {
        // TODO : トランジション
        SceneManager.LoadScene("Main");
    }

    private void BackToTitle()
    {
        // TODO : トランジション
        SceneManager.LoadScene("Title");
    }
}
