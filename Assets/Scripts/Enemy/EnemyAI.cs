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
        // InspectorタブのonTriggerStayで指定された処理を実行する
        onTriggerStay.Invoke(other);
    }

    private void OnTriggerExit(Collider other)
    {
        // InspectorタブのonTriggerStayで指定された処理を実行する
        onTriggerExit.Invoke(other);
    }

    private void OnTriggerEnter(Collider other)
    {
        // InspectorタブのonTriggerStayで指定された処理を実行する
        onTriggerEnter.Invoke(other);
    }

    // UnityEventを継承したクラスに[Serializable]属性を付与することで、Inspectorウインドウ上に表示できるようになる。
    [Serializable]
    public class TriggerEvent : UnityEvent<Collider>
    {
    }
}
