using System;
using System.Collections.Generic;
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

        //다음 목적시 설정
    }

    public void Execute()
    {

        //자동 이동

        //몬스터 감지

        //공격상태로 전환
    }

    public void Exit()
    {
        //이동 애니메이션 종료
    }
}