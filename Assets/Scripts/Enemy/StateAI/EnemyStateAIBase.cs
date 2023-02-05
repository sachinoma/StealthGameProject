using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public struct EnemyStateAIInitInfo
{
    public EnemyAI ai;
    public EnemyModel model;
    public NavMeshAgent agent;
    public PlayerModel player;
}

public abstract class EnemyStateAIBase
{
    protected EnemyAI _ai;
    protected EnemyModel _model;
    protected NavMeshAgent _agent;
    protected PlayerModel _player;

    public abstract EnemyState State { get; }
    protected virtual bool IsChasingState { get; }
    protected virtual bool IsEnableAgent { get; }
    public virtual bool CanDetectPlayer { get; }
    public virtual bool CanDetectSound { get; }

    public EnemyStateAIBase(EnemyStateAIInitInfo initInfo)
    {
        _ai = initInfo.ai;
        _model = initInfo.model;
        _agent = initInfo.agent;
        _player = initInfo.player;
    }

    public virtual void OnEnter(object additionalInfo = null)
    {
        _model.UpdateEnemyChasing(IsChasingState);
        _agent.isStopped = !IsEnableAgent;
    }

    public virtual void OnUpdate()
    {

    }

    public virtual void OnExit()
    {

    }

    public virtual void PlayerDetected()
    {
        _ai.ChangeState(EnemyState.Chase);
    }

    public virtual void SoundDetected(Transform target)
    {
        _ai.ChangeState(EnemyState.SoundSarch, target);
    }
}
