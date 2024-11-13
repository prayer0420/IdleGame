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

        //���¸ӽ� �ʱ�ȭ
        stateMachine = new PlayerStateMachine(this);
    }

    public void Start()
    {
        CurrentTarget = FindObjectOfType<EnemyController>();
    }
    public void Update()
    {
        //���¸ӽ� ������Ʈ
        stateMachine.Update();
    }

    public void SetNextDestination()
    {
        //�����Ŵ����κ��� ���������� ����
    }

    public void AutoMove()
    {
        //�������� �̵�
        Vector3 direction  = (destination-transform.position).normalized;
        CharacterController.Move(direction * moveSpeed * Time.deltaTime);
        //�������� �����ϸ� ���� ������ ����
        if(Vector3.Distance(transform.position, destination)<0.1f)
        {
            SetNextDestination();
        }
        //���� ȸ��
        if(direction!=Vector3.zero)
        {
            Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation,rotation, Time.deltaTime);
        }
    }

    public bool IsEnemyInRange()
    {
        //�ֺ��� ���� �ִ��� Ž��
        Collider[] hits = Physics.OverlapSphere(transform.position, detectRange);

        foreach(var hit in hits)
        {
            //���� ���̶��
            if(hit.CompareTag("Enemy"))
            {
                //���� Ÿ���� �� ������ ����
                CurrentTarget = hit.GetComponent<EnemyController>();
                return true;
            }    
        }
        //���� �ƴ϶�� false
        return false;
    }

    public void AttackEnemy()
    { 
      
        if(CurrentTarget ==null)
            return;

        //���ݹ������� �̵�
        if(Vector3.Distance(transform.position, CurrentTarget.transform.position) > attackRange)
        {
            Vector3 direction = (CurrentTarget.transform.position - transform.position).normalized;
            CharacterController.Move(direction * moveSpeed * Time.deltaTime);

        }
        else
        {
            CurrentTarget.TakeDamage(10f);
        }

        // ���� ȸ��
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
        //ü�º��� �˸�
        OnHealthUpdate?.Invoke();
    }

    public void Die()
    {
        Animator.SetTrigger("Die");
        //��� �̺�Ʈ ȣ��
        OnDeath?.Invoke();
    }
}
