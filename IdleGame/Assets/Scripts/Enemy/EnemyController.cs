using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public Animator Animator { get; private set; }
    public CharacterController CharacterController { get; private set; }

    private EnemyStateMachine stateMachine;
    private Transform playerTransform;

    public float sightRange = 10f;
    public float attackRange = 1.5f;
    public float moveSpeed = 2f;
    public float MaxHealth { get; private set; }
    public float Health { get; private set; }
    public float AttackPower { get; private set; }
    public float DefensePower { get; private set; }
    private EnemyData enemyData;

    public MapController currentMap; // 현재 맵 정보를 저장

    public float gravity = -9.81f;
    private Vector3 velocity;
    private bool isGrounded;
    public float groundCheckDistance = 0.4f;
    public LayerMask groundMask;

    public Transform groundCheck;

    public bool IsDead;

    public void Awake()
    {
        Animator = GetComponent<Animator>();
        CharacterController = GetComponent<CharacterController>();
        stateMachine = new EnemyStateMachine(this);
        
    }

    public void Start()
    {
        playerTransform = PlayerController.Instance.transform;
    }

    public void OnEnable()
    {
        // 적 데이터 로드 및 상태 초기화
        int enemyID = 1; // 적의 ID를 설정하거나 프리팹에서 설정할 수 있도록 변경
        enemyData = EnemyDatabase.Instance.EnemyDatas.Find(e => e.EnemyID == enemyID);
        if (enemyData != null)
        {
            MaxHealth = enemyData.Health;
            Health = enemyData.Health;
            AttackPower = enemyData.AttackPower;
            DefensePower = enemyData.DefensePower;
        }
        else
        {
            MaxHealth = 100f;
            Health = 100f;
            AttackPower = 10f;
            DefensePower = 5f;
        }

        IsDead = false;
        stateMachine.ChangeState(stateMachine.IdleState);
    }

    public void Update()
    {
        if (IsDead)
            return;

        HandleGravity();
        stateMachine.Update();
    }

    private void HandleGravity()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Grounded 상태 유지
        }

        velocity.y += gravity * Time.deltaTime;
        CharacterController.Move(velocity * Time.deltaTime);
    }

    public bool CanSeePlayer()
    {
        float distance = Vector3.Distance(transform.position, playerTransform.position);
        return distance <= sightRange;
    }

    public bool IsPlayerInAttackRange()
    {
        float distance = Vector3.Distance(transform.position, playerTransform.position);
        return distance <= attackRange;
    }

    public void MoveTowardsPlayer()
    {
        Vector3 direction = (playerTransform.position - transform.position).normalized;
        CharacterController.Move(direction * moveSpeed * Time.deltaTime);

        if (direction != Vector3.zero)
        {
            Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);
            float rotationSpeed = 10f;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
        }
    }

    public void AttackPlayer()
    {
        if (PlayerController.Instance.Health <= 0f)
            return;

        PlayerController.Instance.TakeDamage(AttackPower);
    }

    public void TakeDamage(float damage)
    {
        Health -= damage;

        if (Health <= 0f)
        {
            IsDead = true;
            Die();
        }
    }

    private void Die()
    {
        Animator.SetTrigger("Die");
        currentMap.OnMonsterDeath(gameObject);
        StartCoroutine(HandleDeath());
    }

    private IEnumerator HandleDeath()
    {
        yield return new WaitForSeconds(2f);
        gameObject.SetActive(false);
    }
}
