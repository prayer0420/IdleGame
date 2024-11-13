using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIdleState : IState
{

    private EnemyStateMachine stateMachine;

    public EnemyIdleState(EnemyStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    public void Enter()
    {
        //대기 애니메이션 설정
        stateMachine.Enemy.Animator.SetBool("isMoving", false);
    }

    public void Execute()
    {
        //플레이어 감지
        if(stateMachine.Enemy.CanSeePlayer())
        {
            //추적상태로 전환
            stateMachine.ChangeState(stateMachine.ChaseState);
        }

    }

    public void Exit()
    {
    }
}
