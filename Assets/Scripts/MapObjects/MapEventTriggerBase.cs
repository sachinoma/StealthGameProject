using UnityEngine;

public abstract class MapEventTriggerBase : MonoBehaviour
{
    [SerializeField] protected string _eventId;
    [SerializeField] private string _targetTag;
    [SerializeField] private bool _isOneOff;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag(_targetTag))
        {
            Trigger();
            if(_isOneOff)
            {
                Destroy(gameObject);
            }
        }
    }

    protected abstract void Trigger();
}
