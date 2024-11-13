using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackState : IState
{
    private PlayerStateMachine stateMachine;

    public PlayerAttackState(PlayerStateMachine playerStateMachine)
    {
        this.stateMachine = playerStateMachine;
    }

    public void Enter()
    {
        //���� �ִϸ��̼� ����
        stateMachine.PlayerController.Animator.SetTrigger("Attack");
    }

    public void Execute()
    {
        if (stateMachine.PlayerController.CurrentTarget == null || stateMachine.PlayerController.CurrentTarget.IsDead)
        {
            // ������ ����� ���ų� �׾����� �̵� ���·� ��ȯ
            stateMachine.PlayerController.CurrentTarget = null;
            stateMachine.ChangeState(stateMachine.MoveState);
        }
        else
        {
            // �� ����
            stateMachine.PlayerController.AttackEnemy();
        }
    }

    public void Exit()
    {

    }
}
