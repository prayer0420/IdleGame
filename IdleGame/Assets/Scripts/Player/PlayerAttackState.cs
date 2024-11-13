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
        //공격 애니메이션 시작
        stateMachine.PlayerController.Animator.SetTrigger("Attack");
    }

    public void Execute()
    {
        if (stateMachine.PlayerController.CurrentTarget == null || stateMachine.PlayerController.CurrentTarget.IsDead)
        {
            // 공격할 대상이 없거나 죽었으면 이동 상태로 전환
            stateMachine.PlayerController.CurrentTarget = null;
            stateMachine.ChangeState(stateMachine.MoveState);
        }
        else
        {
            // 적 공격
            stateMachine.PlayerController.AttackEnemy();
        }
    }

    public void Exit()
    {

    }
}
