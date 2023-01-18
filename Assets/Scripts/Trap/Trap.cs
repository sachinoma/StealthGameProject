using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    Animator animator;

    [SerializeField]
    private ParticleSystem trapFX;

    [Header("追加機能 - 緩める")]
    [SerializeField] private bool _isShowDown = false;
    [SerializeField] private float _showDownMultiplier = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag(Tag.Player))
        {
            animator.SetBool("isOn", true);
            trapFX.Play();

            SetPlayer(other, true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag(Tag.Player))
        {
            animator.SetBool("isOn", false);
            trapFX.Stop();

            SetPlayer(other, false);
        }
    }

    private void SetPlayer(Collider other, bool isTriggerEnter)
    {
        PlayerModel player = other.GetComponentInParent<PlayerModel>();
        if(player == null)
        {
            Debug.LogWarning("PlayerModelが探せない。");
            return;
        }

        float speedMultiplier = (_isShowDown && isTriggerEnter) ? _showDownMultiplier : 1;
        player.SetInTrap(isTriggerEnter, speedMultiplier);
    }
}
