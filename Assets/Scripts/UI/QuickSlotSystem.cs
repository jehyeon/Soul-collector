using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickSlotSystem : MonoBehaviour
{
    private GameManager gameManager;
    public GameManager GameManager { get { return gameManager; } }
    // true: default mode, false: SetQuickSlot mode
    private bool defaultMode = true;
    public bool DefaultMode { get { return defaultMode; } }
    private QuickSlot[] slots;

    [SerializeField]
    private GameObject goSetModeBackground;     // background
    [SerializeField]
    private GameObject goSlotParent;            // slot parent
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            slots[0].Act();
            return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            slots[0].Act();
            return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            slots[1].Act();
            return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            slots[2].Act();
            return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            slots[3].Act();
            return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            slots[4].Act();
            return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            slots[5].Act();
            return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            slots[6].Act();
            return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            slots[7].Act();
            return;
        }
    }

    public void LoadQuickSlotSystem()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        slots = goSlotParent.GetComponentsInChildren<QuickSlot>();

        // Load slot
        for (int i = 0; i < 8; i++)
        {
            int quickSlotId = gameManager.SaveManager.Save.QuickSlot[i];
            
            if (quickSlotId == -1)
            {
                // none
                slots[i].Init(this, QuickSlotType.None, -1, i);
            }
            else if (quickSlotId < 0)
            {
                // skill id
                quickSlotId *= -1;
                slots[i].Init(this, QuickSlotType.Skill, quickSlotId, i);
            }
            else
            {
                slots[i].Init(this, QuickSlotType.Use, quickSlotId, i);
            }
        }
    }

    public void SetQuickSlot(int index, QuickSlotType type, int id)
    {
        if (type == QuickSlotType.Use)
        {
            gameManager.SaveManager.Save.QuickSlot[index] = id;
            gameManager.SaveManager.SaveData();
        }
        else if (type == QuickSlotType.Skill)
        {
            // slots[index] = id * -1;
            gameManager.SaveManager.Save.QuickSlot[index] = id * -1;
            gameManager.SaveManager.SaveData();
        }
    }

    public void DeleteQuickSlot(int index, bool save = true)
    {
        slots[index].Clear();
        gameManager.SaveManager.Save.QuickSlot[index] = -1;
        if (save)
        {
            gameManager.SaveManager.SaveData();
        }
    }

    public void CheckIsQuickSlot(int slotId, bool save = true)
    {
        // 퀵슬롯 등록 시 이 함수를 호출해서 중복된 슬롯을 삭제해야 함
        // 삭제 및 판매하는 아이템 슬롯 id가 use인 경우 해당 함수 호출해야 함
        for (int i = 0; i < gameManager.SaveManager.Save.QuickSlot.Count; i++)
        {
            if (gameManager.SaveManager.Save.QuickSlot[i] == slotId)
            {
                DeleteQuickSlot(i, save);
                return;
            }
        }
    }

    public void CloseSetQuickSlotMode()
    {
        defaultMode = true;
        Close();
    }

    // -------------------------------------------------------------
    // UI gameObject (use on UIController.cs)
    // -------------------------------------------------------------
    public void Open(bool isDefaultMode = true)
    {
        defaultMode = isDefaultMode;
        goSetModeBackground.SetActive(!defaultMode);
        this.gameObject.SetActive(true);
    }

    public void Close()
    {
        goSetModeBackground.SetActive(false);
        this.gameObject.SetActive(false);
    }

    public void ShowItemDetail(Item item, Vector3 pos)
    {
        gameManager.UIController.OpenItemDetail(item, pos);
    }

    public void CloseItemDetail()
    {
        gameManager.UIController.CloseItemDetail();
    }
}
