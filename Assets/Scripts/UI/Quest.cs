using UnityEngine.EventSystems;
using UnityEngine;
using TMPro;

public class Quest : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private TextMeshProUGUI questName;
    [SerializeField]
    private TextMeshProUGUI questCount;
    [SerializeField]
    private TextMeshProUGUI questDes;

    private int id;
    private QuestSystem questSystem;
    private int progressCount;
    private int maxProgressCount;
    private float tempCount;
    private bool isDone;

    public void Set(QuestSystem system, int questId, string name, string description, int nowProgress, int maxProgress)
    {
        questSystem = system;
        id = questId;
        questName.text = name;
        questDes.text = description;
        progressCount = nowProgress;
        tempCount = nowProgress;
        maxProgressCount = maxProgress;
        questCount.text = string.Format("{0} / {1}", progressCount, maxProgressCount);
        isDone = false;

        if (progressCount == maxProgressCount)
        {
            // 이미 완료된 퀘스트인 경우
            UpdateProgress(progressCount);
            DoneQuest();
        }
    }

    public void Update()
    {
        if (id == 0)
        {
            tempCount += questSystem.gameManager.Player.PlayerAgent.velocity.sqrMagnitude * Time.deltaTime;

            if (tempCount > maxProgressCount)
            {
                progressCount = maxProgressCount;
                UpdateProgress(progressCount);
                DoneQuest();
            }
            else
            {
                UpdateProgress(tempCount);
            }
        }
    }

    private void UpdateProgress(int data)
    {
        questCount.text = string.Format("{0} / {1}", data, maxProgressCount);
    }

    private void UpdateProgress(float data)
    {
        questCount.text = string.Format("{0} / {1}", Mathf.FloorToInt(data), maxProgressCount);
    }

    private void DoneQuest()
    {
        questDes.text = "퀘스트 완료.\n클릭하여 보상을 받으세요.";
        isDone = true;
    }

     public void OnPointerClick(PointerEventData eventData)
    {
        if (!isDone)
        {
            // 퀘스트 완료가 아니면 무시
            return;
        }

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            questSystem.DoneProgress(id);
            // Destroy(this.gameObject);
        }
    }
}
