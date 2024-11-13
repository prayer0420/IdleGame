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

    public float interactionRange = 2f;
    public void OnEnable()
    {
        CharacterDatabase.Instance.DataLoadComplete += HandleDataLoadComplete;
    }
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
        SetStartPosition();
        SetNextDestination();
    }

    public void OnDisable()
    {
        CharacterDatabase.Instance.DataLoadComplete -= HandleDataLoadComplete;
    }
    public void Update()
    {
        stateMachine.Update();
        CheckIfReachedExit();
    }

    public void HandleDataLoadComplete()
    {
        CharacterData data = CharacterDatabase.Instance.playerData;
        MaxHealth = data.Health;
        Health = data.Health;
        AttackPower = data.AttackPower;
        DefensePower = data.DefensePower;
    }
    public  void SetStartPosition()
    {
        GameObject entrance = GameObject.FindGameObjectWithTag("Entrance");
        if (entrance != null)
        {
            transform.position = entrance.transform.position;
            transform.rotation = entrance.transform.rotation;
        }
        else
        {
            Debug.LogError("Entrance not found in the current map.");
        }
    }

    public void SetNextDestination()
    {
        ExitPoint = GameObject.FindGameObjectWithTag("Exit");
        if (ExitPoint == null)
        {
            Debug.LogError("ExitPoint not found in the current map.");
            return;
        }

        Debug.Log($"Next ExitPoint found at: {ExitPoint.transform.position}");
    }
    private void CheckIfReachedExit()
    {
        if (ExitPoint == null)
            return;

        if (Vector3.Distance(transform.position, ExitPoint.transform.position) < 1f)
        {
            // Exit에 도달함
            MapManager.Instance.OnPlayerReachExit();
            SetNextDestination();
        }
    }
    public void AutoMove()
    {
        //목적지로 이동
        Vector3 destination = ExitPoint.transform.position;
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
            float rotationSpeed = 10f;
            transform.rotation = Quaternion.RotateTowards(transform.rotation,rotation, rotationSpeed* Time.deltaTime);
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
                Debug.Log("적 찾았다");
                //현재 타겟을 그 적으로 변경
                CurrentTarget = hit.GetComponent<EnemyController>();
                Debug.Log(CurrentTarget.name);
                return true;
            }    
        }
        //적이 아니라면 false
        return false;
    }

    public void AttackEnemy()
    { 
        if(CurrentTarget == null)
            return;

        //죽으면 내비둬
        if (CurrentTarget.IsDead)
            return;

        //공격범위내로 이동
        if(Vector3.Distance(transform.position, CurrentTarget.transform.position) > attackRange)
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
        //체력변경 알림
        OnHealthUpdate?.Invoke();
        Debug.Log($"{this.name}가 공격받았다. HP : {Health}");
    }

    public void Die()
    {
        Animator.SetTrigger("Die");
        //사망 이벤트 호출
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
}
