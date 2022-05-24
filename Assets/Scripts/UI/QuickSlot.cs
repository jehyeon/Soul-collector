using UnityEngine.EventSystems;
using UnityEngine;

public enum QuickSlotType
{
    Skill,
    Use,
    None
}

public class QuickSlot : Slot, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    private QuickSlotSystem parent;
    private QuickSlotType type;
    private int id;
    private int index;

    public void Init(QuickSlotSystem quickSlotSystem, QuickSlotType quickSlotType, int quickSlotId, int slotIndex)
    {
        parent = quickSlotSystem;
        type = quickSlotType;
        id = quickSlotId;
        index = slotIndex;

        if (type == QuickSlotType.Use)
        {
            Set(parent.GameManager.Inventory.GetItemBySlotId(id));
        }
    }

    public void Clear()
    {
        type = QuickSlotType.None;
        id = -1;
        ClearSlot();
        Color backgroundColor;
        ColorUtility.TryParseHtmlString("#000000C8", out backgroundColor);
        SetBackground(backgroundColor);
    }

    public void Act()
    {
        if (type == QuickSlotType.None || id == -1)
        {
            // 퀵슬롯에 아이템 등록이 안된 경우
            return;
        }
        if (type == QuickSlotType.Use)
        {
            parent.GameManager.Inventory.UseBySlotId(id);
        }

        if (parent.GameManager.Inventory.GetItemBySlotId(id) == null)
        {
            // Use 이후 아이템 검색이 안되는 경우 (삭제된 경우)
            parent.DeleteQuickSlot(index);
        }
    }

    // -------------------------------------------------------------
    // 마우스 이벤트
    // -------------------------------------------------------------
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!parent.DefaultMode)
        {
            InventorySlot slot = parent.GameManager.Inventory.Slots[parent.GameManager.Inventory.SelectedSlotIndex];
            id = slot.Id;
            parent.CheckIsQuickSlot(id);
            Set(slot.Item);
            parent.SetQuickSlot(index, QuickSlotType.Use, id);
            type = QuickSlotType.Use;
            parent.Close();
            return;
        }

        if (type == QuickSlotType.None)
        {
            return;
        }

        if (type == QuickSlotType.Use)
        {
            
            return;
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

        parent.ShowItemDetail(item, transform.position);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // 마우스 오버 아웃
        parent.CloseItemDetail();
    }
}
