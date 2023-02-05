using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(Collider))]
public class EnemyAI : MonoBehaviour
{
    [Header("敵AIのベース")]
    [SerializeField] private EnemyModel _model;
    [SerializeField] private NavMeshAgent _agent;
    private PlayerModel _player;
    private EnemyStateAIBase _currentStateAI;

    [Header("徘徊場所")]
    [SerializeField] private List<Transform> _searchPosList;
    private int _currentSearchPosIndex = -1;

    [Header("視界")]
    [SerializeField] private float _searchAngle = 100f;
    [SerializeField] private SphereCollider _searchArea;

    private void Start()
    {
        _player = FindObjectOfType<PlayerModel>();

        ChangeState(EnemyState.Idle);
    }

    private void Update()
    {
        if(_model.CanControl)
        {
            _currentStateAI.OnUpdate();
        }
    }

    public void ChangeState(EnemyState newState, object additionalInfo = null)
    {
        if(_currentStateAI != null && _currentStateAI.State == newState)
        {
            return;
        }

        _currentStateAI?.OnExit();  // 初期化の時、_currentStateAI == nullのこともある
        _currentStateAI = CreateStateAI(newState);
        _currentStateAI.OnEnter(additionalInfo);
    }

    private EnemyStateAIBase CreateStateAI(EnemyState state)
    {
        EnemyStateAIInitInfo initInfo = new EnemyStateAIInitInfo
        {
            ai = this,
            model = _model,
            agent = _agent,
            player = _player
        };

        switch(state)
        {
            case EnemyState.Idle:       return new EnemyIdleStateAI(initInfo);
            case EnemyState.Move:       return new EnemyMoveStateAI(initInfo);
            case EnemyState.Chase:      return new EnemyChaseStateAI(initInfo);
            case EnemyState.SoundSarch: return new EnemySoundSearchStateAI(initInfo);
            case EnemyState.Attack:     return new EnemyAttackStateAI(initInfo);
        }

        Debug.LogError($"まだEnemyStateAIBaseを配置していない：EnemyState : {state}");
        return null;
    }

    public Transform GetNextSearchPos()
    {
        if(_searchPosList == null || _searchPosList.Count == 0)
        {
            Debug.LogError("まだ_searchPosListに指定していない");
            return null;
        }

        ++_currentSearchPosIndex;
        if(_currentSearchPosIndex >= _searchPosList.Count)
        {
            _currentSearchPosIndex = 0;
        }

        return _searchPosList[_currentSearchPosIndex];
    }

    public float GetAgentVelocity()
    {
        return _agent.velocity.magnitude;
    }

    public bool CheckIsReachDestination(float allowDistance)
    {
        return !_agent.pathPending && _agent.remainingDistance <= allowDistance;
    }

    private void OnTriggerStay(Collider other)
    {
        if(!_player.CanBeDetected())
        {
            return;
        }

        if(_currentStateAI.CanDetectPlayer)
        {
            if(DetectPlayer(other))
            {
                _currentStateAI.PlayerDetected();
                return;
            }
        }

        if(_currentStateAI.CanDetectSound)
        {
            Transform target = DetectSound(other);
            if(target != null)
            {
                _currentStateAI.SoundDetected(target);
                return;
            }
        }

    }

    private bool DetectPlayer(Collider collider)
    {
        if(collider.CompareTag(Tag.Player))
        {
            var positionDiff = collider.transform.position - transform.position;  // 自身（敵）とプレイヤーの距離
            var angle = Vector3.Angle(transform.forward, positionDiff);  // 敵から見たプレイヤーの方向
            if(angle <= _searchAngle)//視界の中にいたら
            {
                float distance = 10f;    // Rayを飛ばす距離
                Vector3 direction = positionDiff.normalized;   // Rayを飛ばす方向

                Ray ray = new Ray(transform.position, direction);  // Rayを飛ばす
                Debug.DrawRay(ray.origin, ray.direction * distance, Color.black);  // Rayをシーン上に描画
                RaycastHit hit;
                if(Physics.Raycast(ray.origin, ray.direction, out hit, distance, Layer.EnemySight))
                {
                    if(hit.transform.root.CompareTag(Tag.Player))
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    private Transform DetectSound(Collider collider)
    {
        if(collider.CompareTag(Tag.Sounds))
        {
            return collider.transform;
        }

        return null;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        EnemyState currentState = _currentStateAI == null ? EnemyState.Idle : _currentStateAI.State;
        if(currentState == EnemyState.Chase || currentState == EnemyState.Attack)
        {
            Handles.color = new Color(1f, 0f, 0f, 0.3f);
        }
        else if(currentState == EnemyState.SoundSarch)
        {
            Handles.color = new Color(0f, 0f, 1f, 0.3f);
        }
        else
        {
            Handles.color = new Color(0f, 1f, 0f, 0.3f);
        }
        Handles.DrawSolidArc(transform.position, Vector3.up, Quaternion.Euler(0f, -_searchAngle, 0f) * transform.forward, _searchAngle * 2f, _searchArea.radius * 1.0f);
    }
#endif
}
