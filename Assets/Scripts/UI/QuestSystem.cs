using System.IO;
using UnityEngine;
using System.Collections.Generic;
using LitJson;

public class QuestSystem : MonoBehaviour
{
    public GameManager gameManager;
    [SerializeField]
    private GameObject questPref;
    [SerializeField]
    private GameObject questParent;

    private List<Dictionary<string, object>> data;
    private string saveFileName = "quest.json";
    private string filePath;

    private QuestJSON questJSON;

    private Quest progressQuest;

    private void Start()
    {
        data = CSVReader.Read("Quest");
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

        if (questJSON.Progress.Count > 0)
        {
            // 진행 중인 퀘스트가 있는 경우
            progressQuest = Instantiate(questPref).GetComponent<Quest>();
            progressQuest.transform.SetParent(questParent.transform);
            // !!! 진행 중인 퀘스트는 1개 밖에 없음 (임시)
            progressQuest.Set(
                this,
                questJSON.Progress[0].Id,
                data[questJSON.Progress[0].Id]["name"].ToString(),
                data[questJSON.Progress[0].Id]["description"].ToString(),
                questJSON.Progress[0].Progress,
                (int)data[questJSON.Progress[0].Id]["count"]
            );
        }
        else if (questJSON.Ready.Count == 0)
        {
            // 남은 퀘스트 목록이 없는 경우
            Destroy(this.gameObject);
        }
    }

    private void Init()
    {   
        // 빈 세이브 생성 후 Load
        this.questJSON = new QuestJSON();
        this.Save();

        Load();
    }

    private void Load()
    {
        // json to obj
        this.questJSON = JsonMapper.ToObject<QuestJSON>(File.ReadAllText(filePath));
    }

    public void Save()
    {
        JsonWriter writer = new JsonWriter();
        writer.PrettyPrint = true;
        writer.IndentValue = 2;
        JsonMapper.ToJson(questJSON, writer);

        File.WriteAllText(filePath, writer.ToString());
    }

    public void DoneProgress(int questId)
    {
        // questId와 progress[0].Id가 다르면 error
        if (questJSON.Progress[0].Id != questId)
        {
            Debug.LogError("퀘스트 버그");
            return;
        }
        
        // 퀘스트 완료로 수정
        questJSON.Progress[0].Progress = (int)data[questJSON.Progress[0].Id]["count"];
        Save();     // 저장

        gameManager.PopupReward((int)data[questJSON.Progress[0].Id]["reward"]);
    }

    public void ClearQuest()
    {
        // reward popup에서 확인 버튼을 누른 경우

        // 퀘스트 클리어 및 보상 획득
        // 인벤토리가 꽉차면 함수 호출 불가능
        gameManager.GetItem(
            (int)data[questJSON.Progress[0].Id]["reward"],
            (int)data[questJSON.Progress[0].Id]["rewardCount"]
        );

        // Progress[0]을 Done에 추가
        questJSON.Done.Add(questJSON.Progress[0].Id);
        questJSON.Progress.RemoveAt(0);     // 기존 progress 제거
        // Ready[0]을 Progress에 추가
        QuestProgress newQuest = new QuestProgress((int)questJSON.Ready[0], 0);
        questJSON.Progress.Add(newQuest);
        questJSON.Ready.RemoveAt(0);

        Save();     // 저장
        
        Destroy(progressQuest.gameObject);

        Quest quest = Instantiate(questPref).GetComponent<Quest>();
        quest.transform.SetParent(questParent.transform);
        // !!! 진행 중인 퀘스트는 1개 밖에 없음 (임시)
        quest.Set(
            this,
            questJSON.Progress[0].Id,
            data[questJSON.Progress[0].Id]["name"].ToString(),
            data[questJSON.Progress[0].Id]["description"].ToString(),
            questJSON.Progress[0].Progress,
            (int)data[questJSON.Progress[0].Id]["count"]
        );
    }
}
