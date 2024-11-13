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
    public EnemyController CurrentTarget { get; set; }

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

    public void Start()
    {
        CurrentTarget = FindObjectOfType<EnemyController>();
    }
    public void Update()
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
        Vector3 direction  = (destination-transform.position).normalized;
        CharacterController.Move(direction * moveSpeed * Time.deltaTime);
        //목적지에 도달하면 다음 목적지 설정
        if(Vector3.Distance(transform.position, destination)<0.1f)
        {
            SetNextDestination();
        }
        //방향 회전
        if(direction!=Vector3.zero)
        {
            Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation,rotation, Time.deltaTime);
        }
    }

    public bool IsEnemyInRange()
    {
        //주변에 적이 있는지 탐색
        Collider[] hits = Physics.OverlapSphere(transform.position, detectRange);

        foreach(var hit in hits)
        {
            //만약 적이라면
            if(hit.CompareTag("Enemy"))
            {
                //현재 타겟을 그 적으로 변경
                CurrentTarget = hit.GetComponent<EnemyController>();
                return true;
            }    
        }
        //적이 아니라면 false
        return false;
    }

    public void AttackEnemy()
    { 
      
        if(CurrentTarget ==null)
            return;

        //공격범위내로 이동
        if(Vector3.Distance(transform.position, CurrentTarget.transform.position) > attackRange)
        {
            Vector3 direction = (CurrentTarget.transform.position - transform.position).normalized;
            CharacterController.Move(direction * moveSpeed * Time.deltaTime);

        }
        else
        {
            CurrentTarget.TakeDamage(10f);
        }

        // 방향 회전
        Vector3 targetDirection = (CurrentTarget.transform.position - transform.position).normalized;
        if (targetDirection != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(targetDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, 720 * Time.deltaTime);
        }
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
        Animator.SetTrigger("Die");
        //사망 이벤트 호출
        OnDeath?.Invoke();
    }
}
