using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : ACharacter
{
    public GameManager gameManager;     // 할당 해줘야 함
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
        // Animator load
        attackAnimSpeed = 1.4f;

        // 스탯창 업데이트
        uiController.UpdateStatUI(stat);
        
        InvokeRepeating("RecoverHp", 10f, 10f);
    }

    void Update()
    {
        Move();     // state가 Move면 destinationPos로 이동
    }

    protected override void UpdatePlayerHpBar()
    {
        uiController.UpdatePlayerHpBar(stat.Hp, stat.MaxHp);
    }

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

    // 골드
    public void GetGold(int droppedGold)
    {
        // cv.GetComponent<Inventory>().UpdateGold(droppedGold);
    }

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

}
