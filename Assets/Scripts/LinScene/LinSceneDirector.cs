using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinSceneDirector : MonoBehaviour
{
    [SerializeField] private Transform _objectsBaseTransform;

    private bool _isMarkerOn = true;

    // Start is called before the first frame update
    void Start()
    {
        MarkObjects(_isMarkerOn);
        SetMiniMap();
    }

    private void MarkObjects(bool isMarkerOn)
    {
        foreach(Transform child in _objectsBaseTransform)
        {
            if(isMarkerOn)
            {
                MarkerManager.Instance?.SetMarker(child);
            }
            else
            {
                MarkerManager.Instance?.CancelMarker(child);
            }

        }
    }

    public void SwitchMarkerStatus()
    {
        _isMarkerOn = !_isMarkerOn;
        MarkObjects(_isMarkerOn);
    }

    private void SetMiniMap()
    {
        PlayerModel player = FindObjectOfType<PlayerModel>();
        if(player != null)
        {
            MiniMapManager.Instance?.SetFocus(player.transform);
        }
    }
}
