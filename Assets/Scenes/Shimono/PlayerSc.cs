using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Threading;
using UnityEngine;
using UnityEditor;
using UnityEngine.InputSystem;

public class PlayerSc : MonoBehaviour
{
    [SerializeField] private SphereCollider col;
    public Transform cameratrans;

    void Update()
    {
        Transform trans = transform;
        transform.position = trans.position;

        col.enabled = false;
        col.radius = 0f;
        if (Keyboard.current.wKey.IsPressed())
        {
            trans.position += trans.TransformDirection(Vector3.forward) * 1 * 0.1f;
            col.enabled = true;
            col.radius = 10f;
        }
        else if (Keyboard.current.aKey.IsPressed())
        {
            trans.position += trans.TransformDirection(Vector3.right) * -1 * 0.1f;
            col.enabled = true;
            col.radius = 10f;
        }
        else if (Keyboard.current.sKey.IsPressed())
        {
            trans.position += trans.TransformDirection(Vector3.forward) * -1 * 0.1f;
            col.enabled = true;
            col.radius = 10f;
        }
        else if (Keyboard.current.dKey.IsPressed())
        {
            trans.position += trans.TransformDirection(Vector3.right) * 1 * 0.1f;
            col.enabled = true;
            col.radius = 10f;
        }
        if (Keyboard.current.qKey.IsPressed())
        {
            col.enabled = false;
            col.radius = 0f;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Handles.color = new Color(1f, 1f, 0f, 0.3f);
        Handles.DrawSolidArc(transform.position, Vector3.up, transform.forward, 360f, col.radius);
    }
#endif
}