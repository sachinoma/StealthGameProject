using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ReactableDetector : MonoBehaviour
{
    private List<IReactable> _reactableList = new List<IReactable>();

    private IReactable GetReactable()
    {
        if (_reactableList.Count == 0)
        {
            return null;
        }

        return _reactableList[0];
    }

    /// <returns>反応しているオブジェクトの <see cref="ReactableType"/></returns>
    public ReactableType TriggerReactable()
    {
        IReactable reactable = GetReactable();
        if(reactable == null)
        {
            return ReactableType.None;
        }

        ReactableType returnType = reactable.GetReactableType();
        bool canReactAgain = reactable.React();
        if(!canReactAgain)
        {
            _reactableList.Remove(reactable);
        }

        return returnType;
    }

    #region Trigger
    private void OnTriggerEnter(Collider other)
    {
        print(other);
        IReactable reactable = other.GetComponent<IReactable>();
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
        print("Exit : " + other);
        IReactable reactable = other.GetComponent<IReactable>();
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
