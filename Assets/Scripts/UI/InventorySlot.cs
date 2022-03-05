using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : Slot, IPointerClickHandler
{
    private int index;

    private int id;
    private bool isEquip;
    public bool isSelected;
    public int upgradeLevel;
    private Inventory inventory;

    [SerializeField]
    private Image image_equipped;
    [SerializeField]
    private GameObject go_selectedFrame;

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

    public override void ClearSlot()
    {
        base.ClearSlot();

        UnEquip();
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
            inventory.SelectSlot(index);
            Select();
        }
        else
        {
            inventory.UnSelectSlot(index);
            UnSelect();
        }

        // if (cv.GetComponent<Inventory>().reinforceMode)
        // {
        //     if (item.Rank > 5)
        //     {
        //         // 강화 불가능한 등급 (ex. 9강, resource)
        //         return;
        //     }

        //     // 강화 모드인 경우
        //     int scrollType = cv.GetComponent<Inventory>().scrollType;
            
        //     if (scrollType == 15)
        //     {
        //         // 무기 강화
        //         if (!(itemId >= 0 && itemId <= 499))
        //         {
        //             // 무기가 아닌 경우
        //             return;
        //         }
        //         cv.GetComponent<Inventory>().Rush(inventoryIndex);
        //     }
        //     else if (scrollType == 16)
        //     {
        //         // 방어구 강화
        //         if (!(itemId >= 500 && itemId <= 1299))
        //         {
        //             // 방어구가 아닌 경우
        //             return;
        //         }
        //         cv.GetComponent<Inventory>().Rush(inventoryIndex);
        //     }
        //     else if (scrollType == 17)
        //     {
        //         // 장신구 강화
        //         if (!(itemId >= 1300 && itemId <= 1600))
        //         {
        //             // 장신구가 아닌 경우
        //             return;
        //         }
        //         cv.GetComponent<Inventory>().Rush(inventoryIndex);
        //     }
        //     else if (scrollType == 18)
        //     {
        //         // 소울스톤 강화
        //         if (!(itemId >= 1608 && itemId <= 1613))
        //         {
        //             // 소울스톤이 아닌 경우
        //             return;
        //         }

        //         // temp
        //     }

        //     return;
        // }
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
    public void Equip()
    {
        image_equipped.gameObject.SetActive(true);
        isEquip = true;
    }

    public void UnEquip()
    {
        if (!isEquip)
        {
            return;
        }

        image_equipped.gameObject.SetActive(false);
        isEquip = false;
    }
}