using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;
using LitJson;
public class Inventory : MonoBehaviour
{
    private bool inventoryActivated = false;
    private bool statDetailActivated = false;

    [SerializeField]
    private GameObject go_statDetail;
    [SerializeField] 
    private GameObject go_inventoryBase;
    [SerializeField]
    private GameObject go_SlotsParent;
    [SerializeField]
    private GameObject go_itemDetail;

    [SerializeField]
    private TextMeshProUGUI text_gold;      // Text Gold
    [SerializeField]
    private TextMeshProUGUI text_damageReduction;      // Text Gold

    [SerializeField]
    public GameObject go_player;

    public int[] equipped;  // 장착 중인 슬롯의 id

    private int gold;   // 골드
    private Slot[] slots;
    private int index;

    public SaveManager saveManager;

    void Awake()
    {
        slots = go_SlotsParent.GetComponentsInChildren<Slot>();
        index = 0;
        gold = 0;

        equipped = new int[12];
        // 장착 정보를 -1로 초기화
        for (int i = 0; i < equipped.Length; i++)
        {
            equipped[i] = -1;
        }
    }

    void Start()
    {
        saveManager = new SaveManager();

        // save.json이 없으면
        // saveManager.Init();

        // save.json이 있으면
        saveManager.Load(); 
        Load();

        UpdateStatDetail();
    }
    void Update()
    {
        UseInventory();
        UseStatDetail();
    }

    // UI 조작키
    private void UseInventory()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (inventoryActivated)
            {
                CloseInventory();
            } 
            else
            {
                OpenInventory();
            }
        }
    }

    private void UseStatDetail()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (statDetailActivated)
            {
                CloseStatDetail();
            } 
            else
            {
                OpenStatDetail();
            }
        }
    }
    
    // Stat UI
    private void OpenStatDetail()
    {
        go_statDetail.SetActive(true);
        statDetailActivated = true;
    }
    public void CloseStatDetail()
    {
        go_statDetail.SetActive(false);
        statDetailActivated = false;
    }
    public void UpdateStatDetail()
    {
        go_statDetail.transform.GetChild(1).GetComponent<Text>().text = go_player.GetComponent<Player>()._stat.ToString();
        text_damageReduction.text = go_player.GetComponent<Player>()._stat.DamageReduction.ToString();
    }

    // Inventory UI
    private void OpenInventory()
    {
        go_inventoryBase.SetActive(true);
        inventoryActivated = true;
    }

    public void CloseInventory()
    {
        go_inventoryBase.SetActive(false);
        inventoryActivated = false;

        // 아이템 디테일 창도 닫음
        CloseItemDetail();
    }

    // Item detail UI
    public void OpenItemDetail(Item item, Image itemImage)
    {
        go_itemDetail.transform.GetChild(1).GetComponent<Image>().sprite = itemImage.sprite;
        go_itemDetail.transform.GetChild(2).GetComponent<Text>().text = item.ItemName;
        go_itemDetail.transform.GetChild(3).GetComponent<Text>().text = item.ToString();

        go_itemDetail.SetActive(true);
    }

    private void CloseItemDetail()
    {
        go_itemDetail.SetActive(false);
        go_itemDetail.transform.GetChild(1).GetComponent<Image>().sprite = null;
        go_itemDetail.transform.GetChild(2).GetComponent<Text>().text = "";
        go_itemDetail.transform.GetChild(3).GetComponent<Text>().text = "";
    }

    // 아이템 획득
    public bool AcquireItem(int _id, int _count = 1)
    {
        if (_id >= 600)
        {
            // resource인 경우 (item id 600 이후가 resource)
            for (int i = 0; i < index; i++)
            {
                if (slots[i].item != null && slots[i].item.Id == _id)
                {
                    // 같은 resource의 아이템이 인벤토리에 이미 있는 경우
                    // 수량만 바꿈
                    slots[i].SetSlotCount(_count);
                    
                    Save();
                    return true;
                }
            }
        }

        // 인벤토리 꽉 찼는지 확인
        if (index >= slots.Length)
        {
            return false;
        }

        slots[index].AddItem(_id, _count);
        index += 1;

        Save();
        return true;
    }

    public void UnEquipItemType(int itemType)
    {
        equipped[itemType] = -1;
    }

    public void EquipItemType(int itemType, int slotId)
    {
        if (equipped[itemType] != -1)
        {
            // 해당 파츠를 착용하고 있으면 먼저 UnEquip
            slots[equipped[itemType]].UnEquip();
        }

        equipped[itemType] = slotId;
    }

    public void UpdateGold(int droppedGold)
    {
        gold += droppedGold;
        text_gold.text = gold.ToString();   // , 추가하기

        Save();
    }

    public void Save()
    {
        // 골드
        saveManager.save.gold = gold;

        // 인벤토리
        saveManager.save.slotIndex = index;
        for (int i = 0; i < index; i++)
        {
            saveManager.save.slots[i].id = slots[i].item.Id;
            saveManager.save.slots[i].count = slots[i].itemCount;
        }

        saveManager.Save();
    }

    private void Load()
    {
        // 골드
        gold = saveManager.save.gold;
        text_gold.text = gold.ToString();

        // 인벤토리
        index = saveManager.save.slotIndex;
        for (int i = 0; i < index; i++)
        {
            // save 데이터로 부터 받아온 값을 다시 slot에 add
            slots[i].AddItem(saveManager.save.slots[i].id, saveManager.save.slots[i].count);
        }
    }
}
