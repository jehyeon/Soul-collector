using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BuffSystem : MonoBehaviour
{
    [SerializeField]
    private GameManager gameManager;
    [SerializeField]
    private ObjectPool buffSlotOP;
    [SerializeField]
    private GameObject buffParent;

    private BuffSlot[] buffs;
    private int lastBuffIndex;

    private void Awake()
    {
        lastBuffIndex = 0;
    }

    public void AddBuff(float remainTime = -1)
    {
        GameObject buffSlot = buffSlotOP.Get();
        buffSlot.transform.parent = buffParent.transform;
        // buff.GetComponent<Buff>().Set(lastBuffIndex);
        lastBuffIndex += 1;
    }

    public void RemoveBuff(int buffIndex)
    {
        buffs[buffIndex] = null;
    }
}