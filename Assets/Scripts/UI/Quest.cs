using UnityEngine.EventSystems;
using UnityEngine;
using TMPro;
using System.Collections.Generic;

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
    private int temp = -1;

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
        if (isDone)
        {
            // 퀘스트가 완료되면 더 이상 확인 안함
            return;
        }

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
        else if (id == 1)
        {
            if (questSystem.gameManager.SaveManager.Save.Equipped[0] == 301)
            {
                // 철 장검이 장착되어 있으면
                progressCount = maxProgressCount;
                UpdateProgress(progressCount);
                DoneQuest();
            }
        }
        else if (id == 2)
        {
            // int inventoryIndex = questSystem.gameManager.Inventory.FindItemUsingSlotId(25);
            for (int i = 0; i < questSystem.gameManager.SaveManager.Save.Slots.Count; i++)
            {
                if (questSystem.gameManager.SaveManager.Save.Slots[i].ItemId == -1)
                {
                    break;
                }

                if (questSystem.gameManager.SaveManager.Save.Slots[i].ItemId == 25)
                {
                    progressCount = questSystem.gameManager.SaveManager.Save.Slots[i].Count;
                }
                
                if (progressCount >= maxProgressCount)
                {
                    UpdateProgress(maxProgressCount);
                    DoneQuest();
                }
            }
        }
        else if (id == 3)
        {
            List<int> quickSlots = questSystem.gameManager.SaveManager.Save.QuickSlot.FindAll(element => element != -1);

            foreach(int slotId in quickSlots)
            {
                // 퀵슬롯에 등록된 아이템 확인
                Item item = questSystem.gameManager.Inventory.GetItemBySlotId(slotId);
                if (item.Id == 25)
                {
                    // 등록된 아이템이 체력 포션인 경우
                    UpdateProgress(maxProgressCount);
                    DoneQuest();
                }
            }
        }
        else if (id == 4)
        {
            if (questSystem.gameManager.SaveManager.Save.Skill.IndexOf(1) != -1)
            {
                UpdateProgress(maxProgressCount);
                DoneQuest();
            }
        }
        else if (id == 5)
        {
            for (int i = 0; i < questSystem.gameManager.SaveManager.Save.Slots.Count; i++)
            {
                if (questSystem.gameManager.SaveManager.Save.Slots[i].ItemId == -1)
                {
                    break;
                }

                if (questSystem.gameManager.SaveManager.Save.Slots[i].ItemId == 12)
                {
                    UpdateProgress(maxProgressCount);
                    DoneQuest();
                }
            }
        }
        else if (id == 6)
        {
            if (questSystem.gameManager.SaveManager.Save.RushCount > 0)
            {
                // 강화를 1번 이상 시도한 경우
                UpdateProgress(maxProgressCount);
                DoneQuest();
            }
        }
        else if (id == 7)
        {
            if (questSystem.gameManager.Floor > 0)
            {
                // 던전에 입장한 경우
                UpdateProgress(maxProgressCount);
                DoneQuest();
            }
        }
        else if (id == 8 || id == 9)
        {
            if (temp == -1)
            {
                // 퀘스트를 막 받은 경우
                temp = questSystem.gameManager.SaveManager.Save.KillCount;
            }

            if (questSystem.gameManager.SaveManager.Save.KillCount != temp)
            {
                // 누적 킬 변동이 생긴 경우
                progressCount = questSystem.gameManager.SaveManager.Save.KillCount - temp;
            }

            if (progressCount >= maxProgressCount)
            {
                UpdateProgress(maxProgressCount);
                DoneQuest();
            }
            else
            {
                UpdateProgress(progressCount);
            }
        }
        else if (id == 10)
        {
            if (questSystem.gameManager.SaveManager.Save.AttackCollection != -1 
                || questSystem.gameManager.SaveManager.Save.DefenseCollection != -1)
            {
                // 컬렉션이 등록되었다면
                UpdateProgress(maxProgressCount);
                DoneQuest();
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
