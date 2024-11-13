using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackState : IState
{
    private EnemyStateMachine stateMachine;

    // ���� ��ٿ� �ð� (�� ����)
    private float attackTime = 2f; // ��: 2�� �������� ����
    private float lastAttackTime = -Mathf.Infinity; // �ʱ�ȭ �� ���� ����

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
        // ���� ���� ���� Ȯ��
        if (Time.time >= lastAttackTime + attackTime)
        {
            if (!stateMachine.Enemy.IsPlayerInAttackRange())
            {
                // ���� �������� ����� ���� ���·� ��ȯ
                stateMachine.ChangeState(stateMachine.ChaseState);
            }
            else
            {
                // ���� ���� ���� ������ ���� ����
                stateMachine.Enemy.AttackPlayer();
                lastAttackTime = Time.time; // ������ ���� �ð� ������Ʈ
            }
        }
    }

    public void Exit()
    {
    }

}
