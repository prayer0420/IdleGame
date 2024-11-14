using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;
    public Animator Animator { get; private set; }
    public CharacterController CharacterController { get; private set; }
    public EnemyController CurrentTarget { get; set; }

    public float moveSpeed = 2f;
    public float attackRange = 1.5f;
    public float detectRange = 3f;
    public float MaxHealth;

    [SerializeField] public float Health { get; set; }
    public float AttackPower { get; set; }
    public float DefensePower { get; set; }

    public Item EquippedWeapon { get; set; }
    public Item EquippedArmor { get; set; }

    [field: SerializeField] public GameObject ExitPoint { get; set; }
    private PlayerStateMachine stateMachine;

    public Action OnDeath;
    public Action OnHealthUpdate;

    public float interactionRange = 2f;

    public float gravity = -9.81f;
    private Vector3 velocity;
    private bool isGrounded;
    public float groundCheckDistance = .4f;
    public LayerMask groundMask;

    public Transform groundCheck;

    public Transform equipPosition;


    private bool hasReachedExit = false;


    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        Animator = GetComponent<Animator>();
        CharacterController = GetComponent<CharacterController>();
    }

    public void Start()
    {
        CharacterData data = CharacterDatabase.Instance.playerData;
        if (data != null)
        {
            MaxHealth = data.Health;
            Health = data.Health;
            AttackPower = data.AttackPower;
            DefensePower = data.DefensePower;
        }

        // 상태 머신 초기화
        stateMachine = new PlayerStateMachine(this);
        stateMachine.ChangeState(stateMachine.MoveState);
        SetStartPosition();
        SetNextDestination();

    }

    public void Update()
    {
        HandleGravity();
        stateMachine.Update();
        //CheckForItemInteraction();
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
    public void SetStartPosition()
    {
        GameObject entrance = GameObject.FindGameObjectWithTag("Entrance");
        if (entrance != null)
        {
            transform.position = entrance.transform.position;
            transform.rotation = entrance.transform.rotation;
        }
    }

    public void OnPlayerReachExit()
    {
        if (!hasReachedExit)
        {
            hasReachedExit = true;
            MapManager.Instance.OnPlayerReachExit();
            SetNextDestination();
        }
        hasReachedExit = false;
    }



    public void SetNextDestination()
    {
        ExitPoint = GameObject.FindGameObjectWithTag("Exit");
        if (ExitPoint == null)
        {
            return;
        }
    }

    public void AutoMove()
    {
        // 목적지로 이동
        Vector3 destination = ExitPoint.transform.position;
        Vector3 direction = (destination - transform.position).normalized;
        CharacterController.Move(direction * moveSpeed * Time.deltaTime);

        // 목적지에 도달하면 다음 목적지 설정
        if (Vector3.Distance(transform.position, destination) < 0.1f)
        {
            SetNextDestination();
        }
        // 방향 회전
        if (direction != Vector3.zero)
        {
            Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);
            float rotationSpeed = 300f;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
        }
    }

    public bool IsEnemyInRange()
    {
        // 주변에 적이 있는지 탐색
        Collider[] hits = Physics.OverlapSphere(transform.position, detectRange);

        foreach (var hit in hits)
        {
            // 만약 적이라면
            if (hit.CompareTag("Enemy"))
            {
                EnemyController enemy = hit.GetComponent<EnemyController>();
                if (enemy != null && !enemy.IsDead)
                {
                    Debug.Log("적 찾았다");
                    // 현재 타겟을 그 적으로 변경
                    CurrentTarget = enemy;
                    return true;
                }
            }
        }
        // 적이 없다면 false
        return false;
    }

    public void AttackEnemy()
    {
        if (CurrentTarget == null)
            return;

        // 죽으면 내비둬
        if (CurrentTarget.IsDead)
            return;

        // 공격범위 내로 이동
        if (Vector3.Distance(transform.position, CurrentTarget.transform.position) > attackRange)
        {
            Vector3 direction = (CurrentTarget.transform.position - transform.position).normalized;
            CharacterController.Move(direction * moveSpeed * Time.deltaTime);
        }
        else
        {
            Debug.Log("공격");
            CurrentTarget.TakeDamage(10f);
        }

        // 방향 회전
        Vector3 targetDirection = (CurrentTarget.transform.position - transform.position).normalized;
        if (targetDirection != Vector3.zero)
        {
            Debug.Log("적 바라보기");
            Quaternion toRotation = Quaternion.LookRotation(targetDirection, Vector3.up);
            float rotationSpeed = 300f;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }
    }

    public void TakeDamage(float damage)
    {
        Health -= damage;
        if (Health <= 0f)
        {
            Die();
        }
        // 체력 변경 알림
        OnHealthUpdate?.Invoke();
    }

    public void Die()
    {
        Animator.SetTrigger("Die");
        // 사망 이벤트 호출
        OnDeath?.Invoke();
    }

    public void CheckForItemInteraction()
    {
        Ray ray = new Ray(transform.position + Vector3.up, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionRange))
        {
            if (hit.collider.CompareTag("Item"))
            {
                ItemPickup itemPickup = hit.collider.GetComponent<ItemPickup>();
                if (itemPickup != null)
                {
                    // 아이템 설명 표시
                    UIManager.instance.ShowItemDescription(itemPickup.item);

                    // 아이템 획득
                    Inventory.Instance.AddItem(itemPickup.item);

                    // 아이템 오브젝트 제거
                    Destroy(hit.collider.gameObject);
                }
            }
        }
    }

    public void ToggleCursor(bool isVisible)
    {
        if (isVisible)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
