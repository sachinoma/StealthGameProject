using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BillboardUI : MonoBehaviour
{
    [SerializeField] private RectTransform _rect;
    [SerializeField] private Image _image;

    private Camera _camera;
    private Vector3 _worldPos;

    private void Start()
    {
        _camera = Camera.main;
        if (_camera == null) {
            Debug.LogWarning("Main Mameraが見つけられない。BillboardUIは表示できない。");
            gameObject.SetActive(false);
        }
    }

    public void SetWorldPos(Vector3 worldPos)
    {
        _worldPos = worldPos;
    }

    private void LateUpdate()
    {
        Vector3 screenPos = _camera.WorldToScreenPoint(_worldPos);
        _rect.position = screenPos;
        _image.enabled = screenPos.z > 0;  // _targetはカメラの後ろにある場合、表示しない
    }
}
