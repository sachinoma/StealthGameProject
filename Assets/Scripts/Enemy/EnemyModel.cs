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

    [Header("徘徊場所")]
    [SerializeField] List<Transform> sarchPosition;
    private int movePosition = 0;

    [Header("敵のベース")]
    private NavMeshAgent _agent;
    public Animator animator;

    private float chaseTimer;
    private float stopTimer;
    private PlayerModel _player;
    private Transform _currentTarget;

    //視界
    [SerializeField] float searchAngle = 100f;
    [SerializeField] private SphereCollider searchArea;


    [Header("敵の色変え")]
    [ColorUsage(false, true)] public Color color1;
    [ColorUsage(false, true)] public Color color2;
    private bool isChase = false;
    [SerializeField] private float colorLerp = 0;

    [SerializeField, Header("メッシュレンダラー")]
    private SkinnedMeshRenderer[] meshRenderer;

    private MaterialPropertyBlock m_mpb;
    public MaterialPropertyBlock mpb
    {
        get { return m_mpb ?? (m_mpb = new MaterialPropertyBlock()); }
    }

    [Header("サウンド")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] audioClips;
    private enum EnemyAudioName
    {
        audioNormal,
        audioChase,
    }

    void Start()
    {
        _player = FindObjectOfType<PlayerModel>();
        AgentSetUp();
        EnterState(EnemyState.Idle);    // 初期化のために、ChangeState()ではなく、EnterState()を呼び出す
    }

    void Update()
    {
        animator.SetFloat("Speed", GetAgentVelocity());

        SetEnemyEmission();

        #region 敵のステート
        switch (currentState)
        {
            case EnemyState.Idle:
                stopTimer -= Time.deltaTime;
                if (stopTimer < 0)
                {
                    ChangeState(EnemyState.Move);
                }
                break;
            case EnemyState.Move:
                if (CheckIsReachDestination(0.1f))
                {
                    movePosition++;
                    if(movePosition >= sarchPosition.Count) { movePosition = 0; }
                    ChangeState(EnemyState.Idle);
                }
                break;
            case EnemyState.SoundSarch:
                UpdateAgentTargetPos();

                if (CheckIsReachDestination(0.1f))
                {
                    ChangeState(EnemyState.Idle);
                }
                else
                {
                    chaseTimer -= Time.deltaTime;
                    if (chaseTimer < 0)
                    {
                        ChangeState(EnemyState.Idle, 0.5f);
                    }
                }
                break;
            case EnemyState.Chase:
                UpdateAgentTargetPos();

                if (CheckIsReachDestination(1.0f))
                {
                    ChangeState(EnemyState.Attack);
                }
                else if (!_player.CanBeDetected())
                {
                    ChangeState(EnemyState.Idle, 0.5f);
                }
                else
                {
                    chaseTimer -= Time.deltaTime;
                    if(chaseTimer < 0)
                    {
                        ChangeState(EnemyState.Idle, 0.5f);
                    }
                }
                break;
            case EnemyState.Attack:
                break;
            case EnemyState.Catch:
                break;
        }
        #endregion
    }

    private void AgentSetUp()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.stoppingDistance = 1f;
    }

    public void ChangeState(EnemyState newState, object additionalInfo = null)
    {
        if(currentState == newState)
        {
            return;
        }

        ExitState(currentState, additionalInfo);
        EnterState(newState, additionalInfo);
    }

    private void EnterState(EnemyState stateToEnter, object additionalInfo = null)
    {
        SetChase(stateToEnter);
        UpdateEnemyAudio();
        SetAgentEnable(stateToEnter);

        switch(stateToEnter)
        {
            case EnemyState.Idle:
                if(additionalInfo != null && additionalInfo is float idleTime)
                {
                    stopTimer = idleTime;
                }
                else
                {
                    stopTimer = 1.0f;
                }
                break;
            case EnemyState.Move:
                SetAgentTarget(sarchPosition[movePosition]);
                UpdateAgentTargetPos();
                break;
            case EnemyState.Chase:
                animator.SetBool("isChase", true);
                StartCoroutine(DelayStartChasing(1.0f));
                break;
            case EnemyState.Attack:
                animator.SetBool("isChase", true);
                animator.SetTrigger("Attack Trigger");
                break;
        }

        currentState = stateToEnter;
    }

    private void ExitState(EnemyState stateToExit, object additionalInfo = null)
    {
        switch(stateToExit)
        {
            case EnemyState.Chase:
            case EnemyState.Attack:
                animator.SetBool("isChase", false);
                break;
        }
    }

    float GetAgentVelocity()
    {
        return _agent.velocity.magnitude;
    }

    private void SetChase(EnemyState enterState)
    {
        switch(enterState)
        {
            case EnemyState.Chase:
            case EnemyState.Attack:
            case EnemyState.Catch:
                isChase = true;
                return;
            case EnemyState.SoundSarch:
                // このチェックは必要ですか？
                isChase = _currentTarget.root.gameObject.CompareTag(Tag.Player);
                return;
            default:
                isChase = false;
                return;
        }
    }

    private void SetEnemyEmission()
    {
        if(isChase)
        {
            colorLerp = Mathf.Min(1, colorLerp + 0.3f * Time.deltaTime);
        }
        else
        {
            colorLerp = Mathf.Max(0, colorLerp - 0.7f * Time.deltaTime);
        }
        Color lerpedColor = Color.Lerp(color1, color2, colorLerp);
        mpb.SetColor(Shader.PropertyToID("_EmissionColor"), lerpedColor);
        for(int i = 0; i < meshRenderer.Length; i++)
        {
            meshRenderer[i].material.EnableKeyword("_EMISSION");
            meshRenderer[i].SetPropertyBlock(m_mpb);
        }
    }

    private void UpdateEnemyAudio()
    {
        EnemyAudioName audioName = isChase ? EnemyAudioName.audioChase : EnemyAudioName.audioNormal;
        audioSource.clip = audioClips[(int)audioName];
        audioSource.Play();
    }

    private void SetAgentEnable(EnemyState enterState)
    {
        switch(enterState)
        {
            case EnemyState.Chase:      // ちょっと止まる演出
            case EnemyState.Idle:
            case EnemyState.Catch:
            case EnemyState.Attack:
                _agent.isStopped = true;
                return;
            default:
                _agent.isStopped = false;
                return;
        }
    }

    private bool CheckIsReachDestination(float allowDistance)
    {
        return !_agent.pathPending && _agent.remainingDistance <= allowDistance;
    }

    private void SetAgentTarget(Transform target)
    {
        _currentTarget = target;
    }

    private void UpdateAgentTargetPos()
    {
        _agent.destination = _currentTarget.position;
    }

    private IEnumerator DelayStartChasing(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);

        // delayTime経ってから、currentStateは変わる可能もある
        if(currentState == EnemyState.Chase)
        {
            SetAgentTarget(_player.transform);
            _agent.isStopped = false;
        }
    }

    #region 音やプレイヤーの捜索
    public void PlayerSarch(Collider collider)
    {
        if (currentState == EnemyState.Attack || currentState == EnemyState.Idle || currentState == EnemyState.Catch) { return; }
        if (!_player.CanBeDetected()) { return; }
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
                    if (hit.transform.root.CompareTag(Tag.Player))
                    {
                        ChangeState(EnemyState.Chase);
                        chaseTimer = 3.0f;
                    }
                }
            }
        }
    }

    public void SoundSarch(Collider collider)
    {
        if(currentState == EnemyState.Chase || currentState == EnemyState.Idle || currentState == EnemyState.Attack || currentState == EnemyState.Catch) { return; }
        if (!_player.CanBeDetected()) { return; }
        if (collider.CompareTag(Tag.Sounds))
        {
            SetAgentTarget(collider.transform);
            ChangeState(EnemyState.SoundSarch);
            chaseTimer = 2.0f;
        }
    }
    #endregion

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (currentState == EnemyState.Chase || currentState == EnemyState.Attack || currentState == EnemyState.Catch)
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
        Handles.DrawSolidArc(transform.position, Vector3.up, Quaternion.Euler(0f, -searchAngle, 0f) * transform.forward, searchAngle * 2f, searchArea.radius * 1.0f);
    }
#endif
}
