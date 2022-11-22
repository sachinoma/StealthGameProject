using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyReactableDummy : MonoBehaviour, IReactable
{
    public ReactableType GetReactableType()
    {
        return ReactableType.Key;
    }

    public bool React()
    {
        Destroy(gameObject);
        return false;
    }
}
