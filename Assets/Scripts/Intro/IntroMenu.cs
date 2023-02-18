using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IntroMenu : MonoBehaviour
{
    public enum View
    {
        None,
        MainMenu,
        Credit,
    }

    [Header("メインメニュー")]
    [SerializeField] private GameObject _mainMenu;

    [Header("ボタン - メインメニュー")]
    [SerializeField] private Button _skipButton;

    [Header("テキスト - メインメニュー")]
    [SerializeField] private TextMeshProUGUI _skipText;

    private View _currentView = View.None;

    public event Action<View> ViewChanged = null;

    // Start is called before the first frame update
    void Start()
    {
        _skipButton.onClick.AddListener(OnSkipBtnClicked);
    }

    private void OnDestroy()
    {
        _skipButton.onClick.RemoveListener(OnSkipBtnClicked);
    }

    #region ボタンイベント

    private void OnSkipBtnClicked()
    {
        GameProgress.Reset();
        SceneControl.ChangeScene(SceneControl.MainSceneName);
    }

    #endregion

    public void Initialize()
    {
        InitMainMenu();
        ChangeView(View.MainMenu);
    }

    private void ChangeView(View viewToChange)
    {
        if(_currentView == viewToChange)
        {
            return;
        }

        _currentView = viewToChange;

        SetMainMenuActive(_currentView == View.MainMenu);

        ViewChanged?.Invoke(_currentView);
    }

    #region メインメニュー

    private void InitMainMenu()
    {
        _skipText.text = LocalizedText.Skip;
    }

    private void SetMainMenuActive(bool isActive)
    {
        _mainMenu.SetActive(isActive);

        if(isActive)
        {
            EventSystem.current.SetSelectedGameObject(_skipButton.gameObject);
        }
    }

    #endregion

}
