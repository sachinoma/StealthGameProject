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
        // ���m�I�u�W�F�N�g��Player�Ȃ�ǂ�������
        if(collider.CompareTag("Player"))
        {
            var positionDiff = collider.transform.position - transform.position;  // ���g�i�G�j�ƃv���C���[�̋���
            var angle = Vector3.Angle(transform.forward, positionDiff);  // �G���猩���v���C���[�̕���
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
