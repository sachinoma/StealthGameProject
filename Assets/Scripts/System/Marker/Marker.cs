using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Marker : MonoBehaviour
{
    [SerializeField] private RectTransform _rect;
    [SerializeField] private Transform _target;
    [SerializeField] private bool _isOn = true;

    private void OnValidate()
    {
        SetIsOn(_isOn);
        if(_isOn)
        {
            StickToTarget();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!_isOn)
        {
            return;
        }

        if (_target == null)
        {
            SetIsOn(false);
            return;
        }

        StickToTarget();
    }

    public void SetTarget(Transform target)
    {
        if(target == null)
        {
            _target = null;
            SetIsOn(false);
        }
        else
        {
            _target = target;
            SetIsOn(true);
        }
    }

    private void SetIsOn(bool isOn)
    {
        _isOn = isOn;
        gameObject.SetActive(isOn);
    }

    private void StickToTarget()
    {
        if (_target == null)
        {
            return;
        }

        Vector3 screenPos = Camera.main.WorldToScreenPoint(_target.position);
        _rect.position = screenPos;
    }
}
