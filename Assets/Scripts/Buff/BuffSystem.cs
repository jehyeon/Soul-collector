using UnityEngine;
using System.Collections.Generic;

public class BuffSystem : MonoBehaviour
{
    [SerializeField]
    private GameManager gameManager;
    [SerializeField]
    private ObjectPool buffSlotOP;
    [SerializeField]
    private GameObject buffParent;

    public Player player;
    public List<BuffSlot> buffs;

    private void Awake()
    {
        buffs = new List<BuffSlot>();
    }

    public void AddBuff(int buffId, Sprite image, Stat stat, float remainTime = -1f)
    {
        // 기존 버프 중 동일한 buffId가 있는지 검사
        for (int i = 0; i < buffs.Count; i++)
        {
            if (buffs[i].Id == buffId)
            {
                // 동일한 id가 있는 경우 즉시 종료
                buffs[i].EndNow();
                break;
            }
        }

        // 새로운 버프 추가
        GameObject buffSlot = buffSlotOP.Get();
        buffSlot.transform.SetParent(buffParent.transform);
        BuffSlot slot = buffSlot.GetComponent<BuffSlot>();
        slot.Set(this, buffId, image, stat, remainTime);
        
        gameManager.UIController.UpdateStatUI();
        buffs.Add(slot);
    }

    public void RemoveBuff(int buffId, GameObject slotObject)
    {
        for (int i = 0; i < buffs.Count; i++)
        {
            if (buffs[i].Id == buffId)
            {
                buffs.RemoveAt(i);
            }
        }
        buffSlotOP.Return(slotObject);
    }
}