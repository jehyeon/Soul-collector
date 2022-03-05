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
    private GameObject goInventoryActBtn;
    [SerializeField]
    private TextMeshProUGUI textInventoryActBtn;

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

    // 삭제
    private bool deleteItemMode;

    // -------------------------------------------------------------
    // Init, Update
    // -------------------------------------------------------------
    void Awake()
    {
        // save로부터 초기화가 안되는 field 초기화
        selectedSlotIndex = -1;
        selectedSlotIndexList = new List<int>();
        multiSelectMode = false;
        deleteItemMode = false;
    }
    
    public void InitInventorySlots()
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

    public void LoadInventory()
    {
        // Save Inventory와 동기화
        for (int i = 0; i < gameManager.SaveManager.Save.InventorySize; i++)
        {   
            // 인벤토리 크기만큼 한번만 loop (inventory clear 후 아이템 재생성)
            if (slots[i].Item != null)
            {
                slots[i].ClearSlot();
            }

            if (i < gameManager.SaveManager.Save.LastSlotIndex)
            {
                slots[i].Load(
                    gameManager.SaveManager.Save.Slots[i].Id, 
                    gameManager.ItemManager.Get(gameManager.SaveManager.Save.Slots[i].ItemId),
                    gameManager.SaveManager.Save.Slots[i].Count
                );
            }
        }
    }

    public void LoadEquipInfo()
    {
        for (int i = 0; i < gameManager.SaveManager.Save.Equipped.Count; i++)
        {
            if (gameManager.SaveManager.Save.Equipped[i] != -1)
            {
                int slotIndex = FindItemUsingSlotId(gameManager.SaveManager.Save.Equipped[i]);
                if (slotIndex != -1)
                {
                    slots[slotIndex].Equip();
                    gameManager.Player.Equip(slots[slotIndex].Item);
                }    
            }
        }

        gameManager.Player.Heal(9999); // 장비 정보 로드할 때에는 풀피로
    }

    // -------------------------------------------------------------
    // 아이템 획득
    // -------------------------------------------------------------
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

    
    // -------------------------------------------------------------
    // 인벤토리 UI action (Slot, Button)
    // -------------------------------------------------------------
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

            UpdateInventoryActBtn();
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

            UpdateInventoryActBtn();
            itemDetail.Close();     // 선택된 슬롯이 없으므로 detail ui 닫기
        }
    }

    private void ResetSelectSlot()
    {
        if (selectedSlotIndex != -1)
        {
            slots[selectedSlotIndex].UnSelect();
            UnSelectSlot(selectedSlotIndex);
            return;
        }
        
        if (selectedSlotIndexList.Count > 0)
        {
            foreach(int index in selectedSlotIndexList)
            {
                slots[index].UnSelect();
            }
            selectedSlotIndexList = new List<int>();
            UpdateInventoryActBtn();
            itemDetail.Close();     // 선택된 슬롯이 없으므로 detail ui 닫기
        }
    }

    private void UpdateInventoryActBtn()
    {
        // 인벤토리 버튼 관련 !!! temp
        if (deleteItemMode)
        {
            goInventoryActBtn.SetActive(true);
            textInventoryActBtn.text = "삭제";
            return;
        }
        
        if (selectedSlotIndex == -1)
        {
            goInventoryActBtn.SetActive(false);
            textInventoryActBtn.text = "";
            return;
        }

        if (slots[selectedSlotIndex].IsEquip)
        {
            goInventoryActBtn.SetActive(true);
            textInventoryActBtn.text = "장착 해제";
        }
        else
        {
            goInventoryActBtn.SetActive(true);
            textInventoryActBtn.text = "장착";
        }
    }

    public void ClickInventoryActBtn()
    {
        // 인벤토리 버튼 클릭 이벤트 !!! 모드 따라 다름
        if (deleteItemMode)
        {
            // 아이템 삭제하기
            Delete(selectedSlotIndexList);
            return;
        }

        if (selectedSlotIndex == -1)
        {
            return;
        }

        int partNum = slots[selectedSlotIndex].Item.PartNum;
        if (!slots[selectedSlotIndex].IsEquip)
        {
            // 장착
            if (gameManager.SaveManager.Save.Equipped[partNum] != -1)
            {
                // 동일 파츠 아이템을 장착한 경우 UnEquip();
                int slotId = gameManager.SaveManager.Save.Equipped[partNum];
                int slotIndex = FindItemUsingSlotId(slotId);
                if (slotIndex != -1)
                {
                    slots[slotIndex].UnEquip();     // only view
                    gameManager.SaveManager.Save.Equipped[partNum] = -1;
                }
            }
            slots[selectedSlotIndex].Equip();   // only view
            gameManager.SaveManager.Save.Equipped[partNum] = slots[selectedSlotIndex].Id;
            gameManager.Player.Equip(slots[selectedSlotIndex].Item);   // stat 업데이트
            gameManager.SaveManager.SaveData();
        }
        else
        {
            // 장착 해제
            slots[selectedSlotIndex].UnEquip();     // only view
            gameManager.SaveManager.Save.Equipped[partNum] = -1;
            gameManager.Player.UnEquip(slots[selectedSlotIndex].Item);   // stat 업데이트
            gameManager.SaveManager.SaveData();
        }

        UpdateInventoryActBtn();
    }

    
    // -------------------------------------------------------------
    // Delete Item
    // -------------------------------------------------------------
    public void ClickDeleteBtn()
    {
        // 인벤토리 우측 하단 삭제 버튼 클릭 시
        if (!deleteItemMode)
        {
            StartDeleteMode();
        }
        else
        {
            EndDeleteMode();
        }
    }

    private void StartDeleteMode()
    {
        ResetSelectSlot();
        deleteItemMode = true;
        multiSelectMode = true;

        // !!! 삭제 모드 UI 띄우기
        UpdateInventoryActBtn();
    }

    public void EndDeleteMode()
    {
        deleteItemMode = false;
        multiSelectMode = false;
        ResetSelectSlot();
        selectedSlotIndexList = new List<int>();
    }

    // private void Delete(int wantToDeleteItemIndex)
    // {
    //     ResetSelectSlot();

    //     gameManager.SaveManager.Save.DeleteSlot(wantToDeleteItemIndex);
    //     gameManager.SaveManager.SaveData();
    //     LoadInventory();
    // }

    private void Delete(List<int> wantToDeleteItemIndeices)
    {
        ResetSelectSlot();

        // 아이템이 삭제되면 index가 1씩 줄어들어서 diff로 조정
        wantToDeleteItemIndeices.Sort();
        int indexDiff = 0;  
        foreach(int index in wantToDeleteItemIndeices)
        {
            gameManager.SaveManager.Save.DeleteSlot(index - indexDiff);
            indexDiff += 1;
        }
        gameManager.SaveManager.SaveData();
        LoadInventory();
    }

    // -------------------------------------------------------------
    // 아이템 구매 (Decrease gold + Add item)
    // -------------------------------------------------------------
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

    // -------------------------------------------------------------
    // Search
    // -------------------------------------------------------------
    public bool isFullInventory()
    {
        // 인벤토리 꽉 찼는지 확인
        if (gameManager.SaveManager.Save.LastSlotIndex > gameManager.SaveManager.Save.InventorySize - 1)
        {
            return true;
        }
        
        return false;
    }

    private int FindItemUsingSlotId(int slotId)
    {
        if (slotId == -1)
        {
            return -1;
        }

        for (int i = 0; i < gameManager.SaveManager.Save.LastSlotIndex; i++)
        {
            if (gameManager.SaveManager.Save.Slots[i].Id == slotId)
            {
                return i;
            }
        }

        return -1;
    }

    // -------------------------------------------------------------
    // Gold
    // -------------------------------------------------------------
    public void UpdateGold(int amount)
    {
        gameManager.SaveManager.Save.Gold += amount;
        text_gold.text = string.Format("{0:#,0}", gameManager.SaveManager.Save.Gold).ToString();
    }

    // -------------------------------------------------------------
    // UI gameObject (use on UIController.cs)
    // -------------------------------------------------------------
    public void Open()
    {
        this.gameObject.SetActive(true);
    }

    public void Close()
    {
        this.gameObject.SetActive(false);
        EndDeleteMode();
        itemDetail.Close();     // Close Item detail UI
    }

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

}
