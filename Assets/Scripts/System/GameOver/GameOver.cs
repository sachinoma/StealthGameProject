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
        Hide();
    }

    private void Initialize()
    {
        _gameOverText.text = LocalizedText.GameOver;
        _retryText.text = LocalizedText.Retry;
        _homeText.text = LocalizedText.Home;

        _playerInput = FindObjectOfType<PlayerInput>();
        _playerInput?.SwitchCurrentActionMap("UI");
    }

    public void Show(UnityAction retryBtnClickedAction, UnityAction homeBtnClickedAction)
    {
        if (retryBtnClickedAction != null)
        {
            _retryButton.onClick.AddListener(retryBtnClickedAction);
        }

        if(homeBtnClickedAction != null)
        {
            _homeButton.onClick.AddListener(homeBtnClickedAction);
        }

        ShowGameOver();
    }

    public void Hide()
    {
        _retryButton.onClick.RemoveAllListeners();
        _homeButton.onClick.RemoveAllListeners();

        _gameOverObject.SetActive(false);
        _menu.SetActive(false);
        gameObject.SetActive(false);
    }

    private void ShowGameOver()
    {
        gameObject.SetActive(true);
        _gameOverObject.SetActive(true);
        StartCoroutine(WaitAndShowMenu(1.0f));
    }

    private IEnumerator WaitAndShowMenu(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        ShowMenu();
    }

    private void ShowMenu()
    {
        _menu.SetActive(true);
        EventSystem.current.SetSelectedGameObject(_retryButton.gameObject);
    }
}
