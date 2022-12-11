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
    float _speed;

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
    bool isAttack;

    int chaseTimer;

    private Animator _animator;

    BoxCollider leftCollider;
    BoxCollider rightCollider;

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
        isAttack = false;
        chaseTimer = 0;
        _agent = GetComponent<NavMeshAgent>();
        GameObject obj = GameObject.Find("robot_enemy");
        _animator = obj.GetComponent<Animator>();
        leftCollider = GameObject.Find("forearm_L.002").GetComponent<BoxCollider>();
        rightCollider = GameObject.Find("forearm_R.002").GetComponent<BoxCollider>();
        _speed = _agent.speed;
    }

    void Update()
    {
        _animator.SetFloat("Speed", _agent.speed);
        if(isChase)
        {
            _animator.SetBool("isChase",true);
            if(_agent.remainingDistance <= 1.5f && !_agent.pathPending && !isAttack)
            {
                Attack();
            }
            return;
        }
        else
        {
            if (chaseTimer > 0)
            {
                chaseTimer--;
                return;
            }
            _animator.SetBool("isChase", false);
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
                stateEnter = true;
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
                    stateEnter = true;
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
                    stateEnter = true;
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
                if (Physics.Raycast(ray.origin, ray.direction, out hit, distance, Layer.EnemySight))
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
        isAttack = true;
        Invoke("ColliderStart", 0.8f);
        _animator.SetTrigger("Attack Trigger");
        Invoke("ColliderReset", 1.12f);
        Invoke("AttackEnd", 2.15f);
    }

    private void AttackEnd()
    {
        isAttack = false;
        _agent.speed = _speed;
    }

    private void ColliderStart()
    {
        rightCollider.enabled = true;
        leftCollider.enabled = true;
    }

    private void ColliderReset()
    {
        rightCollider.enabled = false;
        leftCollider.enabled = false;
        _agent.speed = 0f;
    }
}
