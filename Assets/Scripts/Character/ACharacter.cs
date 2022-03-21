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
    protected Stat stat;
    // protected State state;
    public State state;
    protected bool canAttack;   // 공격 쿨타임

    // 목적지 및 타겟
    protected Vector3 destinationPos;
    protected GameObject target;
    protected Vector3 targetDir;      // 타겟 방향 벡터
    // 애니메이터
    protected Animator animator;    // 하위 클래스에서 할당

    private float rotationSpeed = 10f;

    // 공격속도
    protected float attackAnimSpeed;    // 하위 클래스에서 할당해야함
    
    protected virtual void Awake()
    {
        // 스탯 생성 및 Idle state
        stat = new Stat();
        state = State.Idle;
        canAttack = true;
    }

    // -------------------------------------------------------------
    // 이동
    // -------------------------------------------------------------
    public void SetDestination(Vector3 position)
    {
        destinationPos = position;
        destinationPos.y = -3f;     // temp
        state = State.Move;
    }

    protected void Move()
    {
        if (destinationPos == null || state != State.Move)
        {
            // 목적지가 없거나 Move 상태가 아닌 경우
            animator.SetBool("isMove", false);
            return;
        }

        if ((destinationPos - this.transform.position).sqrMagnitude <= 0.05f)
        {
            // 목적지에 도착하면
            state = State.Idle;
            return;
        }
        
        this.transform.position += (destinationPos - this.transform.position).normalized * Time.deltaTime * stat.MoveSpeed;
        
        // y축만 회전
        this.transform.rotation = Quaternion.Lerp(
            this.transform.rotation,
            Quaternion.LookRotation(
                new Vector3(destinationPos.x, 0f, destinationPos.z) 
                - new Vector3(this.transform.position.x, 0f, this.transform.position.z)
            ),
            Time.deltaTime * rotationSpeed
        );
        animator.SetFloat("MoveSpeed", stat.MoveSpeed * .2f);   // Animation speed = actual speed * 5
        animator.SetBool("isMove", true);
    }    

    // -------------------------------------------------------------
    // 공격
    // -------------------------------------------------------------
    public void SetTarget(GameObject gameObject)
    {
        // 타겟 지정
        target = gameObject;
        InvokeRepeating("CheckTargetInAttackRange", .1f, .1f);  // 타겟이 지정되면 일정 주기마다 동작
    }

    private void CheckTargetInAttackRange()
    {
        // 타겟이 attackRange 안에 있는지 확인
        if (target == null)
        {
            // 타겟이 없어지면 공격 범위 확인 중지
            CancelInvoke("CheckTargetInAttackRange");
            return;
        }

        targetDir = target.transform.position - this.transform.position;    // 방향 벡터 갱신
        targetDir.y = 0f;
        if (targetDir.sqrMagnitude < Mathf.Pow(stat.AttackRange, 2))
        {
            // 공격 범위 안에 들어온 경우
            if (Vector3.Angle(targetDir, this.transform.forward) < 5)
            {
                // 전방 10도(5 * 2)에 있는 경우
                // target과 가까워져도 바라보고 있지 않으면 state.Move를 유지해서 전방을 바라보게 됨
                state = State.Attack;
                
                if (canAttack)
                {
                    // 공격 쿨타임이 돌면
                    StartCoroutine("Attack");
                }
            }
            else
            {
                // 공격 사거리 안에 있는데도, 바라보고 있지 않는 경우
                state = State.Move;
            }
        }
    }

    protected IEnumerator Attack()
    {
        canAttack = false;
        float animationSpeed = 1 / stat.AttackSpeed;                    // !!! 무기별 애니메이션 속도가 다를경우 offset이 필요할 수 있음
        float actualAttackSpeed = attackAnimSpeed * stat.AttackSpeed;
        StartCoroutine("StartAttackCoolTime", actualAttackSpeed);       // 공격 쿨타임 계산
        animator.SetFloat("AttackSpeed", animationSpeed);               // 애니메이션 공격 속도 설정
        animator.SetTrigger("isAttack");
        yield return new WaitForSeconds(actualAttackSpeed * 0.5f);      // 공격 애니메이션 실행한지 50% 지나면
        target.GetComponent<ACharacter>().Attacked(CalculateDamage());  // 데미지 계산
        yield break;
    }

    protected IEnumerator StartAttackCoolTime(float attackCoolTime)
    {
        yield return new WaitForSeconds(attackCoolTime);
        canAttack = true;
    }

    private int CalculateDamage()
    {
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
    protected void Attacked(int damage)
    {
        int damageResult = (damage - stat.DamageReduction) > 0
            ? damage - stat.DamageReduction
            : 0;

        stat.DecreaseHp(damageResult);

        UpdatePlayerHpBar();
        
        if (stat.Hp < 1)
        {
            Die();
        }
    }

    protected virtual void UpdatePlayerHpBar()
    {
        // Player에서 재 선언
    }

    public virtual void Die()
    {
        // Die action
        Debug.Log("Die!");
        Destroy(this.gameObject);
    }
}
