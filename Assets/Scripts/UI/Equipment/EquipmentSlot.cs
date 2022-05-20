using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentSlot : Slot, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    private bool isSelected;

    [SerializeField]
    private Equipment equipment;    // 상위 Equipment

    [SerializeField]
    private GameObject go_selectedFrame;
    [SerializeField]
    private int slotIndex;          // Equipment index

    public int Index { get { return slotIndex; } }
    
    // -------------------------------------------------------------
    // 인벤토리 슬롯 터치 이벤트
    // -------------------------------------------------------------
    public void OnPointerClick(PointerEventData eventData)
    {
        if (item == null)
        {
            // 장착된 아이템이 없는 경우 return
            return;
        }

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            // 마우스 왼클릭
            if (!isSelected)
            {
                equipment.SelectSlot(slotIndex);
                Select();
            }
            else
            {
                equipment.UnSelectSlot();
                UnSelect();
            }
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            // 마우스 우클릭
            equipment.UnEquipItem(slotIndex);
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

        equipment.ShowItemDetail(item, transform.position);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // 마우스 오버 아웃
        equipment.CloseItemDetail();
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