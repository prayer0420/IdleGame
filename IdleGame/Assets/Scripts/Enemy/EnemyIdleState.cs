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
        //��� �ִϸ��̼� ����
        stateMachine.Enemy.Animator.SetBool("isMoving", false);
    }

    public void Execute()
    {
        //�÷��̾� ����
        if(stateMachine.Enemy.CanSeePlayer())
        {
            //�������·� ��ȯ
            stateMachine.ChangeState(stateMachine.ChaseState);
        }

    }

    public void Exit()
    {
    }
}
