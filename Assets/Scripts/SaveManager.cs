using System.IO;
using UnityEngine;
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
        File.WriteAllText(filePath, JsonMapper.ToJson(save));
    }

    public void SaveData(Save save)
    {
        // obj to json
        File.WriteAllText(filePath, JsonMapper.ToJson(save));
    }
    public void Init()
    {
        // 빈 세이브 생성 후 Load
        this.SaveData(new Save());
        this.Load();
    }
}
