using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;
// using LitJson;

public class Inventory : MonoBehaviour
{
    // UI 창 활성화 여부
    private bool inventoryActivated = false;
    private bool statDetailActivated = false;
    private bool shopActivated = false;
    private bool craftActivated = false;

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

    // Craft UI
    [SerializeField]
    private GameObject go_craftUI;

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

    // 강화 UI
    [SerializeField]
    private GameObject go_reinforce;

    // Manager
    public SaveManager saveManager;
    private DropManager dropManager;

    // 인벤토리 Slot, 골드, Save
    private Slot[] slots;
    private int selectedSlotIndex;      // 선택된 index
    public int scrollSlotIndex;     // 주문서가 저장된 slot index
    public int scrollType;          // scrollSlotIndex의 주문서가 어떤 주문서 타입인지
    public bool reinforceMode;

    void Awake()
    {
        // save로부터 초기화가 안되는 field 초기화
        slots = go_SlotsParent.GetComponentsInChildren<Slot>();
        selectedSlotIndex = -1;
        reinforceMode = false;
        scrollSlotIndex = -1;
        scrollType = -1;

        dropManager = gameObject.AddComponent<DropManager>();
    }

    void Start()
    {
        saveManager = new SaveManager();
        Load();

        UpdateStatDetail();
    }

    void Update()
    {
        // !!! 키보드 이벤트를 한 곳으로 옮기고 컨트롤러 따로 만들기
        UseInventory();
        UseStatDetail();
        UseShop();
        UseCraft();
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

    private void UseCraft()
    {
        // Temp
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (craftActivated)
            {
                CloseCraftUI();
            } 
            else
            {
                OpenCraftUI();
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

        // 강화 창 닫음
        CloseReinforceUI();
    }

    // Shop UI
    private void OpenShop()
    {
        go_shop.SetActive(true);
        shopActivated = true;

        CloseReinforceUI();
        CloseItemDetail();
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
            // 선택한 슬롯이 있는 경우 UnSelect()
            slots[selectedSlotIndex].UnSelect();
            // selectedSlotIndex는 -1로 초기화 됨
        }
    }

    // Craft UI
    private void OpenCraftUI()
    {
        craftActivated = true;
        go_craftUI.SetActive(craftActivated);
    }
    public void CloseCraftUI()
    {
        craftActivated = false;
        go_craftUI.SetActive(craftActivated);
    }

    // 아이템 획득
    public bool AcquireItem(int _id, int _count = 1)
    {
        if (_id >= 1600)
        {
            // resource인 경우 (item id 1600 이후가 resource)
            for (int i = 0; i < saveManager.Save.LastSlotIndex; i++)
            {
                if (slots[i].Item != null && slots[i].ItemId == _id)
                {
                    // 같은 resource의 아이템이 인벤토리에 이미 있는 경우, 수량만 바꿈
                    slots[i].SetSlotCount(_count);
                    saveManager.Save.Slots[i].UpdateCount(_count);

                    return true;
                }
            }
        }

        // 인벤토리 꽉 찼는지 확인
        if (saveManager.Save.LastSlotIndex >= slots.Length)
        {
            return false;
        }

        slots[saveManager.Save.LastSlotIndex].AddItem(_id, _count);

        saveManager.Save.AddSlot(_id, _count);

        return true;
    }

    public void UnEquipItemType(int itemType)
    {
        saveManager.Save.Equipped[itemType] = -1;
    }

    public void EquipItemType(int itemType, int slotId)
    {
        if (saveManager.Save.Equipped[itemType] != -1)
        {
            // 해당 파츠를 착용하고 있으면 먼저 UnEquip
            slots[saveManager.Save.Equipped[itemType]].UnEquip();
        }

        saveManager.Save.Equipped[itemType] = slotId;
    }

    public void UpdateGold(int droppedGold)
    {
        saveManager.Save.Gold += droppedGold;

        text_gold.text = saveManager.Save.GoldText;
    }

    private void LoadInventory()
    {
        // saveManager.Save.Slots 기준으로 inventory clear 후 아이템 재생성
        for (int i = 0; i < saveManager.Save.LastSlotIndex; i++)
        {
            // slots[i].AddItem()
        }

        // // 골드
        // gold = saveManager.Save.Gold;
        // text_gold.text = saveManager.Save.GoldText;

        // // 인벤토리
        // index = saveManager.save.slotIndex;
        // for (int i = 0; i < index; i++)
        // {
        //     // save 데이터로 부터 받아온 값을 다시 slot에 add
        //     slots[i].AddItem(saveManager.save.slots[i].id, saveManager.save.slots[i].count);
        // }

        // // 장착 정보
        // equipped = saveManager.Save.Equipped;
    }

    private void Reload()
    {
        // Slot all clear
        for (int i = 0; i < index; i++)
        {
            slots[i].ClearSlot();
        }

        Load();     // 슬롯 비우고 다시 load
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
        if (selectedSlotIndex == -1)
        {
            return;
        }

        if (slots[selectedSlotIndex].Item.ItemType == 12)
        {
            // 장비, 사용 아이템이 아닌 경우
            return;
        }

        if (slots[selectedSlotIndex].Item.ItemType < 12)
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
        else if (slots[selectedSlotIndex].Item.ItemType > 12)
        {
            if (index > 59 && slots[selectedSlotIndex].Item.ItemType == 13)
            {
                // 상자 아이템인 경우 인벤토리가 꽉찰 때 사용 안됨
                return;
            }
            Use(slots[selectedSlotIndex].ItemId, slots[selectedSlotIndex].Item.ItemType);
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

    public void Delete(int slotIndex = -1)
    {
        // save의 배열이 바뀌게 되므로 Reload()를 해줘야 함
        // All slot clear -> Load()

        // Select slot item 삭제, 개수 선택 미구현
        if (slotIndex == -1)
        {
            // 파라미터 없이 호출되었을 때에는 selectedSlotIndex를 삭제

            if (selectedSlotIndex == -1)
            {
                // 선택한 Slot이 없는 경우 삭제 안됨
                return;    
            }

            slotIndex = selectedSlotIndex;
        }       

        // 아이템 삭제 후 save
        saveManager.Delete(slotIndex);
        saveManager.Save();

        // 선택한 ItemDetail 닫음
        CloseItemDetail();

        Reload();
    }

    public void Use(int itemId, int itemType)
    {
        // 13 - 뽑기 아이템, 14 - 체력 회복 아이템, 15 - 강화 아이템
        if (itemType == 14)
        {
            // 우선 테이블을 쓰지 않음
            if (itemId == 1620)
            {
                go_player.GetComponent<Stat>().Heal(50);
                slots[selectedSlotIndex].SetSlotCount(-1);
                if (slots[selectedSlotIndex].ItemCount < 1)
                {
                    Delete();
                }
            }
            else if (itemId == 1621)
            {
                go_player.GetComponent<Stat>().Heal(200);
                slots[selectedSlotIndex].SetSlotCount(-1);
                if (slots[selectedSlotIndex].ItemCount < 1)
                {
                    Delete();
                }
            }
        }
        else if (itemType == 13)
        {
            if (itemId == 1623)
            {
                // 방어구 상자
                GambleBox(1);
            }
            else if (itemId == 1625)
            {
                // 무기 상자
                GambleBox(2);
            }
        }
        else if (itemType >= 15 && itemType <= 18)
        {
            Reinforce(itemType);
        }
    }

    public void GambleBox(int dropId)
    {
        slots[selectedSlotIndex].SetSlotCount(-1);
        if (slots[selectedSlotIndex].ItemCount < 1)
        {
            Delete();
        }
        int itemId = dropManager.RandomItem(dropId);
        AcquireItem(itemId);    // 추가 후 저장
    }

    public void Reinforce(int _scrollType)
    {
        // 강화 모드
        reinforceMode = true;
        scrollSlotIndex = selectedSlotIndex;    // 마지막에 선택한 슬롯의 index
        scrollType = _scrollType;

        // UI 모두 닫고, unselect
        OpenReinforceUI();
        CloseItemDetail();
        CloseShop();
        CloseStatDetail();
    }

    public void OpenReinforceUI()
    {
        go_reinforce.SetActive(true);
    }
    public void CloseReinforceUI()
    {
        go_reinforce.SetActive(false);
        reinforceMode = false;
        scrollSlotIndex = -1;
        scrollType = -1;
    }

    public void Rush(int slotIndex)
    {
        if (scrollSlotIndex == -1)
        {
            // Error
            return;
        }

        slots[scrollSlotIndex].SetSlotCount(-1);
        if (slots[scrollSlotIndex].ItemCount < 1) 
        {
            // scroll이 없어지면서 slotIndex가 바뀔 수도 있음
            int beforeSlotId = saveManager.save.slots[slotIndex].slotId;

            Delete(scrollSlotIndex);
            CloseReinforceUI();       // 스크롤 인덱스가 초기화

            slotIndex = FindItemUsingSlotId(beforeSlotId);  // slotIndex 업데이트
        }
        else
        {
            // 스크롤 갯수가 1 이상 남아있는 경우 save 업데이트
            saveManager.UpdateCount(scrollSlotIndex, -1);
        }

        float rand = Random.value;
        float percent = 0f;
        if (slots[slotIndex].upgradeLevel % 2 == 0)
        {
            percent = .5f;
        }
        else
        {
            percent = .33f;
        }

        if (rand < percent)
        {
            // 강화 성공
            slots[slotIndex].Upgrade();
            saveManager.save.slots[slotIndex].id += 1;
            saveManager.Save();

            Reload();
        }
        else
        {
            // 강화 실패
            int beforeScrollSlotId = -1;
            if (scrollSlotIndex != -1)
            {
                // 강화 아이템이 없어지면서 scrollSlotIndex가 바뀔 수도 있음
                beforeScrollSlotId = saveManager.save.slots[scrollSlotIndex].slotId;
            }

            Delete(slotIndex);

            scrollSlotIndex = FindItemUsingSlotId(beforeScrollSlotId);  // scrollSlotIndex 업데이트
        }
    }

    public void SetItemCount(int slotIndex, int count)
    {
        slots[slotIndex].SetSlotCount(count);
        if (slots[slotIndex].ItemCount < 1) 
        {
            Delete(slotIndex);
        }
        else
        {
            Save();
        }
    }

    public int FindItemUsingItemId(int itemId)
    {
        if (itemId == -1)
        {
            return -1;
        }

        for (int i = 0; i < index; i++)
        {
            if (slots[i].ItemId == itemId)
            {
                return i;
            }
        }

        return -1;
    }

    private int FindItemUsingSlotId(int slotId)
    {
        if (slotId == -1)
        {
            return -1;
        }

        for (int i = 0; i < index; i++)
        {
            if (saveManager.save.slots[i].slotId == slotId)
            {
                return i;
            }
        }

        return -1;
    }

    public int GetAmountItem(int itemId)
    {
        for (int i = 0; i < saveManager.save.slotIndex; i++)
        {
            if (saveManager.save.slots[i].id == itemId)
            {
                return saveManager.save.slots[i].count;
            }
        }
        
        return 0;
    }

    public bool CheckEnoughInventory()
    {
        if (index >= slots.Length)
        {
            return false;
        }
        
        return true;
    }
}
