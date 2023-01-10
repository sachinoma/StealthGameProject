using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Debug = UnityEngine.Debug;
using random = UnityEngine.Random;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyModel : MonoBehaviour
{
    [Header("ステート")]
     public EnemyState currentState;
    [HideInInspector] public bool stateEnter;

    [Header("徘徊場所")]
    [SerializeField] List<Transform> sarchPosition;

    [Header("敵のベース")]
    private NavMeshAgent _agent;
    public Animator animator;
    private float _speed;
    int chaseTimer;
    [HideInInspector] public int stopTimer;
    [SerializeField] private SphereCollider playerSounds;
    [SerializeField] Transform enemySarch;//今どこに向かっているか
    //視界
    [SerializeField] float searchAngle = 100f;
    [SerializeField] private SphereCollider searchArea;

    void Start()
    {
        AgentSetUp();
    }

    void Update()
    {
        animator.SetFloat("Speed", _agent.speed);

        #region 敵のステート
        switch (currentState)
        {
            case EnemyState.Idle:
                #region 開始1回の処理
                if (stateEnter)
                {
                    stateEnter = false;
                    _agent.speed = 0f;
                }
                #endregion

                if (stopTimer <= 0)
                {
                    currentState = EnemyState.Move;
                    _agent.speed = _speed;
                    stateEnter = true;
                }
                else
                {
                    stopTimer--;
                }
                break;
            case EnemyState.Move:
                #region 開始1回の処理
                if (stateEnter)
                {
                    stateEnter = false;
                    int rnd = random.Range(0, sarchPosition.Count);
                    _agent.destination = sarchPosition[rnd].position;
                    enemySarch.position = sarchPosition[rnd].position;
                }
                #endregion
                if (_agent.remainingDistance <= 0.1f && !_agent.pathPending)
                {
                    currentState = EnemyState.Idle;
                    stateEnter = true;
                    return;
                }
                break;
            case EnemyState.SoundSarch:
                #region 開始1回の処理
                if (stateEnter)
                {
                    stateEnter = false;
                }
                #endregion
                if (_agent.remainingDistance <= 0.1f && !_agent.pathPending)
                {
                    currentState = EnemyState.Idle;
                    stopTimer = 60;
                    stateEnter = true;
                    return;
                }
                break;
            case EnemyState.Chase:
                #region 開始1回の処理
                if (stateEnter)
                {
                    animator.SetBool("isChase", true);
                    stateEnter = false;
                }
                #endregion

                if (_agent.remainingDistance <= 1f && !_agent.pathPending)
                {
                    currentState = EnemyState.Attack;
                    stateEnter = true;
                }
                else if (playerSounds.gameObject.activeSelf == false)
                {
                    currentState = EnemyState.Idle;
                    animator.SetBool("isChase", false);
                    stopTimer = 30;
                    stateEnter = true;
                }
                else
                {
                    if(chaseTimer <= 0)
                    {
                        currentState = EnemyState.Idle;
                        animator.SetBool("isChase", false);
                        stopTimer = 30;
                        stateEnter = true;
                    }
                    else
                    {
                        chaseTimer--;
                    }
                }
                break;
            case EnemyState.Attack:
                #region 開始1回の処理
                if (stateEnter)
                {
                    animator.SetTrigger("Attack Trigger");
                    stateEnter = false;
                }
                #endregion
                break;
        }
        #endregion
    }

    private void AgentSetUp()
    {
        chaseTimer = 0;
        stopTimer = 0;
        currentState = EnemyState.Idle;
        _agent = GetComponent<NavMeshAgent>();
        _agent.stoppingDistance = 1f;
        _speed = _agent.speed;
        stateEnter = true;
    }


    #region 音やプレイヤーの捜索
    public void PlayerSarch(Collider collider)
    {
        if (currentState == EnemyState.Attack || currentState == EnemyState.Idle) { return; }
        if (playerSounds.gameObject.activeSelf == false) { return; }
        // 検知オブジェクトがPlayer
        if (collider.CompareTag(Tag.Player))
        {
            var positionDiff = collider.transform.position - transform.position;  // 自身（敵）とプレイヤーの距離
            var angle = Vector3.Angle(transform.forward, positionDiff);  // 敵から見たプレイヤーの方向
            if (angle <= searchAngle)//視界の中にいたら
            {
                RaycastHit hit;
                Vector3 direction;   // Rayを飛ばす方向
                float distance = 10f;    // Rayを飛ばす距離

                Vector3 temp = collider.transform.position - transform.position;
                direction = temp.normalized;

                Ray ray = new Ray(transform.position, direction);  // Rayを飛ばす
                Debug.DrawRay(ray.origin, ray.direction * distance, Color.black);  // Rayをシーン上に描画
                if (Physics.Raycast(ray.origin, ray.direction, out hit, distance, Layer.EnemySight))
                {
                    if (hit.collider.CompareTag(Tag.Player))
                    {
                        stateEnter = currentState != EnemyState.Chase;
                        currentState = EnemyState.Chase;
                        _agent.destination = collider.transform.position;
                        enemySarch.position = collider.transform.position;
                        chaseTimer = 60;
                    }
                    else if(hit.collider.CompareTag(Tag.Sounds))
                    {
                        if (hit.collider.gameObject.transform.parent.parent.CompareTag(Tag.Player))
                        {
                            stateEnter = currentState != EnemyState.Chase;
                            currentState = EnemyState.Chase;
                            _agent.destination = collider.transform.position;
                            enemySarch.position = collider.transform.position;
                            chaseTimer = 60;
                        }
                    }
                }
            }
        }
    }

    public void SoundSarch(Collider collider)
    {
        if(currentState == EnemyState.Chase || currentState == EnemyState.Idle || currentState == EnemyState.Attack) { return; }
        if (collider.CompareTag(Tag.Sounds))
        {
            stateEnter = currentState != EnemyState.SoundSarch;
            currentState = EnemyState.SoundSarch;
            _agent.destination = collider.transform.position;
            enemySarch.position = collider.transform.position;
        }
    }
    #endregion

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (currentState == EnemyState.Chase)
        {
            Handles.color = new Color(1f, 0f, 0f, 0.3f);
        }
        else if (currentState == EnemyState.SoundSarch)
        {
            Handles.color = new Color(0f, 0f, 1f, 0.3f);
        }
        else
        {
            Handles.color = new Color(0f, 1f, 0f, 0.3f);
        }
        Handles.DrawSolidArc(transform.position, Vector3.up, Quaternion.Euler(0f, -searchAngle, 0f) * transform.forward, searchAngle * 2f, searchArea.radius * 1.5f);
    }
#endif
}
