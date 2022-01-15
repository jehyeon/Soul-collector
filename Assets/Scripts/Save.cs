
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Save
{
    public int gold;
    public int slotIndex;
    public int lastSlotId;
    // public SlotSave[] slots;
    public List<SlotSave> slots;

    public Save()
    {
        gold = 0;
        slotIndex = 0;
        lastSlotId = 0;
        slots = new List<SlotSave>();
        for (int i = 0; i < 60; i++)
        {
            slots.Add(new SlotSave(lastSlotId));
            lastSlotId += 1;
        }
    }
}

public class SlotSave
{
    public int slotId;
    public int id;
    public int count;

    public SlotSave()
    {
        slotId = -1;
        id = -1;
        count = 0;
    }

    public SlotSave(int _slotId)
    {
        slotId = _slotId;
        id = -1;
        count = 0;
    }
}
