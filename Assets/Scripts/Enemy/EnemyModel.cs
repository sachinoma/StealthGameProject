using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyModel : MonoBehaviour
{
    [SerializeField] float searchAngle = 100f;
    private NavMeshAgent _agent;

    [SerializeField] private SphereCollider searchArea;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    public void OnDetectObject(Collider collider)
    {
        // 検知オブジェクトがPlayerなら追いかける
        if(collider.CompareTag("Player"))
        {
            var positionDiff = collider.transform.position - transform.position;  // 自身（敵）とプレイヤーの距離
            var angle = Vector3.Angle(transform.forward, positionDiff);  // 敵から見たプレイヤーの方向
            if (angle <= searchAngle)
            {
                _agent.destination = collider.transform.position;
            }
        }
        else if(collider.CompareTag("Sounds"))
        {
            _agent.destination = collider.transform.position;
        }
    }

    private void OnDrawGizmos()
    {
        Handles.color = new Color(0f, 1f, 0f, 0.3f);
        Handles.DrawSolidArc(transform.position, Vector3.up, Quaternion.Euler(0f, -searchAngle, 0f) * transform.forward, searchAngle * 2f, searchArea.radius);
    }
}
