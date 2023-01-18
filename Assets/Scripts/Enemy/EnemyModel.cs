﻿using System.Collections;
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
    private int movePosition = 0;

    [Header("敵のベース")]
    private NavMeshAgent _agent;
    public Animator animator;
    private float _speed;
    private float _acceleration;
    int chaseTimer;
    [HideInInspector] public int stopTimer;
    [SerializeField] private SphereCollider playerSounds;
    [SerializeField] Transform enemySarch;//今どこに向かっているか
    //視界
    [SerializeField] float searchAngle = 100f;
    [SerializeField] private SphereCollider searchArea;


    [Header("敵の色変え")]
    [ColorUsage(false, true)] public Color color1;
    [ColorUsage(false, true)] public Color color2;
    private Color lerpedColor = Color.white;
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
        AgentSetUp();
    }

    void Update()
    {
        animator.SetFloat("Speed", _agent.speed);

        SetColorLerp();
        SetEnemyEmission();

        #region 敵のステート
        switch (currentState)
        {
            case EnemyState.Idle:
                #region 開始1回の処理
                if (stateEnter)
                {
                    //追跡エフェクトを元に戻す
                    SetChase(false);
                    SetEnemyAudio((int)EnemyAudioName.audioNormal);

                    stateEnter = false;
                    _agent.speed = 0f;
                }
                #endregion

                if (stopTimer <= 0)
                {
                    currentState = EnemyState.Move;
                    _agent.speed = _speed;
                    _agent.acceleration = _acceleration;
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
                    //追跡エフェクトを元に戻す
                    stateEnter = false;
                    //int rnd = random.Range(0, sarchPosition.Count);
                    _agent.destination = sarchPosition[movePosition].position;
                    enemySarch.position = sarchPosition[movePosition].position;
                }
                #endregion
                if (_agent.remainingDistance <= 0.1f && !_agent.pathPending)
                {
                    currentState = EnemyState.Idle;
                    movePosition++;
                    if(movePosition >= sarchPosition.Count) {  movePosition = 0; }
                    stateEnter = true;
                    return;
                }
                break;
            case EnemyState.SoundSarch:
                #region 開始1回の処理
                if (stateEnter)
                {
                    //追跡エフェクトを元に戻す
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
                    //追跡エフェクトをつける
                    SetChase(true);
                    SetEnemyAudio((int)EnemyAudioName.audioChase);

                    _agent.speed = 0f;
                    _agent.acceleration = 0f;
                    stopTimer = 60;

                    animator.SetBool("isChase", true);
                    stateEnter = false;
                }
                #endregion

                if(stopTimer > 0)
                {
                    stopTimer--;
                }
                else
                {
                    _agent.speed = _speed;
                    _agent.acceleration = _acceleration;
                }

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
        _acceleration = _agent.acceleration;
        stateEnter = true;
    }

    private void SetChase(bool flag)
    {
        isChase = flag;
    }

    private void SetColorLerp()
    {
        lerpedColor = Color.Lerp(color1, color2, colorLerp);
        if(isChase)
        {
            if(colorLerp < 1)
            {
                colorLerp += 0.3f * Time.deltaTime;
            }
        }
        else
        {
            if(colorLerp > 0)
            {
                colorLerp -= 0.7f * Time.deltaTime;
            }
        }
    }

    private void SetEnemyEmission()
    {
        mpb.SetColor(Shader.PropertyToID("_EmissionColor"), lerpedColor);
        for(int i = 0; i < meshRenderer.Length; i++)
        {
            meshRenderer[i].material.EnableKeyword("_EMISSION");
            meshRenderer[i].SetPropertyBlock(m_mpb);
        }
    }

    private void SetEnemyAudio(int clipNum)
    {
        audioSource.clip = audioClips[clipNum];
        audioSource.Play();
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
                float distance = 20f;    // Rayを飛ばす距離

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
