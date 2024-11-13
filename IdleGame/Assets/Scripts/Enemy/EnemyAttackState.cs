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
            //���ݹ������� ����� �������·� ��ȯ
            stateMachine.ChangeState(stateMachine.ChaseState);
        }
        else
        {
            //�װ� �ƴ϶�� �÷��̾� ����
            stateMachine.Enemy.AttackPlayer();
        }
    }

    public void Exit()
    {
    }

}
