using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackStateAI : EnemyStateAIBase
{
    public override EnemyState State => EnemyState.Attack;
    protected override bool IsChasingState => true;
    protected override bool IsEnableAgent => false;
    public override bool CanDetectPlayer => false;
    public override bool CanDetectSound => false;

    public EnemyAttackStateAI(EnemyStateAIInitInfo initInfo) : base(initInfo)
    {
    }

    public override void OnEnter(object additionalInfo = null)
    {
        base.OnEnter(additionalInfo);

        _model.Attack();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        // EnemyAttackStateAIのOnUpdate()を呼び出すというのは、
        // EnemyModelが攻撃状態から回復して、EnemyModel.CanControlをtrueに戻るということです。
        _ai.ChangeState(EnemyState.Idle);
    }
}
