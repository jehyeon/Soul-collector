
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Save
{
    public int gold;
    public int slotIndex;
    // public SlotSave[] slots;
    public List<SlotSave> slots;

    public Save()
    {
        gold = 0;
        slotIndex = 0;
        slots = new List<SlotSave>();
        // slots = Enumerable.Repeact(new SlotSave(), 60).ToList();
        // slots = new SlotSave[60];
        for (int i = 0; i < 60; i++)
        {
            slots.Add(new SlotSave());
        }
    }
}

public class SlotSave
{
    // public int slotIndex;
    public int id;
    public int count;

    public SlotSave()
    {
        // slotIndex = _slotIndex;
        id = -1;
        count = 0;
    }
}
