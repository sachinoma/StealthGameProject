using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainSceneEventTrigger : MonoBehaviour
{
    [SerializeField] private string _eventId;

    public void Trigger()
    {
        MainSceneEventManager.TriggerEvent(_eventId, this, null);
    }
}
