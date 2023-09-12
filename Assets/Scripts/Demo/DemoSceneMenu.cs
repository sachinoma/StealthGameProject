using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DemoSceneMenu : MonoBehaviour
{
    [Header("ボタン - メニュー")]
    [SerializeField] private Button _continueButton;

    [Header("テキスト - メニュー")]
    [SerializeField] private TextMeshProUGUI _continueText;

    void Start()
    {
        InitMenu();

        _continueButton.onClick.AddListener(OnContinueBtnClicked);
    }

    private void OnDestroy()
    {
        _continueButton.onClick.RemoveListener(OnContinueBtnClicked);
    }

    #region ボタンイベント

    private void OnContinueBtnClicked()
    {
        SceneControl.ChangeScene(SceneControl.TitleSceneName);
    }

    #endregion

    public void InitMenu()
    {
        _continueText.text = LocalizedText.PressAToContinue;
        EventSystem.current.SetSelectedGameObject(_continueButton.gameObject);
    }
}
