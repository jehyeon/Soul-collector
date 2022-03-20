using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : Slot, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    // 인벤토리 슬롯 정보
    private int index;      // 인벤토리 슬롯 index
    private int id;         // 인벤토리 슬롯 ID
    private bool isEquip;   // 슬롯 아이템 장착 여부
    public bool isSelected; // 슬롯 선택 여부

    private Inventory inventory;

    // [SerializeField]     // !!! UNUSED
    // private Image image_equipped;
    [SerializeField]
    private GameObject go_selectedFrame;

    public int Index { get { return index; } }
    public int Id { get { return id; } }
    public bool IsEquip { get { return isEquip; } }

    // -------------------------------------------------------------
    // 인벤토리 슬롯 생성 및 로드
    // -------------------------------------------------------------
    public void Init(int slotindex, Inventory parentInventory)
    {
        // 게임 시작 시 슬롯 생성될 때 부여
        index = slotindex;
        inventory = parentInventory;
    }

    public void Load(int slotId, Item loadedItem, int count)
    {
        id = slotId;
        Set(loadedItem, count);
    }

    // -------------------------------------------------------------
    // 인벤토리 슬롯 마우스 이벤트
    // -------------------------------------------------------------
    public void OnPointerClick(PointerEventData eventData)
    {
        if (inventory.Mode == InventoryMode.NotWork)
        {
            // 인벤토리 모드가 NotWork인 경우 클릭 이벤트 비활성화
            return;
        }

        if (item == null)
        {
            // 아이템이 없는 경우 그냥 return
            return;
        }

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            // 마우스 왼클릭
            if (inventory.reinforceMode)
            {
                // !!!
                // 인벤토리가 강화 모드인 경우
                if (item.Rank > 5)
                {
                    // 강화 불가능한 등급 (ex. +9, resource data rank: 6)
                    return;
                }

                if (isEquip)
                {
                    // 장착한 아이템은 강화 불가능
                    return;
                }

                if (inventory.scrollType == item.ItemType)
                {
                    // 선택된 주문서 타입과 현재 슬롯의 아이템 타입이 같아야 함
                    inventory.Reinforce(index, item.Id);     // 강화할 아이템 slot index, item ID 전달
                }
            }
            else
            {
                if (!isSelected)
                {
                    inventory.SelectSlot(index);
                    Select();
                }
                else
                {
                    inventory.UnSelectSlot(index);
                    UnSelect();
                }   
            }
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            // 마우스 우클릭
            inventory.RightClick(index);
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

        inventory.ShowItemDetail(item, transform.position);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // 마우스 오버 아웃
        inventory.CloseItemDetail();
    }

    // -------------------------------------------------------------
    // 인벤토리 슬롯 선택
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
    // 인벤토리 슬롯 강화 (Inventory.Reinforce() 강화 성공 시)
    // -------------------------------------------------------------
    public void Upgrade(Item nextItem)
    {
        // nextItem Id: Item.Id + 1, ItemManager를 통해 호출 후 재 할당
        Set(nextItem);
    }
}