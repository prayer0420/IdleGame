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

        //���¸ӽ� �ʱ�ȭ
        stateMachine = new PlayerStateMachine(this);
    }


    private void Update()
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

        //�������� �����ϸ� ���� ������ ����


        //���� ȸ��
    }

    public bool IsEnemyInRange()
    {
        //�ֺ��� ���� �ִ��� Ž��
        Collider[] hits = Physics.OverlapSphere(transform.position, detectRange);

        foreach(var hit in hits)
        {
            //���� ���̶��

            //���� Ÿ���� �� ������ ����
            return true;
        }
        //���� �ƴ϶�� false
        return false;
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
        //��� �̺�Ʈ ȣ��
        OnDeath?.Invoke();
    }
}
