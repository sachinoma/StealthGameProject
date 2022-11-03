using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinSceneDirector : MonoBehaviour
{
    [SerializeField] private Transform _objectsBaseTransform;

    // Start is called before the first frame update
    void Start()
    {
        MarkObjects();
        SetMiniMap();
    }

    private void MarkObjects()
    {
        foreach(Transform child in _objectsBaseTransform)
        {
            MarkerManager.Instance?.SetMarker(child);
        }
    }

    private void SetMiniMap()
    {
        LinSceneDummyPlayer player = FindObjectOfType<LinSceneDummyPlayer>();
        if(player != null)
        {
            MiniMapManager.Instance?.SetFocus(player.transform);
        }
    }
}
