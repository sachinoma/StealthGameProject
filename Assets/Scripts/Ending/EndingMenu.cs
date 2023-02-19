using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndingMenu : MonoBehaviour
{
    [Header("ボタン - Home")]
    [SerializeField] private Button _homeButton;

    [Header("テキスト - Home")]
    [SerializeField] private TextMeshProUGUI _skipText;

    [Header("Animator")]
    [SerializeField] private Animator _animator;

    [Header("サウンド")]
    [SerializeField] private AudioSource bgm;

    void Start()
    {
        _homeButton.onClick.AddListener(OnHomeBtnClicked);
    }

    private void OnDestroy()
    {
        _homeButton.onClick.RemoveListener(OnHomeBtnClicked);
    }

    #region ボタンイベント

    private void OnHomeBtnClicked()
    {
        GameProgress.Reset();
        SceneControl.ChangeScene(SceneControl.TitleSceneName);
    }

    #endregion

    public void Initialize()
    {
        InitMainMenu();     
    }

    private void OnMenuShowingAnimEvent()
    {
        EventSystem.current?.SetSelectedGameObject(_homeButton.gameObject);
        bgm.Play();
    }

 

    #region メインメニュー

    private void InitMainMenu()
    {
        _skipText.text = LocalizedText.Home;
        //EventSystem.current.SetSelectedGameObject(_homeButton.gameObject);
    }

    #endregion

    #region Animator

    public void TextAnimStart()
    {
        _animator.SetBool("Clear", true);
    }

    #endregion
}
