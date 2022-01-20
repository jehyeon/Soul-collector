using System.IO;
using UnityEngine;
using LitJson;

public class SaveManager
{
    private string saveFileName = "save.json";
    private string _filePath;

    private Save _save;
    public Save Save { get { return _save; } }

    public SaveManager()
    {
        // 파일이 있는 지 확인
        _filePath = Path.Combine(Application.dataPath, saveFileName);
        FileInfo fileInfo = new FileInfo(_filePath);
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
        this._save = JsonMapper.ToObject<Save>(File.ReadAllText(_filePath));
    }

    public void SaveData()
    {
        // obj to json
        File.WriteAllText(_filePath, JsonMapper.ToJson(_save));
    }

    public void SaveData(Save save)
    {
        // obj to json
        File.WriteAllText(_filePath, JsonMapper.ToJson(save));
    }
    public void Init()
    {
        // 빈 세이브 생성 후 Load
        this.SaveData(new Save());
        this.Load();
    }
}
