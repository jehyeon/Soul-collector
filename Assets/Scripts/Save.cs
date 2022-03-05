
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Save
{
    // json to obj를 위해 save는 모두 public으로 선언
    public int Gold;              // Gold 정보
    public int LastSlotIndex;     // 아이템이 있는 마지막 슬롯 index
    public int LastSlotId;        // 다음 슬롯의 id
    public int InventorySize;     // 인벤토리 사이즈
    public List<int> Equipped;    // 장착 상태인 슬롯 id list
    public List<SlotSave> Slots;  // 슬롯 정보

    // Methods
    public Save()
    {
        Gold = 9999999;     // !!! for test
        LastSlotIndex = 0;
        LastSlotId = 0;
        Equipped = new List<int> {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1};
        Slots = new List<SlotSave>();
        InventorySize = 60;
        for (int i = 0; i < InventorySize; i++)
        {
            Slots.Add(new SlotSave(LastSlotId));
            LastSlotId += 1;
        }
    }

    // 맨 앞 빈 슬롯에 아이템 추가
    public void AddItem(int itemId, int itemCount)
    {
        Slots[LastSlotIndex].ItemId = itemId;
        Slots[LastSlotIndex].Count = itemCount;
        LastSlotIndex += 1;
    }

    // 인벤토리 마지막에 빈 슬롯 추가
    public void AddSlot()
    {
        Slots.Add(new SlotSave(LastSlotId));
        LastSlotId += 1;
    }

    public void AddSlot(int itemId, int count)
    {
        Slots.Add(new SlotSave(LastSlotId, itemId, count));
        LastSlotId += 1;
    }

    public void DeleteSlot(int slotIndex)
    {
        // slots의 slotIndex slot 삭제
        Slots[slotIndex] = null;

        Slots = Slots
            .Where(slot => slot != null)
            .ToList();

        slotIndex -= 1;

        // 빈 슬롯 생성
        this.AddSlot();
    }

    public void UpgradeInventorySize()
    {
        // 슬롯 사이즈 추가
        InventorySize += 4;
        for (int i = 0; i < 4; i++)
        {
            Slots.Add(new SlotSave(LastSlotId));
            LastSlotId += 1;
        }
    }
}

public class SlotSave
{
    public int Id;
    public int ItemId;
    public int Count;

    public SlotSave()
    {
        Id = -1;
        ItemId = -1;
        Count = 0;
    }

    public SlotSave(int id)
    {
        Id = id;
        ItemId = -1;
        Count = 0;
    }

    public SlotSave(int id, int itemId, int count)
    {
        Id = id;
        ItemId = itemId;
        Count = count;
    }

    public void UpdateCount(int diff)
    {
        Count += diff;
    }
    
    public void Set(int slotId, int itemId, int itemCount)
    {
        Id = slotId;
        ItemId = itemId;
        Count = itemCount;
    }
}
