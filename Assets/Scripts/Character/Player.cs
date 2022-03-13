using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : ACharacter
{
    public GameManager gameManager;
    private UIController ui;
    
    public Stat Stat { get { return stat; } }

    protected override void Awake()
    {
        base.Awake();

        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        ui = GetComponent<UIController>();
        ui.InitPlayerHpBar(stat.Hp);        // 체력바 설정
    }
    private void Start()
    {
        // Animator load
        animator = GetComponentInChildren<Animator>();

        // 스탯창 업데이트
        ui.UpdateStatUI(stat);
        
        InvokeRepeating("RecoverHp", 10f, 10f);
    }

    void Update()
    {
        Move();     // state가 Move면 destinationPos로 이동
    }

    protected override void UpdatePlayerHpBar()
    {
        ui.UpdatePlayerHpBar(stat.Hp, stat.MaxHp);
    }

    private void RecoverHp()
    {
        if (stat.Hp >= stat.MaxHp)
        {
            // 이미 최대 체력인 경우
            return;
        }
        
        stat.RecoverHp();
        ui.UpdatePlayerHpBar(stat.Hp, stat.MaxHp);  // 체력 자동 회복이 된 경우 ui 업데이트
    }

    public void Heal(int amount)
    {
        stat.Heal(amount);
        ui.UpdatePlayerHpBar(stat.Hp, stat.MaxHp);
    }

    // 골드
    public void GetGold(int droppedGold)
    {
        // cv.GetComponent<Inventory>().UpdateGold(droppedGold);
    }

    // 플레이어 장비 장착
    public void Equip(Item equipping)
    {
        Debug.LogFormat("{0} 장착", equipping.ItemName);
        stat.Equip(equipping);
        ui.UpdateStatUI(stat);
        ui.UpdatePlayerHpBar(stat.Hp, stat.MaxHp);
    }

    public void UnEquip(Item unEquipping)
    {
        Debug.LogFormat("{0} 장착 해제", unEquipping.ItemName);
        stat.UnEquip(unEquipping);
        ui.UpdateStatUI(stat);
        ui.UpdatePlayerHpBar(stat.Hp, stat.MaxHp);
    }

}
