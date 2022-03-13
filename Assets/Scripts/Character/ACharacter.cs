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
    protected State state;
    protected bool canAttack;   // 공격 쿨타임

    // 목적지 및 타겟
    protected Vector3 destinationPos;
    protected GameObject target;

    // 애니메이터
    protected Animator animator;    // 하위 클래스에서 할당

    private float rotationSpeed = 10f;

    protected virtual void Awake()
    {
        // 스탯 생성 및 Idle state
        stat = new Stat();
        state = State.Idle;
        canAttack = true;
    }

    // 타겟 지정
    public void SetTarget(GameObject gameObject)
    {
        target = gameObject;
        state = State.Move;     // 공격 범위 안에 들어오면 state = State.Attack;
        InvokeRepeating("CheckTargetInAttackRange", .1f, .1f);  // 타겟이 지정되면 일정 주기마다 동작
    }

    public void SetDestination(Vector3 position)
    {
        destinationPos = position;
        state = State.Move;
    }


    private void CheckEnemyInAttackRange()
    {
        // 타겟이 attackRange 안에 있는지 확인
        if (target == null)
        {
            // 공격 범위 확인 중지
            CancelInvoke("CheckTargetInAttackRange");
            return;
        }

        if (!canAttack)
        {
            // 공격 쿨타임인 경우
            // !!! 함수 호출 주기에 따른 공격 딜레이가 발생할 수 있음
            return;
        }

        if ((target.transform.position - this.transform.position).sqrMagnitude < stat.AttackRange * stat.AttackRange)
        {
            // 공격 범위 안에 들어온 경우
            state = State.Attack;

            Attack();
        }
        else
        {
            // 타겟이 지정되어 있지만, 멀어진 경우
            state = State.Move;
        }
    }

    // 이동 관련
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
            // if (target == null)
            // {
            state = State.Idle;
            // }
            // else
            // {
            //     state = State.Attack;
            // }
            return;
        }
        
        this.transform.position += (destinationPos - this.transform.position).normalized * Time.deltaTime * stat.MoveSpeed;
        // temp
        this.transform.rotation = Quaternion.Lerp(
            this.transform.rotation,
            Quaternion.LookRotation(destinationPos - this.transform.position),
            Time.deltaTime * rotationSpeed
        );
        animator.SetBool("isMove", true);
    }    

    // 공격 관련
    protected void Attack()
    {
        if (target == null || state != State.Attack || canAttack)
        {
            return;
        }

        target.GetComponent<ACharacter>().Attacked(CalculateDamage());
        Invoke("StartAttackCoolTime", 0f);  // 공격 쿨타임 계산
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

    IEnumerator StartAttackCoolTime()
    {
        canAttack = false;
        yield return new WaitForSeconds(stat.AttackSpeed);
        canAttack = true;
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
