using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BuffSystem : MonoBehaviour
{
    [SerializeField]
    private GameObject buffPref;

    private UIController uiController;

    private Buff[] buffs;
    private int lastBuffIndex;

    private void Awake()
    {
        lastBuffIndex = 0;
        uiController = GameObject.Find("UI Controller").GetComponent<UIController>();
    }

    public void AddBuff(float remainTime = -1)
    {
        GameObject buff = Instantiate(buffPref);
        buff.transform.parent = uiController.BuffParent.transform;
        buff.GetComponent<Buff>().Set(lastBuffIndex);
        lastBuffIndex += 1;
    }

    public void RemoveBuff(int buffIndex)
    {
        buffs[buffIndex] = null;
    }
}