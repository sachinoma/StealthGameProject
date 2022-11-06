using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class EnemyAI : MonoBehaviour
{
    [SerializeField] private TriggerEvent onTriggerStay = new TriggerEvent();
    [SerializeField] private TriggerEvent onTriggerExit = new TriggerEvent();
    [SerializeField] private TriggerEvent onTriggerEnter = new TriggerEvent();

    /// <param name="other"></param>
    private void OnTriggerStay(Collider other)
    {
        // Inspector�^�u��onTriggerStay�Ŏw�肳�ꂽ���������s����
        onTriggerStay.Invoke(other);
    }

    private void OnTriggerExit(Collider other)
    {
        // Inspector�^�u��onTriggerStay�Ŏw�肳�ꂽ���������s����
        onTriggerExit.Invoke(other);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Inspector�^�u��onTriggerStay�Ŏw�肳�ꂽ���������s����
        onTriggerEnter.Invoke(other);
    }

    // UnityEvent���p�������N���X��[Serializable]������t�^���邱�ƂŁAInspector�E�C���h�E��ɕ\���ł���悤�ɂȂ�B
    [Serializable]
    public class TriggerEvent : UnityEvent<Collider>
    {
    }
}
