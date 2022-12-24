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
        /*
         * バグ：
         * GameObject.FindはSceneの中で名前に当たるGameObjectを戻り値になる。
         * 例えば、3コのEnemy(EnemyA, EnemyB, EnemyC)がsceneに入れられた。EnemyごとにEnemyAttackを持っている。
         * でも、三つのEnemyAttackもEnemyCの"forearm_L.002"と"forearm_R.002"が探せる。

         * 自分の"forearm_L.002"を探したい場合、Transform.Findを使う。
         * でも、こっちの場合、EnemyAttackは"forearm_L.002"GameObjectに直接に追加した。そして、以下のようにするといい。
         * leftCollider = GetComponent<BoxCollider>();

         * ちらみに、僕はGameObject.FindとTransform.Findがあまり好きではない。
         * GameObjectの名前に関連するので、名前を変えると、バグが生じるということだ。
         * GameObjectの命名制約がない限り、危険だと思う。
         * プリハブの場合は、[SerializeField]を使うのが好きだ。

         * また、今"forearm_L.002"と"forearm_R.002"は各自にEnemyAttackを追加した。つまり、EnemyAttackの中に、一つのcolliderだけを参照するも足りる。
         */
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
