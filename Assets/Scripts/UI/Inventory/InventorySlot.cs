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
        // 마우스 왼클릭
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
            if (!isSelected)
            {
                
                if (inventory.Mode == InventoryMode.Reinforce)
                {
                    // 강화 모드인 경우 reinforce slot에 추가
                    if (inventory.CheckItemForReinforce(item))
                    {
                        // 강화 아이템의 종류가 같은지 확인
                        if (inventory.AddItemToReinforceSlot(item, id, itemCount))
                        {
                            // 강화 슬롯이 부족하면 select 안됨
                            inventory.SelectSlot(index);
                            Select();
                        }
                    }
                }
                else
                {
                    inventory.SelectSlot(index);
                    Select();
                }
            }
            else
            {
                inventory.UnSelectSlot(index);
                UnSelect();
                if (inventory.Mode == InventoryMode.Reinforce)
                {
                    // 강화 모드인 경우 reinforce slot에서 삭제
                    inventory.RemoveItemToReinforceSlot(id, Item.Id);
                }
            }
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (inventory.Mode == InventoryMode.Shop)
            {
                // 상점 모드에서는 우클릭 기능 제한
                return;
            }
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
    public void Select()
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