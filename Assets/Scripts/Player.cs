using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : ACharacter
{
    public GameObject targetEnemy;

    public Canvas cv;

    [SerializeField]
    private Slider hpBar;

    void Start()
    {
        hpBar.maxValue = _stat.Hp;
    }

    void Update()
    {
        CheckEnemyInAttackRange();

        // 공격 쿨타임 계산
        CheckCanAttack();

        // state가 Move면 destinationPos로 이동
        Move();

        // UI
        UpdateHpBar();
    }

    private void CheckEnemyInAttackRange()
    {
        if (targetEnemy == null)
        {
            return;
        }

        if (Vector3.Distance(targetEnemy.transform.position, this.transform.position) <= 1.1f)
        {
            // state를 Attack으로 수정하여 플레이어가 움직이지 않도록 함
            state = State.Attack;

            // 공격 범위는 고정 (나중에 수정)
            if (canAttack)
            {
                // 공격 쿨타임 돌면 Attack()
                Attack(targetEnemy);
            }
        }
    }

    private void UpdateHpBar()
    {
        hpBar.value = _stat.Hp;
    }
}
