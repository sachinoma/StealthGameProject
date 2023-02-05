using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyIdleStateAI : EnemyStateAIBase
{
    public override EnemyState State => EnemyState.Idle;
    protected override bool IsChasingState => false;
    protected override bool IsEnableAgent => false;
    public override bool CanDetectPlayer => false;
    public override bool CanDetectSound => false;

    private Coroutine _waitCoroutine = null;

    public EnemyIdleStateAI(EnemyStateAIInitInfo initInfo) : base(initInfo)
    {
    }

    public override void OnEnter(object additionalInfo = null)
    {
        base.OnEnter(additionalInfo);

        float idleTime = 1.0f;
        if(additionalInfo != null && additionalInfo is float time)
        {
            idleTime = time;
        }

        _waitCoroutine = _ai.StartCoroutine(ChangeToMoveState(idleTime));
    }

    public override void OnExit()
    {
        base.OnExit();

        if(_waitCoroutine != null)
        {
            _ai.StopCoroutine(_waitCoroutine);
            _waitCoroutine = null;
        }
    }

    private IEnumerator ChangeToMoveState(float idleTime)
    {
        yield return new WaitForSeconds(idleTime);

        _waitCoroutine = null;

        _ai.ChangeState(EnemyState.Move, _ai.GetNextSearchPos());

    }
}
