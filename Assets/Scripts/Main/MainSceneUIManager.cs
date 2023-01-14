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

    private Coroutine _showMessageCoroutine = null;

    public void ShowItemGotMessage()
    {
        if(_showMessageCoroutine != null)
        {
            StopCoroutine(_showMessageCoroutine);
        }

        _showMessageCoroutine = StartCoroutine(ShowMessage(LocalizedText.ItemGot));
    }

    public void AddObtainedItem(PickUpItem.ItemType type)
    {
        Color cardColor = Color.white;
        switch(type)
        {
            case PickUpItem.ItemType.Card_White:    cardColor = Color.white;    break;
            case PickUpItem.ItemType.Card_Red:      cardColor = Color.red;      break;
            case PickUpItem.ItemType.Card_Blue:     cardColor = Color.blue;     break;
            case PickUpItem.ItemType.Card_Yellow:   cardColor = Color.yellow;   break;
            default:
                Debug.LogError($"まだ実装していない。PickUpItem.ItemType：{type}");
                return;
        }

        GameObject icon = Instantiate(_cardIconTemplate, _cardIconBaseTransform);
        Image iconImage = icon.GetComponent<Image>();
        if(iconImage != null)
        {
            iconImage.color = cardColor;
        }
    }

    public void AddObtainedItems(List<PickUpItem.ItemType> types)
    {
        if(types == null)
        {
            return;
        }

        foreach(PickUpItem.ItemType type in types)
        {
            AddObtainedItem(type);
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
