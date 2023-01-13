using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MainSceneEventTrigger))]
public class SimpleMapTigger : MonoBehaviour
{
    [SerializeField] private string _targetTag;
    [SerializeField] private bool _isOneOff;

    private MainSceneEventTrigger _trigger;

    private void Start()
    {
        TryGetComponent(out _trigger);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag(_targetTag))
        {
            _trigger?.Trigger();
            if(_isOneOff)
            {
                Destroy(gameObject);
            }
        }
    }
}
