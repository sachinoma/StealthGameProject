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
    [Header("ボタン - メインメニュー")]
    [SerializeField] private Button _skipButton;

    [Header("テキスト - メインメニュー")]
    [SerializeField] private TextMeshProUGUI _skipText;

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
    }

    #region メインメニュー

    private void InitMainMenu()
    {
        _skipText.text = LocalizedText.Skip;
        EventSystem.current.SetSelectedGameObject(_skipButton.gameObject);
    }

    #endregion

}
