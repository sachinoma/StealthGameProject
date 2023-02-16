//#define BACKDOOR_ENABLED

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MainSceneDirector : MonoBehaviour
{
    [SerializeField] private MainSceneUIManager _uiManager;
    [SerializeField] private AnnouncementManager _announcementManagerTemplate;

    [Header("カード")]
    [SerializeField] private Transform _cardsBaseTransform;

    [Header("扉と関連すること")]
    [SerializeField] private Door _tutorialDoor;
    [SerializeField] private Door[] _endpointDoors;
    [SerializeField] private AudioSource _endpointAudio;

    [Header("カーソルの表示")]
    [SerializeField] private bool _isCursorInvisible = true;

    private AnnouncementManager _announcementManager = null;

    private List<CardType> _operatedCardTypes = new List<CardType>();

    private void Start()
    {
        LoadGameProgress();

        if(_isCursorInvisible)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        MainSceneEventManager.PlayerDied.Handler += OnPlayerDied;
        MainSceneEventManager.ItemGot.Handler += OnItemGot;
        MainSceneEventManager.GameClear.Handler += OnGameClear;
        MainSceneEventManager.TerminalOperated.Handler += OnTerminalOperated;
        MainSceneEventManager.HintTriggered.Handler += OnHintTriggered;

#if BACKDOOR_ENABLED || UNITY_EDITOR
        gameObject.AddComponent<MainSceneBackdoor>();
#endif
    }

    private void OnDestroy()
    {
        MainSceneEventManager.PlayerDied.Handler -= OnPlayerDied;
        MainSceneEventManager.ItemGot.Handler -= OnItemGot;
        MainSceneEventManager.GameClear.Handler -= OnGameClear;
        MainSceneEventManager.TerminalOperated.Handler -= OnTerminalOperated;
        MainSceneEventManager.HintTriggered.Handler -= OnHintTriggered;
    }

    private void Update()
    {
        // 簡単にタイトルシーンに戻るように
        if((Gamepad.current != null && Gamepad.current.leftShoulder.isPressed && Gamepad.current.startButton.isPressed) ||
           (Keyboard.current != null && Keyboard.current.digit0Key.wasPressedThisFrame))
        {
            SceneControl.ChangeScene(SceneControl.TitleSceneName);
        }
    }

    private void LoadGameProgress()
    {
        // カード
        List<CardType> obtainedItems = GameProgress.ObtainedItems;
        _uiManager.AddObtainedItems(obtainedItems);
        SetCardsActive(obtainedItems);

        // チュートリアルルーム
        if(GameProgress.IsTutorialRoomOpened)
        {
            _tutorialDoor.Open();
            _uiManager.SetItemUsed(GameProgress.GetTutorialCardType());
        }
    }

    private void SetCardsActive(List<CardType> obtainedItems)
    {
        if(obtainedItems == null)
        {
            return;
        }

        PickUpItem[] cardsInMap = _cardsBaseTransform.GetComponentsInChildren<PickUpItem>();
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

    #region Broke out announcement

    private IEnumerator DelayPlayBrokeOutAnnouncement(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);

        if(_announcementManager == null)
        {
            _announcementManager = Instantiate(_announcementManagerTemplate);
        }

        _announcementManager.PlayBreakOutAnnouncement();
    }

    #endregion

    #region OnPlayerDied

    private void OnPlayerDied(object sender, EventArgs e)
    {
        Debug.Log("ゲームオーバー");

        StartCoroutine(WaitAndGameOver(5.0f));

#if BACKDOOR_ENABLED || UNITY_EDITOR
        gameObject.GetComponent<MainSceneBackdoor>()?.SetBackdoorActive(false);
#endif
    }

    private IEnumerator WaitAndGameOver(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        _announcementManager?.ClearCaption();
        SceneControl.LoadUI(SceneControl.GameOverUISceneName);
    }

    #endregion

    #region OnItemGot

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

    #endregion

    #region OnGameClear

    private void OnGameClear(object sender, EventArgs e)
    {
        Debug.Log("ゲームクリア");

        _endpointAudio.Stop();

        SceneControl.LoadUI(SceneControl.GameClearUISceneName);

#if BACKDOOR_ENABLED || UNITY_EDITOR
        gameObject.GetComponent<MainSceneBackdoor>()?.SetBackdoorActive(false);
#endif
    }

    #endregion

    #region OnTerminalOperated

    private void OnTerminalOperated(object sender, EventArgs e)
    {
        if(e is not TerminalOperatedEventArgs terminalOperatedEventArgs)
        {
            Debug.LogWarning($"EventArgsが合っていない");
            return;
        }

        _uiManager.SetItemUsed(terminalOperatedEventArgs.Type);

        if(terminalOperatedEventArgs.Type == GameProgress.GetTutorialCardType())
        {
            _tutorialDoor.Open();
            StartCoroutine(DelayPlayBrokeOutAnnouncement(0.5f));
            GameProgress.SaveTutorialDone();
        }
        else
        {
            _operatedCardTypes.Add(terminalOperatedEventArgs.Type);
            CheckAndActivateEndpoint();
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
        foreach(Door door in _endpointDoors)
        {
            door.Open();
        }

        _endpointAudio.PlayDelayed(1.0f);
    }

    #endregion

    #region OnHintTriggered

    private void OnHintTriggered(object sender, EventArgs e)
    {
        if(e is not HintEventArgs hintEventArgs)
        {
            Debug.LogWarning($"EventArgsが合っていない");
            return;
        }

        _uiManager.ShowHint(hintEventArgs.Type);
    }

    #endregion
}
