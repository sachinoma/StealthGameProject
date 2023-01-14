using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrototypeSceneUIManager : MonoBehaviour
{
    [SerializeField] private Text _lifeText;
    [SerializeField] private Text _keyText;
    [SerializeField] private Text _inputModeText;
    [SerializeField] private Text _messageText;

    private PlayerModel _playerModel;

    // Start is called before the first frame update
    void Start()
    {
        _playerModel = FindObjectOfType<PlayerModel>();
        if(_playerModel != null)
        {
            UpdateLifeDisplay();
            //UpdateKeyDisplay(false);

            //_playerModel.DamageTaken += UpdateLifeDisplay;
            //_playerModel.PickedUp += UpdateKeyDisplay;
        }

        PrototypeMessageEvent.MessageReceived += MessageReceivedHandler;

        _messageText.enabled = false;

        MicRange micRange = FindObjectOfType<MicRange>();
        if (micRange != null)
        {
            UpdateInputModeText(micRange.IsMicMode);
        }
    }

    private void OnDestroy()
    {
        if(_playerModel != null)
        {
            //_playerModel.DamageTaken -= UpdateLifeDisplay;
            //_playerModel.PickedUp -= UpdateKeyDisplay;
        }

        PrototypeMessageEvent.MessageReceived -= MessageReceivedHandler;
    }

    private void UpdateLifeDisplay()
    {
        _lifeText.text = $"生命 : {_playerModel.CurrentLife}";
    }

    //private void UpdateKeyDisplay(PickUpItem.ItemType itemType)
    //{
    //    if(itemType == PickUpItem.ItemType.Key)
    //    {
    //        UpdateKeyDisplay(true);
    //    }
    //}

    //private void UpdateKeyDisplay(bool hasKey)
    //{
    //    _keyText.text = "鍵 : " + (hasKey ? "O" : "X");
    //}

    private void MessageReceivedHandler(string message)
    {
        StopAllCoroutines();
        StartCoroutine(ShowMessage(message));
    }

    private IEnumerator ShowMessage(string message)
    {
        _messageText.enabled = true;
        _messageText.text = message;

        yield return new WaitForSeconds(3.0f);

        _messageText.enabled = false;
    }

    public void UpdateInputModeText(bool isMicMode)
    {
        _inputModeText.text = isMicMode ? "マイク入力" : "キー入力";
    }
}
