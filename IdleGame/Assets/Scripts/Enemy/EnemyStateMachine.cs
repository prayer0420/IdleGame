using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateMachine : StateMachine
{
    public EnemyController Enemy { get; private set; }
    public IState IdleState {  get; private set; }
    public IState ChaseState { get; private set; }
    public IState AttackState { get; private set; }

    public EnemyStateMachine(EnemyController enemy)
    {
        Enemy = enemy;
        IdleState = new EnemyIdleState(this);
        ChaseState = new EnemyChaseState(this);
        AttackState = new EnemyAttackState(this);
        ChangeState(IdleState);
    }

}
