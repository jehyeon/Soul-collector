using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;
// using LitJson;

public class Inventory : MonoBehaviour
{
    [SerializeField]
    private GameManager gameManager;
    
    [SerializeField]
    private TextMeshProUGUI text_gold;      // Text Gold
    [SerializeField]
    private ItemDetail itemDetail;

    // Slot pref
    [SerializeField]
    private GameObject go_slotParent;
    [SerializeField]
    private GameObject go_inventorySlotPref;

    // slots
    private InventorySlot[] slots;

    // 슬롯 선택
    private bool multiSelectMode;               // 다중 선택 모드 (삭제, 강화)
    private int selectedSlotIndex;              // 선택된 index (multiSelectMode == false)
    private List<int> selectedSlotIndexList;    // 선택된 index list (multiSelectMode == true)

        public int scrollSlotIndex;     // 주문서가 저장된 slot index
    public int scrollType;          // scrollSlotIndex의 주문서가 어떤 주문서 타입인지
    public bool reinforceMode;

    void Awake()
    {
        // save로부터 초기화가 안되는 field 초기화
        selectedSlotIndex = -1;
        selectedSlotIndexList = new List<int>();
        reinforceMode = false;
        multiSelectMode = false;
        scrollSlotIndex = -1;
        scrollType = -1;

        // dropManager = gameObject.AddComponent<DropManager>();
    }

    void Start()
    {
        InitInventorySlots();
        LoadInventory();
    }
    
    private void InitInventorySlots()
    {
        // 인벤토리 크기 만큼 슬롯 생성
        for (int i = 0; i < gameManager.SaveManager.Save.InventorySize; i++)
        {
            GameObject inventorySlot = Instantiate(go_inventorySlotPref);
            inventorySlot.transform.SetParent(go_slotParent.transform);
            inventorySlot.GetComponent<InventorySlot>().Init(i, this.GetComponent<Inventory>());
        }

        // 생성된 슬롯을 slots에 저장
        slots = go_slotParent.GetComponentsInChildren<InventorySlot>();
    }
    
    // 인벤토리 꽉 찼는지 확인
    public bool isFullInventory()
    {
        if (gameManager.SaveManager.Save.LastSlotIndex >= gameManager.SaveManager.Save.InventorySize - 1)
        {
            return true;
        }
        
        return false;
    }

    // 아이템 획득
    public void AcquireItem(Item item, int count = 1)
    {
        // 인벤토리 Full이면 해당 함수 호출 안됨
        if (item.ItemType == ItemType.Material || item.ItemType == ItemType.Use)
        {
            // 중첩 가능한 아이템만 확인
            // Inventory slot save 데이터에 동일한 아이템이 있는지 확인
            for (int i = 0; i < gameManager.SaveManager.Save.LastSlotIndex; i++)
            {
                if (gameManager.SaveManager.Save.Slots[i].ItemId == item.Id)
                {
                    // 수량만 변경
                    slots[i].SetSlotCount(slots[i].Count + count);  // only view
                    gameManager.SaveManager.Save.Slots[i].UpdateCount(count);

                    return;
                }
            }
        }
        
        slots[gameManager.SaveManager.Save.LastSlotIndex].Set(item, count);     // only view
        gameManager.SaveManager.Save.AddItem(item.Id, count);
    }

    // 슬롯 관련
    public void SelectSlot(int slotIndex)
    {
        if (multiSelectMode)
        {
            selectedSlotIndexList.Add(slotIndex);
        }
        else
        {
            if (selectedSlotIndex != -1)
            {
                slots[selectedSlotIndex].UnSelect();    // 기존에 선택된 슬롯 선택 해제
            }
            selectedSlotIndex = slotIndex;
            itemDetail.Open(slots[selectedSlotIndex].Item);
        }
    }

    public void UnSelectSlot(int slotIndex)
    {
        if (multiSelectMode)
        {
            selectedSlotIndexList.Remove(slotIndex);
        }
        else
        {
            selectedSlotIndex = -1;
        }
    }
    // UnEquip
//     cv.GetComponent<Inventory>().UnEquipItemType(item.ItemType);    // 해당 파츠 아이템을 UnEquip
    //     cv.GetComponent<Inventory>().go_player.GetComponent<Stat>().UnEquip(item);
    //     cv.GetComponent<Inventory>().UpdateStatDetail();
    //      slot.UnEquip()  // 그냥 equip표시만 없앰
    //     SetInventoryBtn();


    // public void UnEquipItemType(int itemType)
    // {
    //     saveManager.Save.Equipped[itemType] = -1;
    //     saveManager.SaveData();
    // }

    // public void EquipItemType(int itemType, int slotId)
    // {
    //     if (saveManager.Save.Equipped[itemType] != -1)
    //     {
    //         // 해당 파츠를 착용하고 있으면 먼저 UnEquip
    //         slots[saveManager.Save.Equipped[itemType]].UnEquip();
    //     }

    //     saveManager.Save.Equipped[itemType] = slotId;
    //     saveManager.SaveData();
    // }

    public void UpdateGold(int amount)
    {
        gameManager.SaveManager.Save.Gold += amount;
        text_gold.text = string.Format("{0:#,0}", gameManager.SaveManager.Save.Gold).ToString();
    }

    private void LoadInventory()
    {
        // SaveManager slot과 동기화

        // inventory clear 후 아이템 재생성
        for (int i = 0; i < gameManager.SaveManager.Save.LastSlotIndex; i++)
        {   
            // 인벤토리 크기만큼 한번만 loop
            if (slots[i].Item != null)
            {
                slots[i].ClearSlot();
            }

            slots[i].Load(
                gameManager.SaveManager.Save.Slots[i].Id, 
                gameManager.ItemManager.Get(gameManager.SaveManager.Save.Slots[i].ItemId),
                gameManager.SaveManager.Save.Slots[i].Count
            );
        }

        // 장착 정보 로드
    }

    // public void ClickInventoryBtn()
    // {
    //     if (selectedSlotIndex == -1)
    //     {
    //         return;
    //     }

    //     if (slots[selectedSlotIndex].Item.ItemType == 12)
    //     {
    //         // 장비, 사용 아이템이 아닌 경우
    //         return;
    //     }

    //     if (slots[selectedSlotIndex].Item.ItemType < 12)
    //     {
    //         // selectedSlotIndex 의 slot이 선택 되었을 때
    //         if (slots[selectedSlotIndex].isEquip)
    //         {
    //             slots[selectedSlotIndex].UnEquip();
    //         }
    //         else
    //         {
    //             slots[selectedSlotIndex].Equip();
    //         }
    //     }
    //     else if (slots[selectedSlotIndex].Item.ItemType > 12)
    //     {
    //         if (index > 59 && slots[selectedSlotIndex].Item.ItemType == 13)
    //         {
    //             // 상자 아이템인 경우 인벤토리가 꽉찰 때 사용 안됨
    //             return;
    //         }
    //         Use(slots[selectedSlotIndex].ItemId, slots[selectedSlotIndex].Item.ItemType);
    //     }

    //     return;
    // }

    public void Buy(Item item, int price)
    {
        if (gameManager.SaveManager.Save.Gold < price)
        {
            // !!! 돈이 부족하다는 pop_up 띄우기
            return;
        }
        // 연속 구매를 해도 1개씩 Buy 호출
        if (isFullInventory())
        {
            return;
        }
        // 아이템 추가, 골드 감소, 세이브 저장
        AcquireItem(item);
        UpdateGold(-1 * price);
        gameManager.SaveManager.SaveData();
    }

    // public void Delete(int slotIndex = -1)
    // {
    //     // save의 배열이 바뀌게 되므로 Reload()를 해줘야 함
    //     // All slot clear -> Load()

    //     // Select slot item 삭제, 개수 선택 미구현
    //     if (slotIndex == -1)
    //     {
    //         // 파라미터 없이 호출되었을 때에는 selectedSlotIndex를 삭제

    //         if (selectedSlotIndex == -1)
    //         {
    //             // 선택한 Slot이 없는 경우 삭제 안됨
    //             return;    
    //         }

    //         slotIndex = selectedSlotIndex;
    //     }       

    //     // 아이템 삭제 후 save
    //     saveManager.Delete(slotIndex);
    //     saveManager.Save();

    //     // 선택한 ItemDetail 닫음
    //     CloseItemDetail();

    //     LoadInventory();
    // }

    // public void Use(int itemId, int itemType)
    // {
    //     // 13 - 뽑기 아이템, 14 - 체력 회복 아이템, 15 - 강화 아이템
    //     if (itemType == 14)
    //     {
    //         // 우선 테이블을 쓰지 않음
    //         if (itemId == 1620)
    //         {
    //             go_player.GetComponent<Stat>().Heal(50);
    //             slots[selectedSlotIndex].SetSlotCount(-1);
    //             if (slots[selectedSlotIndex].ItemCount < 1)
    //             {
    //                 Delete();
    //             }
    //         }
    //         else if (itemId == 1621)
    //         {
    //             go_player.GetComponent<Stat>().Heal(200);
    //             slots[selectedSlotIndex].SetSlotCount(-1);
    //             if (slots[selectedSlotIndex].ItemCount < 1)
    //             {
    //                 Delete();
    //             }
    //         }
    //     }
    //     else if (itemType == 13)
    //     {
    //         if (itemId == 1623)
    //         {
    //             // 방어구 상자
    //             GambleBox(1);
    //         }
    //         else if (itemId == 1625)
    //         {
    //             // 무기 상자
    //             GambleBox(2);
    //         }
    //     }
    //     else if (itemType >= 15 && itemType <= 18)
    //     {
    //         Reinforce(itemType);
    //     }
    // }

    // public void GambleBox(int dropId)
    // {
    //     slots[selectedSlotIndex].SetSlotCount(-1);
    //     if (slots[selectedSlotIndex].ItemCount < 1)
    //     {
    //         Delete();
    //     }
    //     int itemId = dropManager.RandomItem(dropId);
    //     AcquireItem(itemId);    // 추가 후 저장
    // }

    // public void Reinforce(int _scrollType)
    // {
    //     // 강화 모드
    //     reinforceMode = true;
    //     scrollSlotIndex = selectedSlotIndex;    // 마지막에 선택한 슬롯의 index
    //     scrollType = _scrollType;

    //     // UI 모두 닫고, unselect
    //     OpenReinforceUI();
    //     CloseItemDetail();
    //     CloseShop();
    //     CloseStatDetail();
    // }

    // public void OpenReinforceUI()
    // {
    //     go_reinforce.SetActive(true);
    // }
    // public void CloseReinforceUI()
    // {
    //     go_reinforce.SetActive(false);
    //     reinforceMode = false;
    //     scrollSlotIndex = -1;
    //     scrollType = -1;
    // }

    // public void Rush(int slotIndex)
    // {
    //     if (scrollSlotIndex == -1)
    //     {
    //         // Error
    //         return;
    //     }

    //     slots[scrollSlotIndex].SetSlotCount(-1);
    //     if (slots[scrollSlotIndex].ItemCount < 1) 
    //     {
    //         // scroll이 없어지면서 slotIndex가 바뀔 수도 있음
    //         int beforeSlotId = saveManager.save.slots[slotIndex].slotId;

    //         Delete(scrollSlotIndex);
    //         CloseReinforceUI();       // 스크롤 인덱스가 초기화

    //         slotIndex = FindItemUsingSlotId(beforeSlotId);  // slotIndex 업데이트
    //     }
    //     else
    //     {
    //         // 스크롤 갯수가 1 이상 남아있는 경우 save 업데이트
    //         saveManager.UpdateCount(scrollSlotIndex, -1);
    //     }

    //     float rand = Random.value;
    //     float percent = 0f;
    //     if (slots[slotIndex].upgradeLevel % 2 == 0)
    //     {
    //         percent = .5f;
    //     }
    //     else
    //     {
    //         percent = .33f;
    //     }

    //     if (rand < percent)
    //     {
    //         // 강화 성공
    //         slots[slotIndex].Upgrade();
    //         saveManager.save.slots[slotIndex].id += 1;
    //         saveManager.Save();

    //         LoadInventory();
    //     }
    //     else
    //     {
    //         // 강화 실패
    //         int beforeScrollSlotId = -1;
    //         if (scrollSlotIndex != -1)
    //         {
    //             // 강화 아이템이 없어지면서 scrollSlotIndex가 바뀔 수도 있음
    //             beforeScrollSlotId = saveManager.save.slots[scrollSlotIndex].slotId;
    //         }

    //         Delete(slotIndex);

    //         scrollSlotIndex = FindItemUsingSlotId(beforeScrollSlotId);  // scrollSlotIndex 업데이트
    //     }
    // }

    // public void SetItemCount(int slotIndex, int count)
    // {
    //     slots[slotIndex].SetSlotCount(count);
    //     if (slots[slotIndex].ItemCount < 1) 
    //     {
    //         Delete(slotIndex);
    //     }
    //     else
    //     {
    //         Save();
    //     }
    // }

    // public int FindItemUsingItemId(int itemId)
    // {
    //     if (itemId == -1)
    //     {
    //         return -1;
    //     }

    //     for (int i = 0; i < index; i++)
    //     {
    //         if (slots[i].ItemId == itemId)
    //         {
    //             return i;
    //         }
    //     }

    //     return -1;
    // }

    // private int FindItemUsingSlotId(int slotId)
    // {
    //     if (slotId == -1)
    //     {
    //         return -1;
    //     }

    //     for (int i = 0; i < index; i++)
    //     {
    //         if (saveManager.save.slots[i].slotId == slotId)
    //         {
    //             return i;
    //         }
    //     }

    //     return -1;
    // }

    // public int GetAmountItem(int itemId)
    // {
    //     for (int i = 0; i < saveManager.save.slotIndex; i++)
    //     {
    //         if (saveManager.save.slots[i].id == itemId)
    //         {
    //             return saveManager.save.slots[i].count;
    //         }
    //     }
        
    //     return 0;
    // }

    // public bool CheckEnoughInventory()
    // {
    //     if (index >= slots.Length)
    //     {
    //         return false;
    //     }
        
    //     return true;
    // }

    // UI gameObject (use on UIController.cs)
    public void Open()
    {
        this.gameObject.SetActive(true);
    }

    public void Close()
    {
        this.gameObject.SetActive(false);
        itemDetail.Close();
    }
}
