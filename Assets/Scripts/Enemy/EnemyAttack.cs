using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [Header("敵のモデル")]
    private EnemyModel enemyModel;

    //[SerializeField] private float _damage = 5.0f;
    [SerializeField] private Transform _caughtPos; 

    [Header("攻撃判定")]
    [SerializeField]
    BoxCollider attackCollider;
    /*
    BoxCollider leftCollider;
    BoxCollider rightCollider;
    */

    void Start()
    {
        enemyModel = GetComponentInParent<EnemyModel>();
        /*
        leftCollider = GameObject.Find("forearm_L.002").GetComponent<BoxCollider>();
        rightCollider = GameObject.Find("forearm_R.002").GetComponent<BoxCollider>();
        */
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
                //playerModel.TakeDamage(_damage);
                enemyModel.currentState = EnemyState.Catch;
                playerModel.Die();
                enemyModel.animator.SetBool("isGameOver", true);
                CaughtPlayer(playerModel);
            }
        }
    }


    public void CaughtPlayer(PlayerModel playerModel)
    {
        playerModel.transform.SetParent(_caughtPos,true);
        playerModel.transform.localPosition = new Vector3(0, -0.2f, 0);
    }

    private void ColliderReset()
    {
        attackCollider.enabled = false;
        /*
        rightCollider.enabled = false;
        leftCollider.enabled = false;
        */
    }
}
