using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackState : IState
{
    private PlayerStateMachine playerStateMachine;

    public PlayerAttackState(PlayerStateMachine playerStateMachine)
    {
        this.playerStateMachine = playerStateMachine;
    }

    public void Enter()
    {
        //���� �ִϸ��̼� ����
        playerStateMachine.PlayerController.Animator.SetTrigger("Attack");
    }

    public void Execute()
    {
        //������ ����� ���ų� �÷��̾ �׾����� �̵����·� ��ȯ


        //�װԾƴ϶�� ��� ����
    }

    public void Exit()
    {

    }
}
