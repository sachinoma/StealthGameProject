using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMoveStateAI : EnemyStateAIBase
{
    public override EnemyState State => EnemyState.Move;
    protected override bool IsChasingState => false;
    protected override bool IsEnableAgent => true;
    public override bool CanDetectPlayer => true;
    public override bool CanDetectSound => true;

    public EnemyMoveStateAI(EnemyStateAIInitInfo initInfo) : base(initInfo)
    {
    }

    public override void OnEnter(object additionalInfo = null)
    {
        base.OnEnter(additionalInfo);

        if(additionalInfo != null && additionalInfo is Transform target)
        {
            _agent.destination = target.position;
        }
        else
        {
            Debug.LogWarning("ターゲットがない。今の位置をターゲットになる");
            _agent.destination = _model.transform.position;
        }
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if(_ai.CheckIsReachDestination(0.1f))
        {
            _ai.ChangeState(EnemyState.Idle);
        }
    }
}
