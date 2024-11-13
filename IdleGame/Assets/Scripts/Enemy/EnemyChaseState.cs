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
        //�÷��̾� ���� �̵�
        stateMachine.Enemy.MoveTowardsPlayer();

        //���� ���� �ȿ� ������ ���� ���·� ��ȯ
        if(stateMachine.Enemy.IsPlayerInAttackRange())
        {
            stateMachine.ChangeState(stateMachine.AttackState);
        }
        //�÷��̾ �þ߿� ����������� �����·� ��ȯ
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
