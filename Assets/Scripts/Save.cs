
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Save
{
    private int _gold;              // Gold 정보
    private int _lastSlotIndex;     // 아이템이 있는 마지막 슬롯 index
    private int _lastSlotId;        // 다음 슬롯의 id
    private int _inventorySize;     // 인벤토리 사이즈
    private List<int> _equipped;    // 장착 상태인 슬롯 id list
    private List<SlotSave> _slots;  // 슬롯 정보

    // Properties
    public List<SlotSave> Slots { get { return _slots; } set { _slots = value; } }
    public int Gold { get { return _gold; } set { _gold = value; } }
    public List<int> Equipped { get { return _equipped; } set { _equipped = value; } }
    public int LastSlotIndex { get { return _lastSlotIndex; } set { _lastSlotIndex = value; } }
    public string GoldText
    {
        get
        {
            return string.Format("{0:#,0}", _gold).ToString();
        }
    }

    // Methods
    public Save()
    {
        _gold = 0;
        _lastSlotIndex = 0;
        _lastSlotId = 0;
        _equipped = new List<int> {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1};
        _slots = new List<SlotSave>();
        _inventorySize = 60;
        for (int i = 0; i < _inventorySize; i++)
        {
            _slots.Add(new SlotSave(_lastSlotId));
            _lastSlotId += 1;
        }
    }

    public void AddSlot()
    {
        _slots.Add(new SlotSave(_lastSlotId));
        _lastSlotId += 1;
    }

    public void AddSlot(int itemId, int count)
    {
        _slots.Add(new SlotSave(_lastSlotId, itemId, count));
        _lastSlotId += 1;
    }

    public void DeleteSlot(int slotIndex)
    {
        // slots의 slotIndex slot 삭제
        _slots[slotIndex] = null;

        _slots = _slots
            .Where(slot => slot != null)
            .ToList();

        slotIndex -= 1;

        // 빈 슬롯 생성
        this.AddSlot();
    }

    public void UpgradeInventorySize()
    {
        // 슬롯 사이즈 추가
        _inventorySize += 4;
        for (int i = 0; i < 4; i++)
        {
            _slots.Add(new SlotSave(_lastSlotId));
            _lastSlotId += 1;
        }
    }
}

public class SlotSave
{
    private int _id;
    private int _itemId;
    private int _count;
    public int Count { get { return _count; } }
    // 장착 아이템이 아니거나, 장착하지 않은 경우 -1, 장착 시 itemType 번호 (ex. 무기 0, 방패 1 ...)
    // private int _equippedType;  

    public SlotSave()
    {
        _id = -1;
        _itemId = -1;
        _count = 0;
    }

    public SlotSave(int id)
    {
        _id = id;
        _itemId = -1;
        _count = 0;
    }

    public SlotSave(int id, int itemId, int count)
    {
        _id = id;
        _itemId = itemId;
        _count = count;
    }

    public void UpdateCount(int diff)
    {
        _count += diff;
    }
    
    public void Set(int slotId, int itemId, int itemCount)
    {
        _id = slotId;
        _itemId = itemId;
        _count = itemCount;
    }
}
