using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ReactableDetector : MonoBehaviour
{
    private List<ReactableBase> _reactableList = new List<ReactableBase>();

    private ReactableBase GetReactable()
    {
        if (_reactableList.Count == 0)
        {
            return null;
        }

        return _reactableList[0];
    }

    public void ReactWithReactable(Action<ReactableBase> action = null)
    {
        ReactableBase reactable = GetReactable();
        action?.Invoke(reactable);

        if(reactable != null)
        {
            _reactableList.Remove(reactable);
        }
    }

    #region Trigger
    private void OnTriggerEnter(Collider other)
    {
        ReactableBase reactable = other.GetComponent<ReactableBase>();
        if(reactable != null)
        {
            if(!_reactableList.Contains(reactable))
            {
                _reactableList.Add(reactable);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        ReactableBase reactable = other.GetComponent<ReactableBase>();
        if(reactable != null)
        {
            if(_reactableList.Contains(reactable))
            {
                _reactableList.Remove(reactable);
            }
        }
    }
    #endregion
}
