using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Player : ACharacter
{
    public GameObject targetEnemy;

    public Canvas cv;

    [SerializeField]
    private Slider hpBar;

    [SerializeField]
    private TextMeshProUGUI hpBarText;      // Text Hp

    // 방어
    private float hpRecoveryCoolTime;

    protected override void Awake()
    {
        base.Awake();
        hpRecoveryCoolTime = 0f;
    }

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

        // Hp 자동회복
        RecoveryHp();

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
        hpBar.value = (float)_stat.Hp / (float)_stat.MaxHp * 100f;
        hpBarText.text = _stat.Hp + "/" + _stat.MaxHp;
        // , 추가하기
    }

    private void RecoveryHp()
    {
        if (_stat.Hp >= _stat.MaxHp)
        {
            return;
        }
        hpRecoveryCoolTime += Time.deltaTime;

        if (hpRecoveryCoolTime > 10)
        {
            hpRecoveryCoolTime = 0f;
            _stat.RecoveryHp();
            // Debug.Log("HP 자동회복" + _stat.HpRecovery);
        }
    }

    public void Heal(int amount)
    {
        _stat.Heal(amount);
    }

    // 골드
    public void GetGold(int droppedGold)
    {
        cv.GetComponent<Inventory>().UpdateGold(droppedGold);
    }
}
