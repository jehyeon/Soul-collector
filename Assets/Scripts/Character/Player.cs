using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

enum PlayerState
{
    Idle,
    Move,
    Attack,
    Drop,
    Die
}

public class Player : ACharacter
{
    public GameManager gameManager;
    private PlayerController playerController;

    private Vector3 destinationPos;
    
    [SerializeField]    // !!! for test
    private PlayerState state;

    public bool invincibility;

    public List<AttackedPassiveSkill> attackedPassiveSkills;

    public Stat Stat { get { return stat; } }

    public NavMeshAgent PlayerAgent { get { return agent; } }
    private float noVeloMoveTime;   // move State인데 움직이지 않는 시간

    // -------------------------------------------------------------
    // Init
    // -------------------------------------------------------------
    private void Awake()
    {
        // ACharacter
        stat = new Stat();
        animator = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
        attackAnimSpeed = 1.4f;
        canAttack = true;
        invincibility = false;
        SyncStat();

        // Player
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        gameManager.UIController.InitPlayerHpBar(stat.Hp);

        playerController = GetComponent<PlayerController>();
        InvokeRepeating("RecoverHp", 10f, 10f);

        noVeloMoveTime = 0f;

        // Skill
        attackedPassiveSkills = new List<AttackedPassiveSkill>();
    }

    private void SyncStat()
    {
        // 스탯 변화시 호출
        agent.speed = stat.MoveSpeed;
        animator.SetFloat("MoveSpeed", stat.MoveSpeed * .2f);   // Animation speed = actual speed * 5
        animator.SetFloat("AttackSpeed", attackAnimSpeed / (1f / stat.AttackSpeed));
    }

    // Check
    private void Update()
    {
        CheckState();
    }

    // -------------------------------------------------------------
    // 이동, 타겟 지정
    // -------------------------------------------------------------
    public void Move(Vector3 destination, bool isKeyboard = false)
    {
        // PlayerController에서 호출
        // if (!isKeyboard)
        // {
        //     agent.velocity = Vector3.zero;  // 방향 전환 시 기존 velocity 영향 X
        // }

        destinationPos = destination;
        MoveMode();
        agent.isStopped = false;
        agent.SetDestination(destination);

        StopCoroutine("Attack");    // 이동 중이면 코루틴 중지
    }

    public void StopMove()
    {
        // attacking일 때 정지 시 사용
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        animator.SetBool("isMove", false);
    }

    private void AgainMove()
    {
        agent.isStopped = false;
        animator.SetBool("isMove", true);
    }

    public void MoveToItem(GameObject item)
    {
        DropMode();
        SetTarget(item);
        agent.SetDestination(target.transform.position);
    }

    // -------------------------------------------------------------
    // State 전환
    // -------------------------------------------------------------
    private void CheckState()
    {
        if (state == PlayerState.Attack)
        {
            if (target == null)
            {
                // 타겟이 없어지면 Idle 모드
                IdleMode();
                return;
            }

            targetDir = target.transform.position - this.transform.position;

            // 공격 상태이면 target의 위치로 이동
            agent.SetDestination(target.transform.position);

            if (targetDir.sqrMagnitude <= Mathf.Pow(stat.AttackRange, 2))
            {
                // 공격 범위 안에 들어오면 공격
                ReadyToAttack();
            }
            else
            {
                AgainMove();
            }

            return;
        }

        if (state == PlayerState.Drop)
        {
            if (target == null)
            {
                // 타겟이 없어지면 Idle 모드
                IdleMode();
                return;
            }
            
            AgainMove();
            targetDir = target.transform.position - this.transform.position;
            
            if (targetDir.sqrMagnitude <= Mathf.Pow(playerController.GetItemRange, 2))
            {
                DroppedItem item = target.GetComponent<DroppedItem>();
                if (gameManager.GetItemCheckInventory(item.Item.Id))
                {
                    // 인벤토리에 빈 슬롯이 있는 경우
                    item.Return();   // 아이템 return
                    gameManager.UIController.PlayGetItemSound();
                }

                IdleMode();
            }
            
            return;
        }

        if (state == PlayerState.Move)
        {
            if (agent.velocity.sqrMagnitude == 0f)
            {
                noVeloMoveTime += Time.deltaTime;

                if (noVeloMoveTime > 0.05f)
                {
                    // 0.05초 이상 제자리 걸음인 경우
                    IdleMode();
                    noVeloMoveTime = 0f;
                }
            }
            else
            {
                noVeloMoveTime = 0f;
            }

            agent.SetDestination(destinationPos);

            if ((destinationPos - this.transform.position).sqrMagnitude <= 0.1f)
            {
                // 이동 상태에서 목적지에 도착하면
                IdleMode();
            }
        }
    }

    public void IdleMode()
    {
        state = PlayerState.Idle;
        StopMove();
        animator.SetBool("isMove", false);
    }

    private void MoveMode()
    {
        state = PlayerState.Move;
        animator.SetBool("isMove", true);
    }

    private void AttackMode()
    {
        state = PlayerState.Attack;
        animator.SetBool("isMove", true);
    }

    private void DropMode()
    {
        state = PlayerState.Drop;
        animator.SetBool("isMove", true);
    }

    // -------------------------------------------------------------
    // 공격, 피격
    // -------------------------------------------------------------
    public void AttackTarget()
    {
        // 지정된 타겟을 공격
        if (target == null)
        {
            // 타겟이 없는 경우
            return;
        }

        AttackMode();
    }
    public void AttackTarget(GameObject target)
    {
        SetTarget(target);
        AttackTarget();
    }

    private void ReadyToAttack()
    {
        StopMove();

        // 타겟을 바라보고 있지 않은 경우
        if (Vector3.Angle(target.transform.position - this.transform.position, this.transform.forward) > 5)
        {
            // 전방 10도(5 * 2) 밖에 있는 경우
            Rotate(targetDir);
        }

        if (canAttack)
        {
            StartCoroutine("Attack");
        }
    }

    // // 공격
    protected override IEnumerator Attack()
    {
        canAttack = false;
        animator.SetTrigger("isAttack");
        Invoke("AttackCooltime", 1f / stat.AttackSpeed);     // 공격 쿨타임
        yield return new WaitForSeconds(1f / stat.AttackSpeed * 0.5f);      // 공격 애니메이션 실행한지 50% 지나면
        bool isDie = target.GetComponent<ACharacter>().Attacked(stat.CalculateAttackDamage());  // 데미지 계산
        if (isDie)
        {
            // taget이 죽은 경우
            SetTarget(null);
            gameManager.SaveManager.Save.KillCount += 1;
        }
        yield break;
    }

    private void AttackCooltime()
    {
        canAttack = true;
    }

    public override bool Attacked(int damage)
    {
        if (invincibility)
        {
            damage = 0;
        }
        int real = stat.TakeDamage(damage);    // 받은 피해 적용 (방어도 계산)
        gameManager.UIController.UpdatePlayerHpBar(stat.Hp, stat.MaxHp);    // view 업데이트

        if (damageTextSystem == null)
        {
            damageTextSystem = GameObject.Find("Damage Text System").GetComponent<DamageTextSystem>();
        }
        damageTextSystem.FloatDamageText(real, this.transform.position);

        if (attackedPassiveSkills.Count > 0)
        {
            // 활성화된 스킬이 있는 경우
            foreach (AttackedPassiveSkill skill in attackedPassiveSkills)
            {
                float temp = Random.value;
                if (temp < skill.Percent)
                {
                    skill.Activate();
                }
            }
        }

        if (stat.Hp < 1)
        {
            Die();
            return true;
        }

        return false;
    }

    protected override void Die()
    {
        Time.timeScale = 0;
        gameManager.PopupMessage("플레이어가 사망했습니다.\n5초 뒤 마을에서 리스폰합니다.", 5f);
        StartCoroutine("PlayerDie");
    }

    IEnumerator PlayerDie()
    {
        yield return new WaitForSecondsRealtime(5f);
        Time.timeScale = 1;
        gameManager.PopupMessageClose();
        gameManager.GoViliage();
        yield break;
    }

    // -------------------------------------------------------------
    // 체력 회복
    // -------------------------------------------------------------
    private void RecoverHp()
    {
        if (stat.Hp >= stat.MaxHp)
        {
            // 이미 최대 체력인 경우
            return;
        }
        
        stat.RecoverHp();
        // 체력 자동 회복이 된 경우 ui 업데이트
        gameManager.UIController.UpdatePlayerHpBar(stat.Hp, stat.MaxHp);
    }

    public void Heal(int amount)
    {
        stat.Heal(amount);
        gameManager.UIController.UpdatePlayerHpBar(stat.Hp, stat.MaxHp);
    }

    public void Heal(bool maxHeal)
    {
        stat.Heal(9999);
        gameManager.UIController.UpdatePlayerHpBar(stat.Hp, stat.MaxHp);
    }

    // -------------------------------------------------------------
    // Inventory 장착 (스탯 변화)
    // -------------------------------------------------------------
    public void Equip(Item equipping)
    {
        // 스탯 수정
        stat.Equip(equipping);
        SyncStat();

        // view 수정
        gameManager.UIController.UpdateStatUI();
    }

    public void UnEquip(Item unEquipping)
    {
        // 스탯 수정
        stat.UnEquip(unEquipping);
        SyncStat();

        // view 수정
        gameManager.UIController.UpdateStatUI();
    }

    // -------------------------------------------------------------
    // Skill 활성화
    // -------------------------------------------------------------
    public void ActivateSkill(Skill skill)
    {
        switch (skill.Type)
        {
            case SkillType.Passive:
                break;
            case SkillType.Active:
                break;
            case SkillType.PercentPassive:
                break;
        }
    }

    public void AddAttackedPassiveSkill(AttackedPassiveSkill skill)
    {
        attackedPassiveSkills.Add(skill);
    }
}
