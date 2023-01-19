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
    [SerializeField] private GameObject _cardIconTemplate;

    [Header("メッセージ")]
    [SerializeField] private float _messageDisplayTime = 3.0f;

    private Dictionary<CardType, Image> _displayingIcons = new Dictionary<CardType, Image>();
    
    private Coroutine _showMessageCoroutine = null;

    public void ShowItemGotMessage()
    {
        if(_showMessageCoroutine != null)
        {
            StopCoroutine(_showMessageCoroutine);
        }

        _showMessageCoroutine = StartCoroutine(ShowMessage(LocalizedText.ItemGot));
    }

    public void AddObtainedItem(CardType type)
    {
        if(_displayingIcons.ContainsKey(type))
        {
            Debug.LogWarning("すでにアイコンを追加した。");
            return;
        }

        Color cardColor = Color.white;
        switch(type)
        {
            case CardType.White:    cardColor = Color.white;    break;
            case CardType.Red:      cardColor = Color.red;      break;
            case CardType.Blue:     cardColor = Color.blue;     break;
            case CardType.Yellow:   cardColor = Color.yellow;   break;
            default:
                Debug.LogError($"まだ実装していない。CardType：{type}");
                return;
        }

        GameObject icon = Instantiate(_cardIconTemplate, _cardIconBaseTransform);
        Image iconImage = icon.GetComponent<Image>();
        if(iconImage != null)
        {
            iconImage.color = cardColor;
        }

        _displayingIcons.Add(type, iconImage);
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
        if(_displayingIcons.ContainsKey(type))
        {
            Color newColor = _displayingIcons[type].color;
            newColor.a = 0.3f;
            _displayingIcons[type].color = newColor;
        }
        else
        {
            Debug.LogWarning("アイコンはまだ追加していない。");
        }
    }

    private IEnumerator ShowMessage(string message)
    {
        _messageText.gameObject.SetActive(true);
        _messageText.text = message;

        yield return new WaitForSeconds(_messageDisplayTime);

        _messageText.gameObject.SetActive(false);
    }
}
