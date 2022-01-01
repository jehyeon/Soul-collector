using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveManager
{
    private string saveFileName = "save.json";

    public UserSave save;

    public SaveManager()
    {
        // SaveManager 생성 시 Load
        LoadSave();
    }

    public void LoadSave()
    {
        string filePath = Path.Combine(Application.dataPath, saveFileName);
        string rawSave = File.ReadAllText(filePath);
        save = JsonUtility.FromJson<UserSave>(rawSave);
    }

    public void Save()
    {
        string json = JsonUtility.ToJson(save, true);
        string path = Path.Combine(Application.dataPath, "save.json");

        File.WriteAllText(path, json);
    }
}
