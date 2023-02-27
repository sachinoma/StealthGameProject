using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class GameOver : MonoBehaviour
{
    [SerializeField] private GameObject _gameOverObject;
    [SerializeField] private GameObject _menu;

    [Header("ボタン")]
    [SerializeField] private Button _retryButton;
    [SerializeField] private Button _homeButton;

    [Header("テキスト")]
    [SerializeField] private TextMeshProUGUI _gameOverText;
    [SerializeField] private TextMeshProUGUI _retryText;
    [SerializeField] private TextMeshProUGUI _homeText;

    PlayerInput _playerInput;

    // Start is called before the first frame update
    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        _gameOverText.text = LocalizedText.GameOver;
        _retryText.text = LocalizedText.Retry;
        _homeText.text = LocalizedText.Home;

        _playerInput = FindObjectOfType<PlayerInput>();
        _playerInput?.SwitchCurrentActionMap("UI");

        _retryButton.onClick.AddListener(OnRetryBtnClicked);
        _homeButton.onClick.AddListener(OnHomeBtnClicked);
    }


    private void OnDestroy()
    {
        _retryButton.onClick.RemoveListener(OnRetryBtnClicked);
        _homeButton.onClick.RemoveListener(OnHomeBtnClicked);
    }

    private void OnMenuShowingAnimEvent()
    {
        EventSystem.current?.SetSelectedGameObject(_retryButton.gameObject);
    }

    private void OnRetryBtnClicked()
    {
        SceneControl.ChangeScene(SceneControl.MainSceneName);
    }

    private void OnHomeBtnClicked()
    {
        SceneControl.ChangeScene(SceneControl.TitleSceneName);
    }
}
