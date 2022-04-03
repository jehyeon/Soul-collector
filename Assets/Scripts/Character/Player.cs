using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : ACharacter
{
    public GameManager gameManager;     // 할당 해줘야 함
    private PlayerController playerController;
    [SerializeField]
    private UIController uiController;
    
    public Stat Stat { get { return stat; } }

    protected override void Awake()
    {
        base.Awake();
        uiController.InitPlayerHpBar(stat.Hp);        // 체력바 설정
    }
    private void Start()
    {
        playerController = GetComponent<PlayerController>();

        // Animator load
        attackAnimSpeed = 1.4f;

        // 스탯창 업데이트
        uiController.UpdateStatUI(stat);
        
        InvokeRepeating("RecoverHp", 10f, 10f);
    }

    void Update()
    {
        Move();             // state가 Move면 destinationPos로 이동
        CheckTarget();      // stat.AttackRange 안에 target이 들어오는 경우 공격
    }


    // -------------------------------------------------------------
    // Override
    // -------------------------------------------------------------
    protected override void MoveDone()
    {
        // 이동이 끝난 뒤 move target disable
        playerController.ClearMoveTarget();
    }

    protected override void TargetDone()
    {
        // CheckTarget에서 target이 null일 경우
        // attack target disable
        playerController.ClearAttackTarget();
    }

    protected override void UpdatePlayerHpBar()
    {
        uiController.UpdatePlayerHpBar(stat.Hp, stat.MaxHp);
    }

    // -------------------------------------------------------------
    // 체력 회복 (스탯 변화)
    // -------------------------------------------------------------
    private void RecoverHp()
    {
        if (stat.Hp >= stat.MaxHp)
        {
            // 이미 최대 체력인 경우
            return;
        }
        
        stat.RecoverHp();
        uiController.UpdatePlayerHpBar(stat.Hp, stat.MaxHp);  // 체력 자동 회복이 된 경우 ui 업데이트
    }

    public void Heal(int amount)
    {
        stat.Heal(amount);
        uiController.UpdatePlayerHpBar(stat.Hp, stat.MaxHp);
    }

    // -------------------------------------------------------------
    // Inventory 장착 (스탯 변화)
    // -------------------------------------------------------------
    // 플레이어 장비 장착
    public void Equip(Item equipping)
    {
        Debug.LogFormat("{0} 장착", equipping.ItemName); // !!! for test
        stat.Equip(equipping);
        uiController.UpdateStatUI(stat);
        uiController.UpdatePlayerHpBar(stat.Hp, stat.MaxHp);
    }

    public void UnEquip(Item unEquipping)
    {
        Debug.LogFormat("{0} 장착 해제", unEquipping.ItemName); // !!! for test
        stat.UnEquip(unEquipping);
        uiController.UpdateStatUI(stat);
        uiController.UpdatePlayerHpBar(stat.Hp, stat.MaxHp);
    }

    // -------------------------------------------------------------
    // Act
    // -------------------------------------------------------------
    public void MoveToTarget()
    {
        if (target == null)
        {
            return;
        }

        SetDestination(target.transform.position);
    }
}
