using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackState : IState
{
    private EnemyStateMachine stateMachine;

    public EnemyAttackState(EnemyStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    public void Enter()
    {
        stateMachine.Enemy.Animator.SetTrigger("Attack");
    }

    public void Execute()
    {
        if(!stateMachine.Enemy.IsPlayerInAttackRange())
        {
            //공격범위에서 벗어나면 추적상태로 전환
            stateMachine.ChangeState(stateMachine.ChaseState);
        }
        else
        {
            //그게 아니라면 플레이어 공격
            stateMachine.Enemy.AttackPlayer();
        }
    }

    public void Exit()
    {
    }

}
