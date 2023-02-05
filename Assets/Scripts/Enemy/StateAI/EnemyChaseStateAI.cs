using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyChaseStateAI : EnemyStateAIBase
{
    public override EnemyState State => EnemyState.Chase;
    protected override bool IsChasingState => true;
    protected override bool IsEnableAgent => false;      // ちょっと止まる演出
    public override bool CanDetectPlayer => true;
    public override bool CanDetectSound => false;

    private const float DelayChaseTime = 1.0f;
    private Coroutine _waitCoroutine;

    private const float TotalChasingTime = 3.0f;
    private float _startChasingTime;

    public EnemyChaseStateAI(EnemyStateAIInitInfo initInfo) : base(initInfo)
    {
    }

    public override void OnEnter(object additionalInfo = null)
    {
        base.OnEnter(additionalInfo);

        PlayerDetected();
        _waitCoroutine = _ai.StartCoroutine(WaitAndChase());
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        _agent.destination = _player.transform.position;

        if(_ai.CheckIsReachDestination(1.0f))
        {
            _ai.ChangeState(EnemyState.Attack);
        }
        else if(!_player.CanBeDetected())
        {
            _ai.ChangeState(EnemyState.Idle, 0.5f);
        }
        else if(Time.time - _startChasingTime > TotalChasingTime)
        {
            _ai.ChangeState(EnemyState.Idle, 0.5f);
        }
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

    private IEnumerator WaitAndChase()
    {
        yield return new WaitForSeconds(DelayChaseTime);

        _waitCoroutine = null;

        _agent.isStopped = false;
    }

    public override void PlayerDetected()
    {
        _startChasingTime = Time.time;
    }
}
