using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChaseState : IState
{
    private EnemyStateMachine stateMachine;

    public EnemyChaseState(EnemyStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    public void Enter()
    {
        stateMachine.Enemy.Animator.SetBool("isMoving", true);
    }

    public void Execute()
    {
        //플레이어 향해 이동
        stateMachine.Enemy.MoveTowardsPlayer();

        //공격 범위 안에 들어오면 공격 상태로 전환
        if(stateMachine.Enemy.IsPlayerInAttackRange())
        {
            stateMachine.ChangeState(stateMachine.AttackState);
        }
        //플레이어가 시야에 사라져버리면 대기상태로 전환
        else if(!stateMachine.Enemy.CanSeePlayer())
        {
            stateMachine.ChangeState(stateMachine.IdleState);
        }
    }

    public void Exit()
    {
        stateMachine.Enemy.Animator.SetBool("isMoving", false);
    }

}
