using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySoundSearchStateAI : EnemyStateAIBase
{
    public override EnemyState State => EnemyState.SoundSarch;
    protected override bool IsChasingState => true;
    protected override bool IsEnableAgent => true;
    public override bool CanDetectPlayer => true;
    public override bool CanDetectSound => true;

    private const float TotalChasingTime = 2.0f;
    private float _startChasingTime;

    public EnemySoundSearchStateAI(EnemyStateAIInitInfo initInfo) : base(initInfo)
    {
    }

    public override void OnEnter(object additionalInfo = null)
    {
        base.OnEnter(additionalInfo);

        if(additionalInfo != null && additionalInfo is Transform target)
        {
            SoundDetected(target);
        }
        else
        {
            Debug.LogWarning("ターゲットがない。今の位置をターゲットになる");
            _agent.destination = _model.transform.position;
        }
    }

    public override void OnUpdate()
    {
        if(_ai.CheckIsReachDestination(0.1f))
        {
            _ai.ChangeState(EnemyState.Idle);
        }
        else if(Time.time - _startChasingTime > TotalChasingTime)
        {
            _ai.ChangeState(EnemyState.Idle, 0.5f);
        }
    }

    public override void SoundDetected(Transform target)
    {
        _agent.destination = target.position;
        _startChasingTime = Time.time;
    }
}
