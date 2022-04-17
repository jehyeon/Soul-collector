using System.Collections;
using UnityEngine;
using UnityEngine.AI;

enum EnemyState
{
    Idle,
    Attack,
    Back
}

public class Enemy : ACharacter
{
    private Spawner spawner;
    private int enemyId;
    protected int dropId;           // 자식 클래스에서 재 할당
    protected Vector3 startPos;
    protected Vector3 startPosDir;

    private EnemyState state;
    private int findRange;      // 플레이어 감지 범위 (default)
    private int backRange;      // 스폰 위치로부터의 제한 거리 (default)
    private float wakeUpDelay;  // wake up 시 딜레이

    public Spawner ParentSpawner { get { return spawner; } }
    public int DropId { get { return dropId; } }

    [SerializeField]
    protected EnemyEffectSound sound;

    // Hp bar
    private EnemyHpBar hpBar;

    // -------------------------------------------------------------
    // Init
    // -------------------------------------------------------------
    protected virtual void Awake()
    {
        // ACharacter
        stat = new Stat();
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        canAttack = true;

        // Enemy
        findRange = 5;
        backRange = 15;
        wakeUpDelay = 2f;
    }

    private void Update()
    {
        FindNearByPlayer();
    }

    public void Set(Spawner parentSpawner, GameObject target)
    {
        stat.Heal(99999);
        spawner = parentSpawner;
        startPos = this.transform.position;
        SetTarget(target);
    }

    // -------------------------------------------------------------
    // State 전환
    // -------------------------------------------------------------
    private void FindNearByPlayer()
    {
        if (target == null)
        {
            // 타겟이 지정 안된 경우 (error)
            Debug.Log("스포너로부터 생성된 enemy가 아닙니다.");
            target = GameObject.Find("Player");
        }

        targetDir = target.transform.position - this.transform.position;
        startPosDir = startPos - this.transform.position;

        if (state == EnemyState.Back)
        {
            // 시작 위치로 돌아가는 중일 때
            if (agent.remainingDistance < 0.2f)
            {
                // 목적지에 도착한 경우
                IdleMode();
            }
            else
            {
                // 경로 재 검색
                agent.SetDestination(startPos);
            }
        }

        if (state == EnemyState.Idle)
        {
            if (targetDir.sqrMagnitude < Mathf.Pow(findRange, 2))
            {
                // 플레이어를 찾기 전

                // StartCoroutine(AttackMode()); // !!! temp
                state = EnemyState.Attack;
                animator.SetBool("isMove", true);
            }
        }

        if (state == EnemyState.Attack)
        {
            agent.SetDestination(target.transform.position);

            if (targetDir.sqrMagnitude <= Mathf.Pow(stat.AttackRange, 2))
            {
                // 공격 범위 안에 들어오면 공격
                ReadyToAttack();
            }
            else
            {
                AgainMove();
                StopCoroutine("Attack");    // 이동 중이면 코루틴 중지
            }

            if (startPosDir.sqrMagnitude > Mathf.Pow(backRange, 2))
            {
                // 스폰 위치로부터 멀리 떨어지면 돌아감
                BackMode();
            }
        }
    }

    private IEnumerator AttackMode()
    {
        // animator.SetTrigger("FindTarget");
        // -> !!! 느낌표 ui 추가
        yield return new WaitForSeconds(wakeUpDelay);
        state = EnemyState.Attack;
        animator.SetBool("isMove", true);
        // agent.SetDestination(target.transform.position);
    }

    private void BackMode()
    {
        state = EnemyState.Back;
        stat.Heal(true);
        // agent.SetDestination(startPos);
    }

    private void IdleMode()
    {
        state = EnemyState.Idle;
        animator.SetBool("isMove", false);
    }

    private void StopMove()
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

    // -------------------------------------------------------------
    // 공격, 피격
    // -------------------------------------------------------------
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

    protected override IEnumerator Attack()
    {
        canAttack = false;
        animator.SetTrigger("isAttack");
        Invoke("AttackCooltime", 1f / stat.AttackSpeed);     // 공격 쿨타임
        yield return new WaitForSeconds(1f / stat.AttackSpeed * 0.5f);      // 공격 애니메이션 실행한지 50% 지나면
        target.GetComponent<ACharacter>().Attacked(stat.CalculateAttackDamage());  // 데미지 계산
        yield break;
    }

    private void AttackCooltime()
    {
        canAttack = true;
    }

    public override bool Attacked(int damage)
    {
        stat.TakeDamage(damage);


        sound.PlayAttackedSound();
        
        if (damageTextSystem == null)
        {
            damageTextSystem = GameObject.Find("Damage Text System").GetComponent<DamageTextSystem>();
        }
        damageTextSystem.FloatDamageText(damage, this.transform.position);

        if (stat.Hp < 1)
        {
            Die();
            return true;
        }

        return false;
    }

    protected override void Die()
    {
        hpBar.Return();
        hpBar = null;
        spawner.Die(this);
    }

    protected void UpdateHpBar()
    {
        if (hpBar == null)
        {
            // init
            hpBar = spawner.GameManager.InitHpBar();
            hpBar.SetTransform(this.transform);
        }

        // update
        hpBar.UpdateHpBar((float)stat.Hp / stat.MaxHp);
    }
}
