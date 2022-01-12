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
    private bool shopActivated = false;

    // 캐릭터 스탯 UI
    [SerializeField]
    private GameObject go_statDetail;
    
    // Shop UI
    [SerializeField]
    private GameObject go_shop;

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
    public int selectedSlotIndex;      // 선택된 index

    // For save and load
    public SaveManager saveManager;

    void Awake()
    {
        slots = go_SlotsParent.GetComponentsInChildren<Slot>();
        index = 0;
        gold = 0;
        selectedSlotIndex = -1;

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
        UseShop();
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

    private void UseShop()
    {
        // Temp
        if (Input.GetKeyDown(KeyCode.S))
        {
            if (shopActivated)
            {
                CloseShop();
            } 
            else
            {
                OpenShop();
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

    // Shop UI
    private void OpenShop()
    {
        go_shop.SetActive(true);
        shopActivated = true;
    }

    public void CloseShop()
    {
        go_shop.SetActive(false);
        shopActivated = false;
        // shop UI를 닫으면 selected 아이템 초기화
        go_shop.transform.GetChild(2).GetChild(0).GetComponent<Shop>().UnSelect();
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

        if (selectedSlotIndex != -1)
        {
            slots[selectedSlotIndex].UnSelect();
        }
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
        string text = string.Format("{0:#,###}", gold).ToString();
        
        if (text == "")
        {
            text = "0";
        }
        text_gold.text = text;
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
        string text = string.Format("{0:#,###}", gold).ToString();
        
        if (text == "")
        {
            text = "0";
        }
        text_gold.text = text;

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
        if (slotIndex != -1 && selectedSlotIndex != -1)
        {
            // 전에 장착한 아이템이 있는 경우
            slots[selectedSlotIndex].UnSelect();
        }
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

    public void Buy(int itemId, int price)
    {
        if (gold < price)
        {
            // 돈이 부족하다는 pop_up 띄우기
            return;
        }


        // 우선 한개만 구입가능함
        // 인벤토리 초과 시  false return
        bool result = AcquireItem(itemId, 1);
        if (result)
        {
            // 정상 구매인 경우에만 돈 차감
            UpdateGold(-1 * price);
        }
    }

    public void Delete()
    {
        if (selectedSlotIndex == -1)
        {
            return;    
        }

        // 아이템 삭제 후 save
        saveManager.Delete(selectedSlotIndex);
        saveManager.Save();

        // 선택한 ItemDetail 닫음
        CloseItemDetail();

        // Slot all clear
        for (int i = 0; i < index; i++)
        {
            slots[i].ClearSlot();
        }

        Load();     // 슬롯 비우고 다시 load
    }
}
