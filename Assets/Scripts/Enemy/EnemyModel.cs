using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.AI;
using System.Security.Cryptography;
using System;
using Debug = UnityEngine.Debug;
using UniRandom = UnityEngine.Random;
using System.Diagnostics;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyModel : MonoBehaviour
{
    enum State
    {
        Free,
        PatrolPoint1,
        PatrolPoint2
    }
    State currentState = State.Free;
    bool stateEnter = true;
    bool isChase;
    bool isSarch;

    int chaseTimer;

    [SerializeField] Transform point1;
    [SerializeField] Transform point2;
    [SerializeField] Transform enemySarch;
    [SerializeField] float searchAngle = 100f;
    private NavMeshAgent _agent;
    [SerializeField] private SphereCollider searchArea;
    public PlayerModel script;

    void Start()
    {
        isChase = false;
        isSarch = false;
        chaseTimer = 0;
        _agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if(isChase)
        {
            if (_agent.remainingDistance <= 1f && !_agent.pathPending)
            {
                GetComponent<Renderer>().material.color = Color.red;
                Attack();
            }
            else
            {
                GetComponent<Renderer>().material.color = Color.white;
            }
            return;
        }
        else
        {
            if(chaseTimer > 0)
            {
                chaseTimer--;
                return;
            }
            if(isSarch)
            {
                if(_agent.remainingDistance <= 0.1f && !_agent.pathPending)
                {
                    isSarch = false;
                    currentState = State.Free;
                }
                return;
            }
        }
        switch (currentState)
        {
            case State.Free:
                #region
                if (stateEnter)//開始1回の処理
                {
                    stateEnter = false;
                }
                bool rnd = RandomBool();
                currentState =  rnd ? State.PatrolPoint1: State.PatrolPoint2;
                #endregion
                break;
            case State.PatrolPoint1:
                #region
                if (stateEnter)//開始1回の処理
                {
                    stateEnter = false;
                }
                _agent.destination = point1.position;
                enemySarch.position = point1.position;
                if (_agent.remainingDistance <= 0.1f && !_agent.pathPending)
                {
                    currentState = State.PatrolPoint2;
                    return;
                }
                #endregion
                break;
            case State.PatrolPoint2:
                #region
                if (stateEnter)//開始1回の処理
                {
                    stateEnter = false;
                }
                _agent.destination = point2.position;
                enemySarch.position = point2.position;
                if (_agent.remainingDistance <= 0.1f && !_agent.pathPending)
                {
                    currentState = State.PatrolPoint1;
                    return;
                }
                #endregion
                break;
        }

    }

    public void OnDetectObjectStay(Collider collider)
    {
        // 検知オブジェクトがPlayerなら追いかける
        if(collider.CompareTag(Tag.Player))
        {
            var positionDiff = collider.transform.position - transform.position;  // 自身（敵）とプレイヤーの距離
            var angle = Vector3.Angle(transform.forward, positionDiff);  // 敵から見たプレイヤーの方向
            if (angle <= searchAngle)//視界の中にいたら
            {
                RaycastHit hit;
                Vector3 direction;   // Rayを飛ばす方向
                float distance = 10;    // Rayを飛ばす距離

                Vector3 temp = collider.transform.position - transform.position;
                direction = temp.normalized;

                Ray ray = new Ray(transform.position, direction);  // Rayを飛ばす
                Debug.DrawRay(ray.origin, ray.direction * distance, Color.black);  // Rayをシーン上に描画
                if (Physics.Raycast(ray.origin, ray.direction * distance, out hit))
                {
                    if (hit.collider.CompareTag(Tag.Player))
                    {
                        isChase = true;
                        isSarch = false;
                        _agent.destination = collider.transform.position;
                        enemySarch.position = collider.transform.position;
                    }
                    else if(isChase)
                    {
                        isChase = false;
                        chaseTimer = 180;
                    }
                }
            }
        }
    }

    public void OnDetectObjectExit(Collider collider)
    {
        if (collider.CompareTag(Tag.Player))
        {
            isChase = false;
            chaseTimer = 180;
        }
    }

    public void OnDetectObjectEnter(Collider collider)
    {
        if (collider.CompareTag(Tag.Sounds))
        {
            isSarch = true;
            _agent.destination = collider.transform.position;
            enemySarch.position = collider.transform.position;
        }
    }

    private void OnDrawGizmos()
    {
        if(isChase)
        {
            Handles.color = new Color(1f, 0f, 0f, 0.3f);
        }
        else if(isSarch)
        {
            Handles.color = new Color(0f, 0f, 1f, 0.3f);
        }
        else
        {
            Handles.color = new Color(0f, 1f, 0f, 0.3f);
        }
        Handles.DrawSolidArc(transform.position, Vector3.up, Quaternion.Euler(0f, -searchAngle, 0f) * transform.forward, searchAngle * 2f, searchArea.radius);
    }

    bool RandomBool()
    {
        return UniRandom.Range(0, 2) == 0;
    }

    void Attack()
    {
        //PlayerModel.csをアタッチしたら関数を呼び出す
        //script.Damage(5f);
    }
}
