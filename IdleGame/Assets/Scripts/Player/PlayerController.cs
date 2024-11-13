using System;
using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : SingletonDontDestroyOnLoad<PlayerController>
{

    public Animator Animator {  get; private set; }
    public CharacterController CharacterController { get; private set; }

    public float moveSpeed = 2f;
    public float attackRange = 1.5f;
    public float detectRange = 5f;
    public float health = 100f;

    private Vector3 destination;
    private PlayerStateMachine stateMachine;

    public Action OnDeath;
    public Action OnHealthUpdate;

    protected override  void Awake()
    {
        base.Awake();

        Animator = GetComponent<Animator>();
        CharacterController = GetComponent<CharacterController>();

        //상태머신 초기화
        stateMachine = new PlayerStateMachine(this);
    }


    private void Update()
    {
        //상태머신 업데이트
        stateMachine.Update();
    }

    public void SetNextDestination()
    {
        //던전매니저로부터 다음목적지 설정
    }

    public void AutoMove()
    {
        //목적지로 이동

        //목적지에 도달하면 다음 목적지 설정


        //방향 회전
    }

    public bool IsEnemyInRange()
    {
        //주변에 적이 있는지 탐색
        Collider[] hits = Physics.OverlapSphere(transform.position, detectRange);

        foreach(var hit in hits)
        {
            //만약 적이라면

            //현재 타겟을 그 적으로 변경
            return true;
        }
        //적이 아니라면 false
        return false;
    }



    public void TakeDamage(float damage)
    {
        health -= damage;
        if(health<=0f)
        {
            Die();
        }
        //체력변경 알림
        OnHealthUpdate?.Invoke();
    }

    public void Die()
    {
        //사망 이벤트 호출
        OnDeath?.Invoke();
    }
}
