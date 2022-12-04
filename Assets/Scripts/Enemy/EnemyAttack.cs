using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] private float _damage = 5.0f;
    BoxCollider leftCollider;
    BoxCollider rightCollider;

    void Start()
    {
        leftCollider = GameObject.Find("forearm_L.002").GetComponent<BoxCollider>();
        rightCollider = GameObject.Find("forearm_R.002").GetComponent<BoxCollider>();
    }
    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag(Tag.Player))
        {
            Debug.Log("hit");
            ColliderReset();

            PlayerModel playerModel = other.GetComponentInParent<PlayerModel>();
            if(playerModel != null)
            {
                playerModel.TakeDamage(_damage);
            }
        }
    }

    private void ColliderReset()
    {
        rightCollider.enabled = false;
        leftCollider.enabled = false;
    }
}
