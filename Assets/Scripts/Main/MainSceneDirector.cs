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
        PlayerModel player = FindObjectOfType<PlayerModel>();
        if(player == null)
        {
            Debug.LogError("PlayerModelが探せない");
        }
        else
        {
            _uiManager.AddObtainedItems(player.obtainedItems);
        }

        if(_isCursorInvisible)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        MainSceneEventManager.PlayerBrokeOut.Handler += OnPlayerBrokeOut;
        MainSceneEventManager.PlayerDied.Handler += OnPlayerDied;
        MainSceneEventManager.ItemGot.Handler += OnItemGot;
        MainSceneEventManager.GameClear.Handler += OnGameClear;
    }

    private void OnDestroy()
    {
        MainSceneEventManager.PlayerBrokeOut.Handler -= OnPlayerBrokeOut;
        MainSceneEventManager.PlayerDied.Handler -= OnPlayerDied;
        MainSceneEventManager.ItemGot.Handler -= OnItemGot;
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
        Debug.Log("ゲームオーバー");
        _isGameOver = true;

        StartCoroutine(WaitAndGameOver(5.0f));
    }

    private void OnItemGot(object sender, EventArgs e)
    {
        if(e is not PickUpEventArgs pickUpEventArgs)
        {
            Debug.LogWarning($"取得したアイテムはPickUpInfoを持っていない");
            return;
        }

        Debug.Log($"アイテムを取得した：{pickUpEventArgs.Type}");

        _uiManager.ShowItemGotMessage();
        _uiManager.AddObtainedItem(pickUpEventArgs.Type);
    }

    private void OnGameClear(object sender, EventArgs e)
    {
        Debug.Log("ゲームクリア");
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
