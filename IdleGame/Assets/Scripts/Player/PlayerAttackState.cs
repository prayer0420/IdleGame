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
        //공격 애니메이션 시작
        playerStateMachine.PlayerController.Animator.SetTrigger("Attack");
    }

    public void Execute()
    {
        //공격할 대상이 없거나 플레이어가 죽었으면 이동상태로 전환


        //그게아니라면 계속 공격
    }

    public void Exit()
    {

    }
}
