using System.IO;
using UnityEngine;
using System.Collections.Generic;
using LitJson;

public class SaveManager
{
    private string saveFileName = "save.json";
    private string filePath;

    private Save save;
    public Save Save { get { return save; } }
    public SaveManager()
    {
        // 파일이 있는 지 확인
        filePath = Path.Combine(Application.dataPath, saveFileName);
        FileInfo fileInfo = new FileInfo(filePath);
        if (fileInfo.Exists)
        {
            this.Load();
        }
        else
        {
            this.Init();
        }
    }

    private void Load()
    {
        // json to obj
        this.save = JsonMapper.ToObject<Save>(File.ReadAllText(filePath));
    }

    public void SaveData()
    {
        // obj to json
        JsonWriter writer = new JsonWriter();
        writer.PrettyPrint = true;
        writer.IndentValue = 2;
        JsonMapper.ToJson(save, writer);

        File.WriteAllText(filePath, writer.ToString());
    }

    public void SaveData(Save save)
    {
        // obj to json
        JsonWriter writer = new JsonWriter();
        writer.PrettyPrint = true;
        writer.IndentValue = 2;
        JsonMapper.ToJson(save, writer);

        File.WriteAllText(filePath, writer.ToString());
    }
    public void Init()
    {
        // 빈 세이브 생성 후 Load
        this.SaveData(new Save());
        this.Load();
    }
}
