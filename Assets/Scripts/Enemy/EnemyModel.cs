using UnityEngine;


public class EnemyModel : MonoBehaviour
{
    [Header("敵のベース")]
    private EnemyAI _ai;
    public Animator animator;


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

    public bool CanControl { get; private set; } = true;

    private void Awake()
    {
        // 原則的に言うと、EnemyModelはAIの存在を知らないはずです。
        // でも、今敵の速度はAI(厳密に言うと、NavMeshAgent)に依存するので、
        // AIから速度を取得するほかないです。
        _ai = GetComponentInChildren<EnemyAI>();
    }

    void Start()
    {
    }

    void Update()
    {
        animator.SetFloat("Speed", _ai.GetAgentVelocity());

        SetEnemyEmission();
    }

    public void UpdateEnemyChasing(bool isChase)
    {
        this.isChase = isChase;
        UpdateEnemyAudio();
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

    public void Attack()
    {
        animator.SetTrigger("Attack Trigger");
        CanControl = false;
    }

    public void FinishedAttack(bool isCaught)
    {
        if(isCaught)
        {
            animator.SetBool("isGameOver", true);
        }
        else
        {
            CanControl = true;
        }
    }
}
