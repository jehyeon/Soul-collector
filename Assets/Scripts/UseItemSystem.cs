using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseItemSystem
{
    private GameManager gameManager;

    public UseItemSystem(GameManager gm)
    {
        gameManager = gm;
    }

    public void Use(int itemId, int slotIndex)
    {
        // 별도의 테이블로 관리 X
        // 사용 즉시 바로 소모하는 건 itemIndex를 사용하여 index 수정
        switch (itemId)
        {
            case 1615:
                // 무기 강화 주문서
                // gameManager.Inventory.StartReinforceMode(slotIndex, ItemType.Weapon);
                break;
            case 1616:
                // 방어구 강화 주문서
                // gameManager.Inventory.StartReinforceMode(slotIndex, ItemType.Armor);
                break;
            case 1620:
                // 체력 포션
                gameManager.Inventory.UpdateItemCount(slotIndex);
                gameManager.Player.Heal(50);
                break;
            case 1621:
                // 고급 체력 포션
                gameManager.Inventory.UpdateItemCount(slotIndex);
                gameManager.Player.Heal(200);
                break;
            case 1622:
                // 제작 재료 상자
                OpenRandomBox(slotIndex, 3);
                break;
            case 1623:
                // 방어구 상자 (1)
                OpenRandomBox(slotIndex, 1);
                break;
            case 1624:
                // 고급 방어구 상자
                // gameManager.Inventory.UpdateItemCountRaw(itemIndex);
                break;
            case 1625:
                // 무기 상자 (2)
                OpenRandomBox(slotIndex, 2);
                break;
            case 1626:
                // 고급 무기 상자
                // gameManager.Inventory.UpdateItemCountRaw(itemIndex);
                break;
        }
    }

    private void OpenRandomBox(int boxSlotIndex, int dropId)
    {
        // boxSlotIndex의 아이템 1개 소모 후 dropId의 랜덤 아이템 추가
        if (gameManager.Inventory.TryToUpdateItemCount(boxSlotIndex))
        {
            // 항목 삭제 후 Save and Reload
            gameManager.Inventory.Delete(boxSlotIndex);
        }
        gameManager.GetItem(GetRandomItemId(dropId));
        gameManager.SaveManager.SaveData();     // 아이템 추가는 Reload 없어도 됨
    }

    private int GetRandomItemId(int dropTableId)
    {
        return gameManager.DropManager.RandomItem(dropTableId);
    }
}
