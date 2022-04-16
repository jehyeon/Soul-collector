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

        // Enemy
        findRange = 5;
        backRange = 15;
        wakeUpDelay = 2f;
    }

    private void Update()
    {
        FindNearByPlayer();
    }

    // -------------------------------------------------------------
    // State 전환
    // -------------------------------------------------------------
    private void FindNearByPlayer()
    {
        Debug.Log(state);
        if (target == null)
        {
            // 타겟이 지정 안된 경우 (error)
            Debug.Log("스포너로부터 생성된 enemy가 아닙니다.");
            target = GameObject.Find("Player").GetComponent<ACharacter>();
        }

        if (state == EnemyState.Back)
        {
            // 시작 위치로 돌아가는 중일 때
            if (agent.remainingDistance < 0.1f)
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

        Vector3 targetDir = target.transform.position - this.transform.position;
        Vector3 startPosDir = startPos - this.transform.position;

        if (state == EnemyState.Idle)
        {
            if (targetDir.sqrMagnitude < Mathf.Pow(findRange, 2))
            {
                // 플레이어를 찾기 전
                StartCoroutine(AttackMode());
            }
        }

        if (state == EnemyState.Attack)
        {
            agent.SetDestination(target.transform.position);

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
    //     hpBar.Return();
    //     hpBar = null;
    //     spawner.Die(this);
    }    
    // public void Set(Spawner parentSpawner, GameObject target)
    // {
    //     stat.Heal(99999);
    //     spawner = parentSpawner;
    //     startPos = this.transform.position;
    //     SetTarget(target);
    // }

    // protected void FindNearByPlayer()
    // {
    //     if (state == State.Attack || state == State.Back)
    //     {
    //         // 공격 중이거나 되돌아가는 상태일 때에는 타겟을 찾지 않음
    //         return;
    //     }
        
    //     if (!isWakeUp && targetDir.sqrMagnitude < Mathf.Pow(findRange, 2))
    //     {
    //         // isWakeUp: false 일 때 findRange 안에만 들어오면 움직이기 시작
    //         isWakeUp = true;
    //         SetDestination(target.transform.position);
    //         return;
    //     }

    //     if (isWakeUp && targetDir.sqrMagnitude > Mathf.Pow(stat.AttackRange, 2))
    //     {
    //         // isWakeUp: true인 경우 공격 사거리 밖일 때만 플레이어에게 이동
    //         SetDestination(target.transform.position);
    //     }
    // }

    // protected void Back()
    // {
    //     // startPos에서 멀어지면 최대 체력이 되고 처음 위치로 돌아감
    //     Vector3 outDistance = this.transform.position - startPos;

    //     if (outDistance.sqrMagnitude > Mathf.Pow(backRange, 2) && state != State.Back)
    //     {
    //         // 돌아가는 상태가 아닌데 시작 지점에서 멀어진 경우
    //         SetDestination(startPos, true);     // state 변경 및 시작 위치로 이동 state = Back
    //         isWakeUp = false;
    //     }

    //     if (state == State.Back)
    //     {
    //         // 돌아가는 상태
    //         stat.Heal(1);     // 호출 주기마다 1씩 체력 회복
    //         UpdateHpBar();

    //         if (outDistance.sqrMagnitude < 0.05f)
    //         {
    //             // 시작 지점으로 돌아오면
    //             state = State.Idle;
    //             hpBar.Return();
    //             hpBar = null;
    //         }
    //     }
    // }

    // public virtual void Reset()
    // {
    //     isWakeUp = false;
    //     // Debug.LogError("Reset: 자식 클래스에서 오버라이드 되지 않음");
    // }

    // protected override void UpdateHpBar()
    // {
    //     if (hpBar == null)
    //     {
    //         // init
    //         hpBar = spawner.GameManager.InitHpBar();
    //         hpBar.SetTransform(this.transform);
    //     }

    //     // update
    //     hpBar.UpdateHpBar((float)stat.Hp / stat.MaxHp);
    // }

    // protected override void PlayAttackedSound()
    // {
    //     sound.PlayAttackedSound();
    // }
}
