using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Troll : Enemy
{
    // for test
    public int minDamage;
    public int maxDamage;
    public int defaultDamage;
    public int maxHp;
    public int damageReduction;
    public float attackSpeed;
    public float moveSpeed;
    public float attackRange;

    private void Start()
    {
        attackAnimSpeed = 2.76f;        // 공격 애니메이션 속도
        // startPos = this.transform.position;  // Enemy.Set()에서 할당
        
        // 스탯 설정
        stat.SetEnemyStat(
            minDamage, maxDamage, defaultDamage, 
            maxHp, damageReduction, 
            attackSpeed, moveSpeed, attackRange
        );

        // Drop table Id
        dropId = 0;
        
        // Sync agent
        agent.speed = stat.MoveSpeed;
        animator.SetFloat("MoveSpeed", stat.MoveSpeed * .2f);   // Animation speed = actual speed * 5
        animator.SetFloat("AttackSpeed", attackAnimSpeed / (1f / stat.AttackSpeed));
    }
}
