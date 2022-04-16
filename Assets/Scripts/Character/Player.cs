using UnityEngine;
using UnityEngine.AI;

enum PlayerState
{
    Idle,
    Move,
    Attack,
    Die
}

public class Player : ACharacter
{
    public GameManager gameManager;
    private PlayerController playerController;
    
    [SerializeField]    // !!! for test
    private PlayerState state;
    private Skill[] skill;

    public Stat Stat { get { return stat; } }
    public Skill[] Skill { get { return skill; } }

    // -------------------------------------------------------------
    // Init
    // -------------------------------------------------------------
    private void Awake()
    {
        // ACharacter
        stat = new Stat();
        animator = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
        SyncStat();

        // Player
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        gameManager.UIController.InitPlayerHpBar(stat.Hp);

        playerController = GetComponent<PlayerController>();
        // // Animator load
        // attackAnimSpeed = 1.4f;
        InvokeRepeating("RecoverHp", 10f, 10f);
    }

    private void SyncStat()
    {
        // 스탯 변화시 호출
        agent.speed = stat.MoveSpeed;
        animator.SetFloat("MoveSpeed", stat.MoveSpeed * .2f);   // Animation speed = actual speed * 5
    }

    // Check
    private void Update()
    {
        CheckState();
    }

    // -------------------------------------------------------------
    // 이동, 타겟 지정
    // -------------------------------------------------------------
    public void SetDestination(Vector3 destination, bool isKeyboard = false)
    {
        // PlayerController에서 호출
        if (!isKeyboard)
        {
            agent.velocity = Vector3.zero;  // 방향 전환 시 기존 velocity 영향 X
        }
        agent.SetDestination(destination);
    }

    public void SetTarget()
    {

    }

    // -------------------------------------------------------------
    // State 전환
    // -------------------------------------------------------------
    private void CheckState()
    {
        // if (state == PlayerState.Attack && state == PlayerState.Move)
        // {
            
        //     return;
        // }
        
        if (agent.velocity.sqrMagnitude > .1f && state == PlayerState.Idle)
        {
            // 정지 상태에서 이동을 시작하면
            MoveMode();
        } 

        if (state == PlayerState.Move && agent.remainingDistance < 0.1f)
        {
            // 이동 상태에서 목적지에 도착하면
            IdleMode();
        }
    }

    private void IdleMode()
    {
        state = PlayerState.Idle;
        animator.SetBool("isMove", false);
    }

    private void MoveMode()
    {
        state = PlayerState.Move;
        animator.SetBool("isMove", true);
    }

    private void AttackMode()
    {

    }

    // -------------------------------------------------------------
    // 공격, 피격
    // -------------------------------------------------------------
    protected override void Attack()
    {

    }

    public override void Attacked()
    {

    }

    protected override void Die()
    {

    }
    // void Update()
    // {
    //     Move();             // state가 Move면 destinationPos로 이동
    //     CheckTarget();      // stat.AttackRange 안에 target이 들어오는 경우 공격
    // }



    // -------------------------------------------------------------
    // Override
    // -------------------------------------------------------------
    // protected override void MoveDone()
    // {
    //     // 이동이 끝난 뒤 move target disable
    //     playerController.ClearMoveTarget();
    // }

    // protected override void TargetDone()
    // {
    //     // CheckTarget에서 target이 null일 경우
    //     // attack target disable
    //     playerController.ClearAttackTarget();
    // }

    // protected override void UpdateHpBar()
    // {
    //     uiController.UpdatePlayerHpBar(stat.Hp, stat.MaxHp);
    // }

    // // 공격
    // protected override IEnumerator Attack(float actualAttackSpeed)
    // {
    //     canAttack = false;
    //     StartCoroutine("StartAttackCoolTime", actualAttackSpeed);       // 공격 쿨타임 계산
    //     yield return new WaitForSeconds(actualAttackSpeed * 0.5f);      // 공격 애니메이션 실행한지 50% 지나면
    //     bool isDie = target.GetComponent<ACharacter>().Attacked(CalculateDamage());  // 데미지 계산
    //     if (isDie)
    //     {
    //         // taget이 죽은 경우
    //         SetTarget(null);
    //     }
    //     state = State.Idle;     // 공격 코루틴이 끝날 때 state 변경
    //     yield break;
    // }

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

    // -------------------------------------------------------------
    // Inventory 장착 (스탯 변화)
    // -------------------------------------------------------------
    public void Equip(Item equipping)
    {
        // 스탯 수정
        stat.Equip(equipping);
        SyncStat();

        // view 수정
        gameManager.UIController.UpdateStatUI(stat);
        gameManager.UIController.UpdatePlayerHpBar(stat.Hp, stat.MaxHp);
    }

    public void UnEquip(Item unEquipping)
    {
        // 스탯 수정
        stat.UnEquip(unEquipping);
        SyncStat();

        // view 수정
        gameManager.UIController.UpdateStatUI(stat);
        gameManager.UIController.UpdatePlayerHpBar(stat.Hp, stat.MaxHp);
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

    // -------------------------------------------------------------
    // Act
    // -------------------------------------------------------------
    // public void MoveToTarget()
    // {
    //     if (target == null)
    //     {
    //         return;
    //     }

    //     SetDestination(target.transform.position);
    // }
}
