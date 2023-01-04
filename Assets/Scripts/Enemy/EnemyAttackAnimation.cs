﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackAnimation : MonoBehaviour
{
    [Header("敵のモデル")]
    private EnemyModel enemyModel;

    [Header("攻撃判定")]
    BoxCollider leftCollider;
    BoxCollider rightCollider;

    private void Start()
    {
        enemyModel = gameObject.transform.parent.GetComponent<EnemyModel>();
        leftCollider = GameObject.Find("forearm_L.002").GetComponent<BoxCollider>();
        rightCollider = GameObject.Find("forearm_R.002").GetComponent<BoxCollider>();
    }

    #region 攻撃処理
    private void AttackEnd()
    {
        enemyModel.currentState = EnemyState.Idle;
        enemyModel.animator.SetBool("isChase", false);
        enemyModel.stateEnter = true;
        enemyModel.stopTimer = 30;
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
}
