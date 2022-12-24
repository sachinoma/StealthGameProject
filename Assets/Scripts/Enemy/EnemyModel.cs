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
    /*
     * このプロジェクトの命名規則
     * EnemyState _currentState;
     */

    /*
     * privateを付けるかどうかがあまり構わないと思うけど、一致するともっと見やすいかなと思う
     */
    [Header("ステート")]
    EnemyState currentState;
    bool stateEnter;

    [Header("徘徊場所")]
    /*
     * ArrayとかListとかの場合、s / Array / List とかを付けた方がいいと思う。
     * 例えば、searchPositions / searchPosList
     * ちらみに、"search"は正確な綴り方だ
     */
    [SerializeField] List<Transform> sarchPosition;

    [Header("敵のベース")]
    private NavMeshAgent _agent;
    private Animator _animator;
    private float _speed;
    int chaseTimer;
    int stopTimer;
    [SerializeField] private SphereCollider playerSounds;
    [SerializeField] Transform enemySarch;//今どこに向かっているか
    //視界
    [SerializeField] float searchAngle = 100f;
    [SerializeField] private SphereCollider searchArea;

    [Header("攻撃判定")]
    BoxCollider leftCollider;
    BoxCollider rightCollider;

    void Start()
    {
        AgentSetUp();

        /*
         * バグ：
         * EnemyAttackのコメントを参考にする
         */

        /*
         * 攻撃のことをEnemyAttackに任せたから、ここはleftColliderとrightColliderをいじるべきではない
         * 例えば、
         * private EnemyAttack[] enemyAttacks;
         * 
         * private void Awake()
         * {
         *     enemyAttacks = GetComponentsInChildren<EnemyAttack>();
         * }
         * 
         * 攻撃をする時：
         * foreach(EnemyAttack enemyAttack in enemyAttacks)
         * {
         *     enemyAttack.Attack();
         * }
         */
        _animator = GameObject.Find("robot_enemy").GetComponent<Animator>();
        leftCollider = GameObject.Find("forearm_L.002").GetComponent<BoxCollider>();
        rightCollider = GameObject.Find("forearm_R.002").GetComponent<BoxCollider>();
    }

    /*
     * Update()のロジックは、ほどんとEnemyAIの役割だと思う
     * どう行動しればいいのかはEnemyAIの役割で、EnemyModelはただ行動を行う
     * 多分下野さんのBehaviour Treeを導入する際に、ここのコードをBehaviour Treeに移行するかなと思う
     */
    void Update()
    {
        _animator.SetFloat("Speed", _agent.speed);

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
                /*
                 * このif文の条件をよく使うから、関数になった方がいい
                 */
                if (_agent.remainingDistance <= 0.1f && !_agent.pathPending)
                {
                    currentState = EnemyState.Idle;
                    stateEnter = true;
                    /*
                     * 多分　stopTimer = 60;　とかを忘れた
                     * 実は関数を用意した方がいいと思う
                     * 例えば、
                     * void SetIdleState(int idleTime)
                     * {
                     *     currentState = EnemyState.Idle;
                     *     stateEnter = true;
                     *     stopTimer = idleTime;
                     * }
                     * 
                     * さらに、stateを変更する時、stateEnter = true も必要なので、以下のようになった方がいいかもしれない。
                     * private EnemyState _currentState;
                     * private EnemyState CurrentState
                     * {
                     *     get { return _currentState; }
                     *     set
                     *     {
                     *         if(_currentState != value)
                     *         {
                     *             _currentState = value;
                     *             stateEnter = true;
                     *         }
                     *     }
                     * }
                     * 
                     */
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
                    _animator.SetBool("isChase", true);
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
                    _animator.SetBool("isChase", false);
                    stopTimer = 30;
                    stateEnter = true;
                }
                else
                {
                    if(chaseTimer <= 0)
                    {
                        currentState = EnemyState.Idle;
                        _animator.SetBool("isChase", false);
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
                    _animator.SetTrigger("Attack Trigger");
                    Attack();
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
    /*
     * EnemyAIの役割だと思う
     */
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

                /*
                 * すでにpositionDiffを求めた
                 */
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
                        /*
                         * このような仕組みはあまりよくないと思う。
                         * というのは、hierarchyの親子関係を強く繋げるからだ。
                         * 
                         * そういえば、なぜSounds tagもRaycastのターゲットになったか？
                         */
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

    /*
     * EnemyAIの役割だと思う
     */
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

    #region 攻撃処理
    void Attack()
    {
        /*
         * 個人的にInvokeの使用があまりおすすめしない。
         * Invokeは関数のstringによって関数に紐つくので、Visual Studioで参照が探せない。
         * 
         * StartCoroutineがおすすめだ。
         * https://docs.unity3d.com/ScriptReference/MonoBehaviour.StartCoroutine.html
         * 一見複雑だけど、これは便利だと思う。(実は、StartCoroutineはUnity一番便利かつ強力なことだと思う。)
         * 
         * こっちの場合は、
         * 
         * void Attack()
         * {
         *     StartCoroutine(AttackCoroutine());
         * }
         * 
         * IEnumerator AttackCoroutine()
         * {
         *     yield return new WaitForSeconds(0.12f);
         *     ColliderStart();
         *     
         *     yield return new WaitForSeconds(1.0f);  // 1.12 - 0.12
         *     ColliderReset();
         *     
         *     yield return new WaitForSeconds(1.03f);  // 2.15 - 1.0 - 0.12
         *     AttackEnd();
         * }
         * 
         * 当然ですが、時間より良い制御方法がある。
         * WaitForSecondsの他に、色々なYieldInstructionもある。
         * https://docs.unity3d.com/ScriptReference/WaitForSeconds.html <-- 下の"See Also"を参考にする
         * 他には、
         * yield return null; <-- 次のフレームまで待っている
         * yield break; <-- returnのよう
         * yield AnotherCoroutine(); <-- AnotherCoroutineを完了まで待っている
         */
        Invoke("ColliderStart", 0.12f);
        Invoke("ColliderReset", 1.12f);
        Invoke("AttackEnd", 2.15f);
    }

    private void AttackEnd()
    {
        currentState = EnemyState.Idle;
        _animator.SetBool("isChase", false);
        stateEnter = true;
        stopTimer = 30;
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
