using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : ACharacter
{
    private GameManager gameManager;
    private UIController ui;

    void Start()
    {
        // 체력바 설정
        ui = GetComponent<UIController>();
        ui.InitPlayerHpBar(stat.Hp);
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
    }

    // 골드
    public void GetGold(int droppedGold)
    {
        // cv.GetComponent<Inventory>().UpdateGold(droppedGold);
    }
}
