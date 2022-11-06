using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Debug = UnityEngine.Debug;

public class EnemySarchPosition : MonoBehaviour
{
    private void OnDrawGizmos()
    {

        Handles.color = new Color(1f, 1f, 1f, 0.5f);
        Handles.DrawSolidArc(transform.position, Vector3.up,transform.forward, 360f, 1f);
    }
}
