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
                // 공격할 대상이 없거나 죽었으면 이동 상태로 전환
                Debug.Log("공격대상없거나 죽었나?");
                stateMachine.PlayerController.CurrentTarget = null;
                stateMachine.ChangeState(stateMachine.MoveState);
            }
            else
            {
                // 적 공격
                stateMachine.PlayerController.AttackEnemy();
                lastAttackTime = Time.time;
            }
        }


    }

    public void Exit()
    {

    }
}
