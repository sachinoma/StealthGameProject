#define BACKDOOR_ENABLED

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if BACKDOOR_ENABLED
using UnityEngine.InputSystem;
#endif

public class MainSceneDirector : MonoBehaviour
{
    [SerializeField] private MainSceneUIManager _uiManager;
    [SerializeField] private AnnouncementManager _announcementManagerTemplate;

    [Header("カード")]
    [SerializeField] private Transform _cardsBaseTrasnform;

    [Header("扉と関連すること")]
    [SerializeField] private Door _tutorialDoor;
    [SerializeField] private Door _endpointDoor;
    [SerializeField] private AudioSource _endpointAudio;

    [Header("カーソルの表示")]
    [SerializeField] private bool _isCursorInvisible = true;

    private AnnouncementManager _announcementManager = null;

    private bool _isGameOver = false;

    private List<CardType> _operatedCardTypes = new List<CardType>();

    private void Start()
    {
        LoadGameProgress();

        if(_isCursorInvisible)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        MainSceneEventManager.PlayerBrokeOut.Handler += OnPlayerBrokeOut;
        MainSceneEventManager.PlayerDied.Handler += OnPlayerDied;
        MainSceneEventManager.ItemGot.Handler += OnItemGot;
        MainSceneEventManager.GameClear.Handler += OnGameClear;
        MainSceneEventManager.TerminalOperated.Handler += OnTerminalOperated;
    }

    private void OnDestroy()
    {
        MainSceneEventManager.PlayerBrokeOut.Handler -= OnPlayerBrokeOut;
        MainSceneEventManager.PlayerDied.Handler -= OnPlayerDied;
        MainSceneEventManager.ItemGot.Handler -= OnItemGot;
        MainSceneEventManager.GameClear.Handler -= OnGameClear;
        MainSceneEventManager.TerminalOperated.Handler -= OnTerminalOperated;
    }

    private void LoadGameProgress()
    {
        List<CardType> obtainedItems = GameProgress.ObtainedItems;
        _uiManager.AddObtainedItems(obtainedItems);

        SetCardsActive(obtainedItems);
    }

    private void SetCardsActive(List<CardType> obtainedItems)
    {
        if(obtainedItems == null)
        {
            return;
        }

        PickUpItem[] cardsInMap = _cardsBaseTrasnform.GetComponentsInChildren<PickUpItem>();
        if(cardsInMap == null)
        {
            return;
        }

        foreach(CardType obtainedItem in obtainedItems)
        {
            foreach(PickUpItem cardInMap in cardsInMap)
            {
                if(cardInMap.GetCardType() == obtainedItem)
                {
                    Destroy(cardInMap.gameObject);
                    break;
                }
            }
        }
    }

#if BACKDOOR_ENABLED
    private void LateUpdate()
    {
        if(!_isGameOver)
        {
            if((Gamepad.current != null && Gamepad.current.bButton.wasPressedThisFrame) ||
               (Keyboard.current != null && Keyboard.current.digit9Key.wasPressedThisFrame))
            {
                SceneControl.ChangeScene(SceneControl.MainSceneName);
            }
            else if((Gamepad.current != null && Gamepad.current.aButton.wasPressedThisFrame) ||
                    (Keyboard.current != null && Keyboard.current.digit0Key.wasPressedThisFrame))
            {
                SceneControl.ChangeScene(SceneControl.TitleSceneName);
            }
            else if((Gamepad.current != null && Gamepad.current.rightShoulder.wasPressedThisFrame) ||
                    (Keyboard.current != null && Keyboard.current.digit1Key.wasPressedThisFrame))
            {
                // 終点の辺り
                PlayerModel player = FindObjectOfType<PlayerModel>();
                player.transform.position = new Vector3(-6, 0, 0);
                player.transform.eulerAngles = new Vector3(0, 90, 0);
            }
            else if((Gamepad.current != null && Gamepad.current.leftShoulder.wasPressedThisFrame) ||
                    (Keyboard.current != null && Keyboard.current.digit2Key.wasPressedThisFrame))
            {
                // Card_Blueの辺り
                PlayerModel player = FindObjectOfType<PlayerModel>();
                player.transform.position = new Vector3(-12.8f, 0, -39.1f);
                player.transform.eulerAngles = new Vector3(0, 135, 0);
            }
            else if((Gamepad.current != null && Gamepad.current.rightTrigger.wasPressedThisFrame) ||
                    (Keyboard.current != null && Keyboard.current.digit3Key.wasPressedThisFrame))
            {
                // Card_Redの辺り
                PlayerModel player = FindObjectOfType<PlayerModel>();
                player.transform.position = new Vector3(28.6f, 0, 23.3f);
                player.transform.eulerAngles = new Vector3(0, 135, 0);
            }
            else if((Gamepad.current != null && Gamepad.current.leftTrigger.wasPressedThisFrame) ||
                    (Keyboard.current != null && Keyboard.current.digit4Key.wasPressedThisFrame))
            {
                // Card_Yellowの辺り
                PlayerModel player = FindObjectOfType<PlayerModel>();
                player.transform.position = new Vector3(-24.45f, 0, 0f);
                player.transform.eulerAngles = new Vector3(0, 270, 0);
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

    private IEnumerator WaitAndGameOver(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        _announcementManager?.ClearCaption();
        SceneControl.LoadUI(SceneControl.GameOverUISceneName);
    }

    private void OnItemGot(object sender, EventArgs e)
    {
        if(e is not ItemGotEventArgs itemGotEventArgs)
        {
            Debug.LogWarning($"取得したアイテムは正しいEventArgsを持っていない");
            return;
        }

        Debug.Log($"アイテムを取得した：{itemGotEventArgs.Type}");

        _uiManager.ShowItemGotMessage();
        _uiManager.AddObtainedItem(itemGotEventArgs.Type);
    }

    private void OnGameClear(object sender, EventArgs e)
    {
        Debug.Log("ゲームクリア");
        _isGameOver = true;
        _endpointAudio.Stop();

        SceneControl.LoadUI(SceneControl.GameClearUISceneName);
    }

    private void OnTerminalOperated(object sender, EventArgs e)
    {
        if(e is not TerminalOperatedEventArgs terminalOperatedEventArgs)
        {
            Debug.LogWarning($"EventArgsが合っていない");
            return;
        }

        switch(terminalOperatedEventArgs.Type)
        {
            case CardType.White:
                _tutorialDoor.Open();
                return;
            case CardType.Red:
            case CardType.Blue:
            case CardType.Yellow:
                _operatedCardTypes.Add(terminalOperatedEventArgs.Type);
                CheckAndActivateEndpoint();
                return;
        }
    }

    private void CheckAndActivateEndpoint()
    {
        CardType[] cardTypeNeeded = { CardType.Red, CardType.Blue, CardType.Yellow };
        foreach(CardType cardType in cardTypeNeeded)
        {
            if(!_operatedCardTypes.Contains(cardType))
            {
                return;
            }
        }

        ActivateEndpoint();
    }

    private void ActivateEndpoint()
    {
        _endpointDoor.Open();
        _endpointAudio.Play();
    }
}
