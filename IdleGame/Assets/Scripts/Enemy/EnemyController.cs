using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    public MapController currentMap; // ���� �� ������ ����

    public float gravity = -9.81f;
    private Vector3 velocity;
    private bool isGrounded;
    public float groundCheckDistance = 0.4f;
    public LayerMask groundMask;

    public Transform groundCheck;


    public bool IsDead => Health <= 0f;

    public void Awake()
    {
        Animator = GetComponent<Animator>();
        CharacterController = GetComponent<CharacterController>();

        stateMachine = new EnemyStateMachine(this);
        playerTransform = PlayerController.Instance.transform;
    }


    public void HandleDataLoadComplete()
    {

    }

    public void OnDisable()
    {
        Health = 100f;
        AttackPower = 10f;
        DefensePower = 5f;
    }
    public void Start()
    {
        // �� ������ �ε�
        int enemyID = 1; // ���� ID�� �����ϰų� �����տ��� ������ �� �ֵ��� ����
        enemyData = EnemyDatabase.Instance.EnemyDatas.Find(e => e.EnemyID == enemyID);
        if (enemyData != null)
        {
            Health = enemyData.Health;
            AttackPower = enemyData.AttackPower;
            DefensePower = enemyData.DefensePower;
        }

        stateMachine.ChangeState(stateMachine.IdleState);
    }

    public void Update()
    {
        HandleGravity();
        stateMachine.Update();
    }
    private void HandleGravity()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // �ణ�� �Ʒ��� ���� �־� Grounded ���¸� ����
        }

        velocity.y += gravity * Time.deltaTime;
        CharacterController.Move(velocity * Time.deltaTime);
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
            float rotationSpeed = 10f;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, rotationSpeed*Time.deltaTime);
        }
    }

    public void AttackPlayer()
    {
        Debug.Log("�÷��̾ ����");
        //������ �����
        if (PlayerController.Instance.Health <= 0f)
        {
            Debug.Log(PlayerController.Instance.Health);
            return;
        }
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
        // ���� �ʿ� ���� ����� �˸�
        // ��� �ִϸ��̼� �� ó��
        StartCoroutine(HandleDeath());
        currentMap.OnMonsterDeath(gameObject);
    }

    private IEnumerator HandleDeath()
    {
        // ��� �ִϸ��̼��� ���̸�ŭ ���
        yield return new WaitForSeconds(2f); 
        gameObject.SetActive(false);
    }

}
