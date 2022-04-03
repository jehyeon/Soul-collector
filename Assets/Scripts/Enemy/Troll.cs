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

    public float tempAnimAttackSpeed;

    private void Start()
    {
        attackAnimSpeed = 2.76f;        // 공격 애니메이션 속도
        // 스탯 설정
        stat.SetEnemyStat(
            minDamage, maxDamage, defaultDamage, 
            maxHp, damageReduction, 
            attackSpeed, moveSpeed, attackRange
        );

        // Drop table Id
        dropId = 0;
    }
    
    private void Update()
    {
        CheckTarget();
        FindNearByPlayer();
        Move();
        Back();
        
        // for test
        if (Input.GetKeyDown(KeyCode.Q))
        {
            stat.SetEnemyStat(
                minDamage, maxDamage, defaultDamage, 
                maxHp, damageReduction, 
                attackSpeed, moveSpeed, attackRange
            );

            attackAnimSpeed = tempAnimAttackSpeed;
        }
    }

    public override void Reset()
    {
        base.Reset();
        Start();
    }
}
