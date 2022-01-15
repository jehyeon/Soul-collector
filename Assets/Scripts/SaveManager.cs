using System.IO;
using UnityEngine;
using System.Linq;
using LitJson;

public class SaveManager
{
    private string saveFileName = "save.json";

    public Save save;


    public void Load()
    {
        string filePath = Path.Combine(Application.dataPath, saveFileName);

        string rawSave = File.ReadAllText(filePath);
        this.save = JsonMapper.ToObject<Save>(rawSave);
    }

    public void Save()
    {
        string json = JsonMapper.ToJson(save);
        string path = Path.Combine(Application.dataPath, "save.json");

        File.WriteAllText(path, json);
    }

    public void Init()
    {
        Save temp = new Save();
        string json = JsonMapper.ToJson(temp);
        
        string path = Path.Combine(Application.dataPath, "save.json");

        File.WriteAllText(path, json);
        Load();
    }

    public void Delete(int slotIndex)
    {
        save.slots[slotIndex] = null;

        save.slots = save.slots
            .Where(slot => slot != null)
            .ToList();

        save.slots.Add(new SlotSave(save.lastSlotId));
        save.slotIndex -= 1;
    }

    public void UpdateCount(int slotIndex, int count)
    {
        save.slots[slotIndex].count += count;
    }
}
