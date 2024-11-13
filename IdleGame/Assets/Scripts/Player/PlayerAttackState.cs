using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackState : IState
{
    private PlayerStateMachine stateMachine;

    private float attackTime = 2f;
    private float lastAttackTime = -Mathf.Infinity;


    public PlayerAttackState(PlayerStateMachine playerStateMachine)
    {
        this.stateMachine = playerStateMachine;
    }

    public void Enter()
    {
        stateMachine.PlayerController.Animator.SetTrigger("Attack");
    }

    public void Execute()
    {
        if (Time.time >= lastAttackTime + attackTime)
        {
            if (stateMachine.PlayerController.CurrentTarget == null || stateMachine.PlayerController.CurrentTarget.IsDead)
            {
                // ������ ����� ���ų� �׾����� �̵� ���·� ��ȯ
                Debug.Log("���ݴ����ų� �׾���?");
                stateMachine.PlayerController.CurrentTarget = null;
                stateMachine.ChangeState(stateMachine.MoveState);
            }
            else
            {
                // �� ����
                stateMachine.PlayerController.AttackEnemy();
                lastAttackTime = Time.time;
            }
        }


    }

    public void Exit()
    {

    }
}
