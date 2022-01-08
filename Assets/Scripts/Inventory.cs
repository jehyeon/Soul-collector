using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;
using LitJson;
public class Inventory : MonoBehaviour
{
    // UI 창 활성화 여부
    private bool inventoryActivated = false;
    private bool statDetailActivated = false;

    // 캐릭터 스탯 UI
    [SerializeField]
    private GameObject go_statDetail;

    // 아이템 detail UI
    [SerializeField]
    private GameObject go_itemDetail;
    [SerializeField]
    private Image img_itemDetailBackground;
    [SerializeField]
    private Image img_itemDetailImage;
    [SerializeField]
    private Image img_itemDetailFrame;
    [SerializeField]
    private TextMeshProUGUI text_itemDetailName;

    [SerializeField]
    private TextMeshProUGUI text_itemDetailDes;

    // 인벤토리 UI
    [SerializeField] 
    private GameObject go_inventoryBase;
    [SerializeField]
    private GameObject go_SlotsParent;
    
    // 캐릭터 체력 및 Gold
    [SerializeField]
    private TextMeshProUGUI text_gold;      // Text Gold
    [SerializeField]
    private TextMeshProUGUI text_damageReduction;      // Text damage reduction

    [SerializeField]
    public GameObject go_player;

    private int gold;   // 골드

    // 인벤토리 Slot
    private Slot[] slots;
    private int index;  // 마지막 슬롯 index

    public int[] equipped;  // 장착정보
    private int selectedSlotIndex;      // 선택된 index

    // For save and load
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

        // 세이브 파일 로드, 없으면 생성
        string path = Path.Combine(Application.dataPath, "save.json");
        FileInfo fileInfo = new FileInfo(path);
        if (fileInfo.Exists)
        {
            saveManager.Load();
        }
        else
        {
            saveManager.Init();
        }

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
        go_statDetail.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = go_player.GetComponent<Player>()._stat.ToString();
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
    public void OpenItemDetail(Item item)
    {
        img_itemDetailImage.sprite = item.ItemImage;
        img_itemDetailFrame.sprite = item.ItemFrame;
        img_itemDetailBackground.color = item.BackgroundColor;
        text_itemDetailName.text = item.ItemName;
        text_itemDetailName.color = item.FontColor;
        text_itemDetailDes.text = item.ToString();

        go_itemDetail.SetActive(true);
    }

    public void CloseItemDetail()
    {
        go_itemDetail.SetActive(false);
        slots[selectedSlotIndex].UnSelect();

        // img_itemDetailImage.sprite = null;
        // img_itemDetailFrame.sprite = null;
        // img_itemDetailBackground.color = null;
        // text_itemDetailName.text = "";
        // text_itemDetailDes.text = "";
    }

    // 아이템 획득
    public bool AcquireItem(int _id, int _count = 1)
    {
        if (_id >= 1600)
        {
            // resource인 경우 (item id 1600 이후가 resource)
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

    public void UpdateSelect(int slotIndex)
    {
        slots[selectedSlotIndex].UnSelect();
        selectedSlotIndex = slotIndex;
    }

    public void ClickInventoryBtn()
    {
        if (slots[selectedSlotIndex].item.ItemType > 12)
        {
            // 장비, 사용 아이템이 아닌 경우
            return;
        }

        if (slots[selectedSlotIndex].item.ItemType < 12)
        {
            // selectedSlotIndex 의 slot이 선택 되었을 때
            if (slots[selectedSlotIndex].isEquip)
            {
                slots[selectedSlotIndex].UnEquip();
            }
            else
            {
                slots[selectedSlotIndex].Equip();
            }
        }
        else if (slots[selectedSlotIndex].item.ItemType == 12)
        {
            slots[selectedSlotIndex].Use();
        }

        return;
    }
}
