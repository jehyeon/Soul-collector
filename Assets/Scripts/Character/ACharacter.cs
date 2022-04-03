using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum State
{
    Idle,
    Die,
    Attack,
    Move,
    Back    // for only Enemy
}

public class ACharacter : MonoBehaviour
{
    // 스탯 정보 및 현재 상태
    protected Stat stat;
    public State state;
    protected bool canAttack;   // 공격 쿨타임

    // 목적지 및 타겟
    protected Vector3 destinationPos;
    protected Vector3 destinationDir;
    protected GameObject target;
    protected Vector3 targetDir;

    // 애니메이터
    protected Animator animator;

    private float rotationSpeed = 10f;

    // 공격속도
    protected float attackAnimSpeed;    // 하위 클래스에서 할당해야함
    
    public GameObject Target { get { return target; } }

    // 데미지 text
    private DamageTextSystem damageTextSystem;

    protected virtual void Awake()
    {
        // 스탯 생성 및 Idle state
        stat = new Stat();
        state = State.Idle;
        canAttack = true;

        animator = GetComponentInChildren<Animator>();
    }

    // -------------------------------------------------------------
    // 이동
    // -------------------------------------------------------------
    public void SetDestination(Vector3 position, bool isBack = false)
    {
        destinationPos = position;
        if (isBack)
        {
            if (state != State.Back)
            {
                state = State.Back;
            }
        }
        else
        {
            if (state != State.Move)
            {
                state = State.Move;
            }
        }
        StopCoroutine("Attack");    // 공격 중이면 캔슬
    }

    protected void Move()
    {
        // Update에서 항시 동작

        if (destinationPos == null || (state != State.Move && state != State.Back))
        {
            // 목적지가 없거나 상태가 Move 혹은 Back이 아닌 경우
            animator.SetBool("isMove", false);
            return;
        }
        
        // Rotate
        Vector3 look = new Vector3(destinationPos.x, 0f, destinationPos.z) 
            - new Vector3(this.transform.position.x, 0f, this.transform.position.z);

        if (look.sqrMagnitude > float.Epsilon)
        {
            this.transform.rotation = Quaternion.Lerp(
                this.transform.rotation,
                Quaternion.LookRotation(look),
                Time.deltaTime * rotationSpeed
            );
        }

        // Move
        destinationDir = destinationPos - this.transform.position;      // 목적지 방향 벡터
        this.transform.position += destinationDir.normalized * Time.deltaTime * stat.MoveSpeed;

        animator.SetFloat("MoveSpeed", stat.MoveSpeed * .2f);   // Animation speed = actual speed * 5
        animator.SetBool("isMove", true);

        if (destinationDir.sqrMagnitude <= 0.05f)
        {
            // 목적지에 도착하면
            state = State.Idle;
            destinationPos = this.transform.position;
            MoveDone();
            return;
        }
    }    

    protected virtual void MoveDone()
    {
        // for player
    }
    // -------------------------------------------------------------
    // 공격
    // -------------------------------------------------------------
    public void SetTarget(GameObject gameObject)
    {
        // 타겟 지정
        target = gameObject;
        if (gameObject == null)
        {
            StopCoroutine("Attack");
        }
    }

    public void CheckTarget()
    {
        // 타겟이 attackRange 안에 있는지 확인
        if (target == null)
        {
            TargetDone();
            // 타겟이 있는 경우에만 확인
            return;
        }

        if (state == State.Back)
        {
            // 돌아가는 상태면 타겟 정보 계산 안함
            return;
        }

        // 타겟 방향 벡터 및 거리 계산
        targetDir = target.transform.position - this.transform.position;
        targetDir.y = 0f;

        if (targetDir.sqrMagnitude < Mathf.Pow(stat.AttackRange, 2))
        {
            // 공격 범위 안에 들어온 경우
            if (Vector3.Angle(targetDir, this.transform.forward) < 5)
            {
                // 전방 10도(5 * 2)에 있는 경우
                // target과 가까워져도 바라보고 있지 않으면 state.Move를 유지해서 전방을 바라보게 됨
                if (canAttack)
                {
                    state = State.Attack;
                    // 공격 쿨타임이 돌면
                    float actualAttackSpeed = 1 / stat.AttackSpeed;
                    float animationSpeed = attackAnimSpeed / actualAttackSpeed;      // !!! 무기별 애니메이션 속도가 다를경우 offset이 필요할 수 있음
                    animator.SetFloat("AttackSpeed", animationSpeed);               // 애니메이션 공격 속도 설정
                    animator.SetTrigger("isAttack");
                    StartCoroutine("Attack", actualAttackSpeed);
                }

                return;
            }
            else
            {
                // 공격 사거리 안에 있는데도, 바라보고 있지 않는 경우
                SetDestination(target.transform.position);
            }
        }
    }

    protected virtual void TargetDone()
    {
        // for player
    }

    protected IEnumerator Attack(float actualAttackSpeed)
    {
        canAttack = false;
        StartCoroutine("StartAttackCoolTime", actualAttackSpeed);       // 공격 쿨타임 계산
        yield return new WaitForSeconds(actualAttackSpeed * 0.5f);      // 공격 애니메이션 실행한지 50% 지나면
        bool isDie = target.GetComponent<ACharacter>().Attacked(CalculateDamage());  // 데미지 계산
        if (isDie)
        {
            // taget이 죽은 경우
            SetTarget(null);
        }
        state = State.Idle;     // 공격 코루틴이 끝날 때 state 변경
        yield break;
    }

    protected IEnumerator StartAttackCoolTime(float attackCoolTime)
    {
        yield return new WaitForSeconds(attackCoolTime);
        canAttack = true;
    }

    protected int CalculateDamage()
    {
        // 최소 데미지 ~ 최대 데미지 + 기본 데미지
        if (stat.CriticalPercent > 0)
        {
            float rand = Random.value;

            if (rand < stat.CriticalPercent * 0.01f)
            {
                // cri effect 추가해야 함
                return stat.MaxDamage + stat.DefaultDamage;
            }
        }

        return Random.Range(stat.MinDamage, stat.MaxDamage + 1) + stat.DefaultDamage;
    }

    // 피격 관련
    protected bool Attacked(int damage)
    {
        // 최종 데미지 - 데미지 리덕션 = 받는 데미지
        int damageResult = (damage - stat.DamageReduction) > 0
            ? damage - stat.DamageReduction
            : 0;

        stat.DecreaseHp(damageResult);
        // Debug.LogFormat("{0}가 {1} 데미지를 입음 -> 남은 체력 {2}", this.name, damageResult, stat.Hp);

        UpdateHpBar();
        
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

    protected virtual void UpdateHpBar()
    {
        // 하위 클래스에서 재 선언
    }

    protected virtual void Die()
    {
        // 하위 클래스에서 재 선언
    }
}
