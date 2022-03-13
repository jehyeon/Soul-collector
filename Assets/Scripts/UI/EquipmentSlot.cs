using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentSlot : Slot, IPointerClickHandler
{
    private int index;
    private bool isSelected;

    // 상위 Equipment
    private Equipment equipment;

    [SerializeField]
    private GameObject go_selectedFrame;

    // -------------------------------------------------------------
    // 인벤토리 슬롯 생성 및 로드
    // -------------------------------------------------------------
    public void Init(int slotindex, Equipment parentEquipment)
    {
        // 게임 시작 시 슬롯 생성될 때 부여
        index = slotindex;
        equipment = parentEquipment;
    }

    // -------------------------------------------------------------
    // 인벤토리 슬롯 터치 이벤트
    // -------------------------------------------------------------
    public void OnPointerClick(PointerEventData eventData)
    {
        if (item == null)
        {
            // 아이템이 아닌 경우 return
            return;
        }

        if (!isSelected)
        {
            equipment.SelectSlot(index);
            Select();
        }
        else
        {
            equipment.UnSelectSlot();
            UnSelect();
        }
    }

    // -------------------------------------------------------------
    // 장착 슬롯 선택
    // -------------------------------------------------------------
    private void Select()
    {
        isSelected = true;
        go_selectedFrame.SetActive(true);
    }

    public void UnSelect()
    {
        isSelected = false;
        go_selectedFrame.SetActive(false);
    }

    // -------------------------------------------------------------
    // 장착 (view만 담당)
    // -------------------------------------------------------------
    public void Equip(Item equippingItem)
    {
        if (item != null)
        {
            // 해당 파츠 아이템을 장착하고 있는 경우
            UnEquip();  // 장착 상태 취소
        }

        Set(equippingItem);
    }

    public void UnEquip()
    {
        if (item == null)
        {
            // 슬롯에 아이템이 없는 경우
            return;
        }

        ClearSlot();
    }
}