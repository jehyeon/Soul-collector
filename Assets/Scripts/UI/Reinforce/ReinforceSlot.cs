using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ReinforceSlot : Slot, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    // ReinforceSlot은 InventorySlot의 아이템을 가져오지 않음
    // 아이템의 view만 관리
    private Reinforce reinforce;
    private int inventorySlotId;    // 인벤토리 slot의 id
    public int InventorySlotId { get { return inventorySlotId; } }

    public void SetReinforceSlot(Reinforce myReinforce, Item inventorySlotItem, int slotId, int count = 1)
    {
        reinforce = myReinforce;
        Set(inventorySlotItem, count);
        inventorySlotId = slotId;
    }

    public void Clear()
    {
        ClearSlot();
        inventorySlotId = -1;
    }

    public void Upgrade(Item nextItem)
    {
        // nextItem Id: Item.Id + 1, ItemManager를 통해 호출 후 재 할당
        Set(nextItem);
    }
    
    // -------------------------------------------------------------
    // 슬롯 이벤트
    // -------------------------------------------------------------
    public void OnPointerClick(PointerEventData eventData)
    {
        if (item == null)
        {
            // 아이템이 없는 경우
            return;
        }

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            // 왼클릭 시
            reinforce.Remove(this, inventorySlotId);
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        // 마우스 오버
        if (item == null)
        {
            // 아이템이 없는 경우 그냥 return
            return;
        }

        reinforce.ShowItemDetail(item, transform.position);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (item == null)
        {
            // 아이템이 없는 경우 그냥 return
            return;
        }
        // 마우스 오버 아웃
        reinforce.CloseItemDetail();
    }    
}
