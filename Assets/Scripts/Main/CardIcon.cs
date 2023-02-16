using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardIcon : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    private const string ObtainAnimTriggerName = "Obtain";
    private const string UseAnimTriggerName = "Use";

    public CardType Type { get; private set; }

    public void Init(CardType cardType, Color color)
    {
        Type = cardType;
        Image[] images = GetComponentsInChildren<Image>();
        if(images == null)
        {
            return;
        }

        foreach(Image image in images)
        {
            image.color = color;
        }
    }

    public void SetObtained()
    {
        _animator.SetTrigger(ObtainAnimTriggerName);
    }

    public void SetUsed()
    {
        _animator.SetTrigger(UseAnimTriggerName);
    }

}
