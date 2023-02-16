using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainSceneUIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _messageText;

    [Header("取得したアイテム")]
    [SerializeField] private Transform _cardIconBaseTransform;
    [SerializeField] private CardIcon _cardIconTemplate;

    [Header("メッセージ")]
    [SerializeField] private float _messageDisplayTime = 3.0f;

    [Header("ヒント")]
    [SerializeField] private Animator _hintAnimator;
    [SerializeField] private TextMeshProUGUI _hintText;
    [SerializeField] private float _hintDisplayTime = 3.0f;
    private const string IsShowHintAnimBool = "isShow";

    private List<CardIcon> _cardIcons;
    
    private Coroutine _showMessageCoroutine = null;
    private Coroutine _showHintCoroutine = null;

    private void Awake()
    {
        GenerateCardIcons();
    }

    #region アイテム

    public void ShowItemGotMessage()
    {
        if(_showMessageCoroutine != null)
        {
            StopCoroutine(_showMessageCoroutine);
        }

        _showMessageCoroutine = StartCoroutine(ShowMessage(LocalizedText.ItemGot));
    }

    private void GenerateCardIcons()
    {
        _cardIcons = new List<CardIcon>();

        CardType[] cardTypes = { CardType.White, CardType.Red, CardType.Blue, CardType.Yellow };
        Color[] cardColors = { Color.white, Color.red, Color.blue, Color.yellow };

        for(int i = 0; i < cardTypes.Length; ++i)
        {
            CardIcon cardIcon = Instantiate(_cardIconTemplate, _cardIconBaseTransform);
            cardIcon.Init(cardTypes[i], cardColors[i]);
            _cardIcons.Add(cardIcon);
        }
    }


    public void AddObtainedItem(CardType type)
    {
        foreach(CardIcon cardIcon in _cardIcons)
        {
            if(cardIcon.Type == type)
            {
                cardIcon.SetObtained();
                return;
            }
        }

        Debug.LogError($"CardType：{type} のCardIconはまだ配置していない。");
    }

    public void AddObtainedItems(List<CardType> types)
    {
        if(types == null)
        {
            return;
        }

        foreach(CardType type in types)
        {
            AddObtainedItem(type);
        }
    }

    public void SetItemUsed(CardType type)
    {
        foreach(CardIcon cardIcon in _cardIcons)
        {
            if(cardIcon.Type == type)
            {
                cardIcon.SetUsed();
                return;
            }
        }

        Debug.LogError($"CardType：{type} のCardIconはまだ配置していない。");
    }

    private IEnumerator ShowMessage(string message)
    {
        _messageText.gameObject.SetActive(true);
        _messageText.text = message;

        yield return new WaitForSeconds(_messageDisplayTime);

        _messageText.gameObject.SetActive(false);
    }

    #endregion

    #region ヒント

    public void ShowHint(HintType hintType)
    {
        string hint = "";
        switch(hintType)
        {
            case HintType.Trap:
                hint = LocalizedText.TrapHint;
                break;
            case HintType.OperationTerminal:
                hint = LocalizedText.OperationTerminalHint;
                break;
            case HintType.Card:
                hint = LocalizedText.CardHint;
                break;
            case HintType.Enemy:
                hint = LocalizedText.EnemyHint;
                break;
            default:
                Debug.LogWarning("hintTypeによってのヒントまだ実装していない。");
                return;
        }

        if(_showHintCoroutine != null)
        {
            StopCoroutine(_showHintCoroutine);
        }

        _showHintCoroutine = StartCoroutine(ShowHintCoroutine(hint));
    }

    private IEnumerator ShowHintCoroutine(string hint)
    {
        _hintText.text = hint;
        _hintAnimator.SetBool(IsShowHintAnimBool, true);

        yield return new WaitForSeconds(_hintDisplayTime);

        _hintAnimator.SetBool(IsShowHintAnimBool, false);
    }

    #endregion

}
