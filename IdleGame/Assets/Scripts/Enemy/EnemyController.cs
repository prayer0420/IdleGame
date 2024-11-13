using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public Animator Animator {  get; private set; }
    public CharacterController CharacterController { get; private set; }

    private EnemyStateMachine stateMachine;
    private Transform playerTransform;

    public float sightRange = 10f;
    public float attackRange = 1.5f;
    public float moveSpeed = 2f;
    public float Health { get; private set; }
    public float AttackPower { get; private set; }
    public float DefensePower { get; private set; }
    private EnemyData enemyData;


    public bool IsDead => Health <= 0f;

    public void Awake()
    {
        Animator = GetComponent<Animator>();
        CharacterController = GetComponent<CharacterController>();

        stateMachine = new EnemyStateMachine(this);
        playerTransform = PlayerController.Instance.transform;
    }

    public void Start()
    {
        stateMachine.ChangeState(stateMachine.IdleState);
        
        // �� ������ �ε�
        int enemyID = 1; // ���� ID�� ����
        enemyData = EnemyDatabase.Instance.EnemyDatas.Find(e => e.EnemyID == enemyID);
        if (enemyData != null)
        {
            Health = enemyData.Health;
            AttackPower = enemyData.AttackPower;
            DefensePower = enemyData.DefensePower;
        }
    }

    public void Update()
    {
        stateMachine.Update();
    }

    public bool CanSeePlayer()
    {
        //�÷��̾���� �Ÿ� Ȯ��
        float distance = Vector3.Distance(transform.position, playerTransform.position);
        //�÷��̾��� �Ÿ��� �þ߰Ÿ����� �۴ٸ�
        return distance <= sightRange;
    }

    public bool IsPlayerInAttackRange()
    {
        //���ݹ������� �ִ��� Ȯ��
        float distance = Vector3.Distance(transform.position, playerTransform.position);
        return distance <= attackRange;
    }

    public void MoveTowardsPlayer()
    {
        //�÷��̾ ���� �̵�
        Vector3 direction = (playerTransform.position - transform.position).normalized;
        CharacterController.Move(direction * moveSpeed * Time.deltaTime);

        //���� ȸ��
        if(direction != Vector3.zero)
        {
            Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, Time.deltaTime);
        }
    }

    public void AttackPlayer()
    {
        //������ �����
        if (PlayerController.Instance.Health <= 0f)
            return;
        //�÷��̾�� ������ ������
        PlayerController.Instance.TakeDamage(5f);
        
    }

    public void TakeDamage(float damage)
    {
        Health -= damage;
        if (IsDead)
        {
            Die();
        }
        Debug.Log($"{this.name}�� ���ݹ޾Ҵ�. HP : {Health}");
    }

    private void Die()
    {
        Animator.SetTrigger("Die");
        
    }
}
