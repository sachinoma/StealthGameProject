using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameClear : MonoBehaviour
{
    [Header("ボタン")]
    [SerializeField] private Button _homeButton;

    [Header("テキスト")]
    [SerializeField] private TextMeshProUGUI _gameClearText;
    [SerializeField] private TextMeshProUGUI _homeText;

    PlayerInput _playerInput;

    // Start is called before the first frame update
    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        _gameClearText.text = LocalizedText.GameClear;
        _homeText.text = LocalizedText.Home;

        _playerInput = FindObjectOfType<PlayerInput>();
        _playerInput?.SwitchCurrentActionMap("UI");

        _homeButton.onClick.AddListener(OnHomeBtnClicked);
    }

    private void OnDestroy()
    {
        _homeButton.onClick.RemoveListener(OnHomeBtnClicked);
    }

    private void OnMenuShowingAnimEvent()
    {
        EventSystem.current?.SetSelectedGameObject(_homeButton.gameObject);
    }

    private void OnHomeBtnClicked()
    {
        SceneControl.ChangeScene(SceneControl.TitleSceneName);
    }
}
