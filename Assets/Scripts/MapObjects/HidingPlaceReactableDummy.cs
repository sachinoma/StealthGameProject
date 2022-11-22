using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HidingPlaceReactableDummy : MonoBehaviour, IReactable
{
    public ReactableType GetReactableType()
    {
        return ReactableType.HidingPlace;
    }

    public bool React()
    {
        return true;
    }
}