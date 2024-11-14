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

    public MapController currentMap; // 현재 맵 정보를 저장

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
        // 적 데이터 로드
        int enemyID = 1; // 적의 ID를 설정하거나 프리팹에서 설정할 수 있도록 변경
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
            velocity.y = -2f; // 약간의 아래로 힘을 주어 Grounded 상태를 유지
        }

        velocity.y += gravity * Time.deltaTime;
        CharacterController.Move(velocity * Time.deltaTime);
    }

    public bool CanSeePlayer()
    {
        //플레이어와의 거리 확인
        float distance = Vector3.Distance(transform.position, playerTransform.position);
        //플레이어의 거리가 시야거리보다 작다면
        return distance <= sightRange;
    }

    public bool IsPlayerInAttackRange()
    {
        //공격범위내에 있는지 확인
        float distance = Vector3.Distance(transform.position, playerTransform.position);
        return distance <= attackRange;
    }

    public void MoveTowardsPlayer()
    {
        //플레이어를 향해 이동
        Vector3 direction = (playerTransform.position - transform.position).normalized;
        CharacterController.Move(direction * moveSpeed * Time.deltaTime);

        //방향 회전
        if(direction != Vector3.zero)
        {
            Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);
            float rotationSpeed = 10f;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, rotationSpeed*Time.deltaTime);
        }
    }

    public void AttackPlayer()
    {
        Debug.Log("플레이어를 공격");
        //죽으면 내비둬
        if (PlayerController.Instance.Health <= 0f)
        {
            Debug.Log(PlayerController.Instance.Health);
            return;
        }
        //플레이어에게 데미지 입히기
        PlayerController.Instance.TakeDamage(5f);
    }

    public void TakeDamage(float damage)
    {
        Health -= damage;
        if (IsDead)
        {
            Die();
        }
        Debug.Log($"{this.name}가 공격받았다. HP : {Health}");
    }

    private void Die()
    {
        Animator.SetTrigger("Die");
        // 현재 맵에 몬스터 사망을 알림
        // 사망 애니메이션 후 처리
        StartCoroutine(HandleDeath());
        currentMap.OnMonsterDeath(gameObject);
    }

    private IEnumerator HandleDeath()
    {
        // 사망 애니메이션의 길이만큼 대기
        yield return new WaitForSeconds(2f); 
        gameObject.SetActive(false);
    }

}
