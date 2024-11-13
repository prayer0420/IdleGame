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
        
        // 적 데이터 로드
        int enemyID = 1; // 적의 ID를 설정
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
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, Time.deltaTime);
        }
    }

    public void AttackPlayer()
    {
        //죽으면 내비둬
        if (PlayerController.Instance.Health <= 0f)
            return;
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
        
    }
}
