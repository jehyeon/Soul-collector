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
            case 13:
            case 14:
            case 38:
            case 39:
                // 무기 강화 주문서
                // 방어구 강화 주문서
                // 빛나는 무기 강화 주문서
                // 빛나는 방어구 강화 주문서
                StartReinforce(slotIndex);
                break;
            case 12:
                // 빈 주문서
                OpenRandomBox(slotIndex, 17);
                break;
            case 17:
                // 제작 재료 상자
                OpenRandomBox(slotIndex, 16);
                break;
            case 18:
                // 무기 상자
                OpenRandomBox(slotIndex, 18);
                break;
            case 19:
                // 방어구 상자
                OpenRandomBox(slotIndex, 19);
                break;
            case 24:
                // 붕대
                gameManager.Inventory.UpdateItemCount(slotIndex);
                gameManager.Player.Heal(50);
                break;
            case 25:
                // 체력 포션
                gameManager.Inventory.UpdateItemCount(slotIndex);
                gameManager.Player.Heal(50);
                break;
            case 26:
                // 고급 체력 포션
                gameManager.Inventory.UpdateItemCount(slotIndex);
                gameManager.Player.Heal(200);
                break;
        }
    }

    public bool CheckCanMultiUse(int itemId)
    {
        // 다중 사용이 가능한 아이템인지 확인
        // 사용 아이템이 추가되는 경우 조건문 업데이트
        if (itemId == 12 || itemId == 17 || itemId == 18 || itemId == 19)
        {
            return true;
        }

        return false;
    }
    // -------------------------------------------------------------
    // 랜덤 박스
    // -------------------------------------------------------------
    private void OpenRandomBox(int boxSlotIndex, int dropId)
    {
        bool isOK = gameManager.GetItemCheckInventory(GetRandomItemId(dropId));

        if (!isOK)
        {
            // 인벤토리가 꽉차서 아이템 추가가 안됨
            return;
        }
        // boxSlotIndex의 아이템 1개 소모 후 dropId의 랜덤 아이템 추가
        if (gameManager.Inventory.TryToUpdateItemCount(boxSlotIndex))
        {
            // 항목 삭제 후 Save and Reload
            gameManager.Inventory.Delete(boxSlotIndex);
        }
    }

    private int GetRandomItemId(int dropTableId)
    {
        return gameManager.DropManager.RandomItem(dropTableId);
    }

    // -------------------------------------------------------------
    // 아이템 강화
    // -------------------------------------------------------------
    private void StartReinforce(int slotIndex)
    {
        // 강화 UI Open
        gameManager.UIController.CloseUI();
        gameManager.UIController.OpenReinforceUI();

        // Inventory Select
        InventorySlot selectedSlot = gameManager.Inventory.Slots[slotIndex];
        selectedSlot.Select();                          // view
        gameManager.Inventory.SelectSlot(slotIndex);    // data

        // Reinforce Add
        gameManager.Reinforce.Add(selectedSlot.Item, selectedSlot.Id, selectedSlot.Count);
    }
}
