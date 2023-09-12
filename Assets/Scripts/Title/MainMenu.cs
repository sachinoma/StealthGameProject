using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
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
    [SerializeField] private Button _gameStartButton;
    [SerializeField] private Button _creditButton;
    [SerializeField] private Button _quitButton;

    [Header("テキスト - メインメニュー")]
    [SerializeField] private TextMeshProUGUI _gameStartText;
    [SerializeField] private TextMeshProUGUI _creditText;
    [SerializeField] private TextMeshProUGUI _quitText;


    [Header("クレジットパネル")]
    [SerializeField] private GameObject _creditPanel;
    [SerializeField] private CreditTitleName _creditTitleNameTemplate;
    [SerializeField] private Transform _creditDetailsBase;

    [Header("ボタン - クレジットパネル")]
    [SerializeField] private Button _exitCreditButton;

    [Header("テキスト - クレジットパネル")]
    [SerializeField] private TextMeshProUGUI _exitCreditText;

    private View _currentView = View.None;

    public event Action<View> ViewChanged = null;

    // Start is called before the first frame update
    void Start()
    {
        _gameStartButton.onClick.AddListener(OnGameStartBtnClicked);
        _creditButton.onClick.AddListener(OnCreditBtnClicked);
        _quitButton.onClick.AddListener(OnQuitBtnClicked);
        _exitCreditButton.onClick.AddListener(OnExitCreditBtnClicked);
    }

    private void OnDestroy()
    {
        _gameStartButton.onClick.RemoveListener(OnGameStartBtnClicked);
        _creditButton.onClick.RemoveListener(OnCreditBtnClicked);
        _quitButton.onClick.RemoveListener(OnQuitBtnClicked);
        _exitCreditButton.onClick.RemoveListener(OnExitCreditBtnClicked);
    }

    #region ボタンイベント

    private void OnGameStartBtnClicked()
    {
        GameProgress.Reset();
        SceneControl.ChangeScene(SceneControl.IntroSceneName);
    }

    private void OnCreditBtnClicked()
    {
        ChangeView(View.Credit);
    }

    private void OnQuitBtnClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void OnExitCreditBtnClicked()
    {
        ChangeView(View.MainMenu);
    }

    #endregion

    public void Initialize()
    {
        InitMainMenu();
        InitCrerditPanel();
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
        SetCreditPanelActive(_currentView == View.Credit);

        ViewChanged?.Invoke(_currentView);
    }

    #region メインメニュー

    private void InitMainMenu()
    {
        _gameStartText.text = LocalizedText.GameStart;
        _creditText.text = LocalizedText.Credit;
        _quitText.text = LocalizedText.Quit;
    }

    private void SetMainMenuActive(bool isActive)
    {
        _mainMenu.SetActive(isActive);

        if(isActive)
        {
            EventSystem.current.SetSelectedGameObject(_gameStartButton.gameObject);
        }
    }

    #endregion

    #region クレジットパネル

    private void InitCrerditPanel()
    {
        _exitCreditText.text = LocalizedText.ExitCredit;

        string[] titleNameArray = LocalizedText.CreditDetails.Split('\n');
        if(titleNameArray == null || titleNameArray.Length == 0)
        {
            Debug.LogWarning("titleNameArrayは何もない。");
            return;
        }

        foreach(string titleName in titleNameArray)
        {
            string[] split = titleName.Split('|');
            if(split == null || split.Length != 2)
            {
                Debug.LogWarning("titleNameの仕組みは合っていない。");
                return;
            }

            CreditTitleName instance = Instantiate(_creditTitleNameTemplate, _creditDetailsBase);
            instance.SetDetails(split[0], split[1]);
        }
    }

    private void SetCreditPanelActive(bool isActive)
    {
        _creditPanel.SetActive(isActive);

        if(isActive)
        {
            EventSystem.current.SetSelectedGameObject(_exitCreditButton.gameObject);
        }
    }

    #endregion

}
