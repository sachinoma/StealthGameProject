using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinSceneDirector : MonoBehaviour
{
    [SerializeField] private Transform _objectsBaseTransform;

    private int _currentObjectIndex = -1;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(MarkObjectCoroutine());
    }

    private IEnumerator MarkObjectCoroutine()
    {
        while(true)
        {
            yield return new WaitForSeconds(1.0f);
            MarkObject();
        }
    }

    private void MarkObject()
    {
        ++_currentObjectIndex;
        if (_currentObjectIndex >= _objectsBaseTransform.childCount)
        {
            _currentObjectIndex = 0;
        }

        MarkerManager.Instance?.SetMarker(_objectsBaseTransform.GetChild(_currentObjectIndex));
    }
}
