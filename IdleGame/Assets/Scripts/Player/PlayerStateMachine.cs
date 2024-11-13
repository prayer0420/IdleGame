using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine : StateMachine
{
    public PlayerController PlayerController {  get; private set; }
    public IState MoveState { get; private set; }
    public IState AttackState { get; private set; }

    public PlayerStateMachine(PlayerController playerController)
    {
        PlayerController = playerController;
        MoveState = new PlayerMoveState(this);
        AttackState = new PlayerAttackState(this);
        ChangeState(MoveState); //초기상태를 이동상태로 설정

    }
}
