using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minotaur : Enemy
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
    public int setDropId = 20;

    private void Start()
    {
        attackAnimSpeed = 3.033f;        // 공격 애니메이션 속도
        // startPos = this.transform.position;  // Enemy.Set()에서 할당
        
        // 스탯 설정
        stat.SetEnemyStat(
            minDamage, maxDamage, defaultDamage, 
            maxHp, damageReduction, 
            attackSpeed, moveSpeed, attackRange
        );
        
        dropId = setDropId;

        // Sync agent
        agent.speed = stat.MoveSpeed;
        animator.SetFloat("MoveSpeed", stat.MoveSpeed * .2f);   // Animation speed = actual speed * 5
        animator.SetFloat("AttackSpeed", attackAnimSpeed / (1f / stat.AttackSpeed));
    }
}
