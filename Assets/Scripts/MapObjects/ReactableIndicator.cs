using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactableIndicator : MonoBehaviour
{
    [SerializeField] private BillboardUI _template;
    [SerializeField] private Transform _refTransform;
    [SerializeField] private bool _isUseInitialPos;

    private Transform _billboardUIBase;
    private BillboardUI _billboardUI;
    private Vector3 _initialPos;

    private void Start()
    {
        GameObject billboardUIBaseObject = GameObject.FindGameObjectWithTag(Tag.BillboardUIBase);
        if (billboardUIBaseObject == null) {
            Debug.LogWarning("Tag : BillboardUIBaseのオブジェクトが見つけられない。");
        } else {
            _billboardUIBase = billboardUIBaseObject.transform;
        }

        if (_isUseInitialPos) {
            _initialPos = _refTransform.position;
        }
    }

    private void OnDestroy()
    {
        Hide();
    }

    public void Show()
    {
        if (_billboardUI == null) {
            _billboardUI = Instantiate<BillboardUI>(_template, _billboardUIBase);

            Vector3 targetWorldPos = _isUseInitialPos ? _initialPos : _refTransform.position;
            _billboardUI.SetWorldPos(targetWorldPos);
        }
    }

    public void Hide()
    {
        if (_billboardUI != null) {
            Destroy(_billboardUI.gameObject);
        }
    }
}