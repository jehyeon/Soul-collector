using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ACharacter : MonoBehaviour
{
    public enum State
    {
        Idle,
        Die,
        Attack,
        Move,   // for only Player
        Back    // for only Enemy
    }
    public Stat _stat;
    public State state;

    // 공격
    private float attackCoolTime;
    protected bool canAttack;

    // 이동
    public Vector3 destinationPos;

    private void Start()
    {
        _stat = gameObject.GetComponent<Stat>();
        state = State.Idle;
        attackCoolTime = 1f;
        canAttack = true;
    }

    protected void CheckCanAttack()
    {
        // 공격 가능한지 확인
        if (!canAttack)
        {
            attackCoolTime += Time.deltaTime;

            if (attackCoolTime > _stat.AttackSpeed)
            {
                canAttack = true;
            }
        }
    }

    protected void Attack(GameObject targetObject)
    {
        int calculedDamage = CalculDamage();

        Debug.Log(gameObject.name + "이 " + targetObject.name + "을 " + calculedDamage + " 데미지로 공격");
        
        // 공격 대상이 Player인지 Enemy인지 확인
        Player isPlayer = targetObject.GetComponent<Player>();
        if (isPlayer == null)
        {
            targetObject.GetComponent<Enemy>().Attacked(calculedDamage);    
        }
        else
        {
            targetObject.GetComponent<Player>().Attacked(calculedDamage);
        }

        // 공격 쿨타임 초기화
        canAttack = false;
        attackCoolTime = 0;
    }

    protected void Attacked(int damage)
    {
        _stat.Attacked(damage);

        Debug.Log(gameObject.name + "의 체력이 " + damage + "만큼 감소하여 현재 체력이 " + _stat.Hp);

        if (_stat.Hp < 1)
        {
            Die();
        }
    }

    protected void Move()
    {
        if (state == State.Move || state == State.Back)
        {
            if (Vector3.Distance(destinationPos, this.transform.position) <= 0.1f)
            {
                // 목적지에 도착하면
                state = State.Idle;
                return;
            }
            
            Vector3 dir = destinationPos - this.transform.position;

            this.transform.position += dir.normalized * Time.deltaTime * _stat.MoveSpeed;
        }
    }    

    protected void Die()
    {
        // Die action
        Debug.Log("Die!");
        Destroy(this.gameObject);
    }

    private int CalculDamage()
    {
        // 스탯 기반 데미지로 수정 예정
        return 5;
    }
}
