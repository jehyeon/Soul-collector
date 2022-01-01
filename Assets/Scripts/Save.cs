
using UnityEngine;

public class Save
{
    public int gold;
    public int slotIndex;
    public SlotSave[] slots;

    public Save()
    {
        gold = 0;
        slotIndex = 0;
        slots = new SlotSave[24];
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i] = new SlotSave();
        }
    }
}

public class SlotSave
{
    public int id;
    public int count;

    public SlotSave()
    {
        id = -1;
        count = 0;
    }
}
