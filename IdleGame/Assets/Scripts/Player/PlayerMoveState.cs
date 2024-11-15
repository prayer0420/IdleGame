﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class PlayerMoveState : IState
{
    private PlayerStateMachine stateMachine;

    public PlayerMoveState(PlayerStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    public void Enter()
    {
        //이동 애니메이션 시작
        stateMachine.PlayerController.Animator.SetBool("isMoving", true);
        ////다음 목적지 설정
        //stateMachine.PlayerController.SetNextDestination();
    }

    public void Execute()
    {
        //몬스터 감지
        if(stateMachine.PlayerController.IsEnemyInRange())
            stateMachine.ChangeState(stateMachine.AttackState);
        else
            //자동 이동
            stateMachine.PlayerController.AutoMove();
    }

    public void Exit()
    {
        //이동 애니메이션 종료
        stateMachine.PlayerController.Animator.SetBool("isMoving", false);
    }
}