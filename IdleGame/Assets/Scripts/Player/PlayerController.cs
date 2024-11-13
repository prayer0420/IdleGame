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
    public float MaxHealth;

    public float Health { get; set; }
    public float AttackPower { get; set; }
    public float DefensePower { get; set; }

    public Item EquippedWeapon { get; set; }
    public Item EquippedArmor { get; set; }

    [field:SerializeField] public GameObject ExitPoint {get; set; }
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

    public void OnEnable()
    {
        CharacterDatabase.Instance.DataLoadComplete += HandleDataLoadComplete;
    }
    public void OnDisable()
    {
        CharacterDatabase.Instance.DataLoadComplete -= HandleDataLoadComplete;
    }
    public void Update()
    {
        stateMachine.Update();
    }

    public void HandleDataLoadComplete()
    {
        CharacterData data = CharacterDatabase.Instance.playerData;
        MaxHealth = data.Health;
        Health = data.Health;
        Debug.Log(Health);
        AttackPower = data.AttackPower;
        DefensePower = data.DefensePower;
    }

    public void SetNextDestination()
    {
        //�����Ŵ����κ��� ���������� ����
    }

    public void AutoMove()
    {
        //�������� �̵�
        Vector3 destination = ExitPoint.transform.position;
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
                Debug.Log("�� ã�Ҵ�");
                //���� Ÿ���� �� ������ ����
                CurrentTarget = hit.GetComponent<EnemyController>();
                Debug.Log(CurrentTarget.name);
                return true;
            }    
        }
        //���� �ƴ϶�� false
        return false;
    }

    public void AttackEnemy()
    { 
        if(CurrentTarget == null)
            return;

        //������ �����
        if (CurrentTarget.IsDead)
            return;

        //���ݹ������� �̵�
        if(Vector3.Distance(transform.position, CurrentTarget.transform.position) > attackRange)
        {
            Vector3 direction = (CurrentTarget.transform.position - transform.position).normalized;
            CharacterController.Move(direction * moveSpeed * Time.deltaTime);

        }
        else
        {
            Debug.Log("����");
            CurrentTarget.TakeDamage(10f);
        }

        // ���� ȸ��
        Vector3 targetDirection = (CurrentTarget.transform.position - transform.position).normalized;
        if (targetDirection != Vector3.zero)
        {
            Debug.Log("�� �ٶ󺸱�");
            Quaternion toRotation = Quaternion.LookRotation(targetDirection, Vector3.up);
            float rotationSpeed = 10f; 
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed*Time.deltaTime);
        }
    }



public void TakeDamage(float damage)
    {
        Health -= damage;
        if(Health<=0f)
        {
            Die();
        }
        //ü�º��� �˸�
        OnHealthUpdate?.Invoke();
        Debug.Log($"{this.name}�� ���ݹ޾Ҵ�. HP : {Health}");
    }

    public void Die()
    {
        Animator.SetTrigger("Die");
        //��� �̺�Ʈ ȣ��
        OnDeath?.Invoke();
    }
}
