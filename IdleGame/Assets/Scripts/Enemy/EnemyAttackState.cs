using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackState : IState
{
    private EnemyStateMachine stateMachine;

    // 공격 쿨다운 시간 (초 단위)
    private float attackTime = 2f; // 예: 2초 간격으로 공격
    private float lastAttackTime = -Mathf.Infinity; // 초기화 시 공격 가능

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
        // 공격 가능 여부 확인
        if (Time.time >= lastAttackTime + attackTime)
        {
            if (!stateMachine.Enemy.IsPlayerInAttackRange())
            {
                // 공격 범위에서 벗어나면 추적 상태로 전환
                stateMachine.ChangeState(stateMachine.ChaseState);
            }
            else
            {
                // 공격 범위 내에 있으면 공격 수행
                stateMachine.Enemy.AttackPlayer();
                lastAttackTime = Time.time; // 마지막 공격 시간 업데이트
            }
        }
    }

    public void Exit()
    {
    }

}
