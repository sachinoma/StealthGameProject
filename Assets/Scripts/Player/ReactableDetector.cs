using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ReactableDetector : MonoBehaviour
{
    private List<ReactableBase> _reactableList = new List<ReactableBase>();

    public ReactableBase GetReactable()
    {
        // 削除したものを残らないように
        while(_reactableList.Count > 0)
        {
            if(_reactableList[0] == null)
            {
                _reactableList.RemoveAt(0);
            }
            else
            {
                return _reactableList[0];
            }
        }

        return null;
    }

    public void PopReactable()
    {
        ReactableBase reactable = GetReactable();
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
                print($"Reactableを検出した：{reactable.gameObject.name}");
                _reactableList.Add(reactable);
                reactable.SetInReactableRange(true);
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
                print($"Reactableから離した：{reactable.gameObject.name}");
                _reactableList.Remove(reactable);
                reactable.SetInReactableRange(false);
            }
        }
    }
    #endregion
}
