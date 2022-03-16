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

    // 인벤토리 슬롯 아이템 정보
    private int level;      // 아이템 강화 성공 횟수, level이 1 이상인 경우 슬롯 하단에 Count 대신 Level 표기

    private Inventory inventory;

    // [SerializeField]     // !!! UNUSED
    // private Image image_equipped;
    [SerializeField]
    private GameObject go_selectedFrame;

    public int Id { get { return id; } }
    public bool IsEquip { get { return isEquip; } }
    public int Level { get { return level; } }      // 레벨에 따라 강화 확률이 달라짐

    // -------------------------------------------------------------
    // 인벤토리 슬롯 생성 및 로드
    // -------------------------------------------------------------
    public void Init(int slotindex, Inventory parentInventory)
    {
        // 게임 시작 시 슬롯 생성될 때 부여
        index = slotindex;
        inventory = parentInventory;
        level = 0;
    }

    public void Load(int slotId, Item loadedItem, int count, int itemLevel = 0)
    {
        id = slotId;
        Set(loadedItem, count);
        level = itemLevel;

        CheckLevel();
    }

    public override void ClearSlot()
    {
        base.ClearSlot();

        // UnEquip();   // !!! UNUSED
        level = 0;
    }

    private void CheckLevel()
    {
        // level이 1 이상인 경우, 강화 등급 표기
        if (level == 0 || itemCount > 1)
        {
            // level이 없거나, count가 2 이상인 경우
            return;
        }

        Color itemCountColor;
        ColorUtility.TryParseHtmlString("#FFFF00FF", out itemCountColor);
        text_count.color = itemCountColor;
        text_count.text = string.Format("+{0}", level);
    }

    // -------------------------------------------------------------
    // 인벤토리 슬롯 마우스 이벤트
    // -------------------------------------------------------------
    public void OnPointerClick(PointerEventData eventData)
    {
        // 마우스 클릭
        if (item == null)
        {
            // 아이템이 없는 경우 그냥 return
            return;
        }

        if (inventory.deleteItemMode && isEquip)
        {
            // 아이템 삭제 모드 일때, 장착 아이템 선택 시 무시
            return;
        }

        if (inventory.reinforceMode)
        {
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
    // 인벤토리 슬롯 장착
    // -------------------------------------------------------------
    // !!! UNUSED 
    // public void Equip()
    // {
    //     image_equipped.gameObject.SetActive(true);
    //     isEquip = true;
    // }

    // public void UnEquip()
    // {
    //     if (!isEquip)
    //     {
    //         return;
    //     }

    //     image_equipped.gameObject.SetActive(false);
    //     isEquip = false;
    // }

    // -------------------------------------------------------------
    // 인벤토리 슬롯 강화 (Inventory.Reinforce() 강화 성공 시)
    // -------------------------------------------------------------
    public void Upgrade(Item nextItem)
    {
        // nextItem Id: Item.Id + 1, ItemManager를 통해 호출 후 재 할당
        Set(nextItem);
        level += 1;
        CheckLevel();
    }
}