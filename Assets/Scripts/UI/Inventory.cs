using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;
// using LitJson;

public class Inventory : MonoBehaviour
{
    private Inventory myInventory;

    [SerializeField]
    private GameManager gameManager;
    private UseItemSystem useItemSystem;
    
    [SerializeField]
    private GameObject goInventoryActBtn;
    [SerializeField]
    private TextMeshProUGUI textInventoryActBtn;

    [SerializeField]
    private TextMeshProUGUI text_gold;      // Text Gold
    // 아이템 상세 정보
    [SerializeField]
    private ItemDetail itemDetail;

    // 아이템 강화 UI
    [SerializeField]
    private GameObject go_reinforce;

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

    // 인벤토리 모드
    public bool deleteItemMode;
    public bool reinforceMode;

    // 강화 모드
    public ItemType scrollType;  // 선택된 주문서 타입
    public int scrollSlotId;   // 선택된 주문서의 슬롯 ID

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
        reinforceMode = false;
    }
    
    private void InitInventorySlots()
    {
        if (myInventory == null)
        {
            myInventory = GetComponent<Inventory>();
        }

        // 인벤토리 크기 만큼 슬롯 생성
        for (int i = 0; i < gameManager.SaveManager.Save.InventorySize; i++)
        {
            GameObject inventorySlot = Instantiate(go_inventorySlotPref);
            inventorySlot.transform.SetParent(go_slotParent.transform);
            inventorySlot.GetComponent<InventorySlot>().Init(i, myInventory);
        }

        // 생성된 슬롯을 slots에 저장
        slots = go_slotParent.GetComponentsInChildren<InventorySlot>();
    }

    private void LoadInventory()
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
                    gameManager.SaveManager.Save.Slots[i].Count,
                    gameManager.SaveManager.Save.Slots[i].Level
                );
            }
        }
    }

    // !DELETE
    // private void LoadEquipInfo()
    // {
    //     // 장착 정보 로드
    //     for (int i = 0; i < gameManager.SaveManager.Save.Equipped.Count; i++)
    //     {
    //         if (gameManager.SaveManager.Save.Equipped[i] != -1)
    //         {
    //             int slotIndex = FindItemUsingSlotId(gameManager.SaveManager.Save.Equipped[i]);
    //             if (slotIndex != -1)
    //             {
    //                 slots[slotIndex].Equip();   // equip 체크
    //                 gameManager.Player.Equip(slots[slotIndex].Item);    // 스탯 추가
    //             }    
    //         }
    //     }
    // }

    private void UnEquipAll()
    {
        for (int i = 0; i < gameManager.SaveManager.Save.Equipped.Count; i++)
        {
            if (gameManager.SaveManager.Save.Equipped[i] != -1)
            {
                int slotIndex = FindItemUsingSlotId(gameManager.SaveManager.Save.Equipped[i]);
                if (slotIndex != -1)
                {
                    // slots[slotIndex].UnEquip();     // equip 체크 해제 !!! UNUSED
                    gameManager.Player.UnEquip(slots[slotIndex].Item);  // 스탯 적용
                }    
            }
        }
    }

    private void InitUseItemSystem()
    {
        useItemSystem = new UseItemSystem(gameManager);
    }

    private void SaveAndLoad()
    {
        gameManager.SaveManager.SaveData();
        int hp = gameManager.Player.Stat.Hp;
        // UnEquipAll();
        LoadInventory();
        // LoadEquipInfo();!DELETE
        gameManager.Player.Stat.Hp = hp;    // UnEquipAll, EquipAll로 체력 정보가 수정될 수 있음
    }

    public void StartInventory()
    {
        // GameManager Start()에서 실행
        UpdateGold(0);
        InitInventorySlots();
        LoadInventory();
        // LoadEquipInfo();!DELETE
        gameManager.Player.Heal(99999); // 게임 시작 시 풀피로 시작
        InitUseItemSystem();
    }

    // -------------------------------------------------------------
    // 아이템 획득
    // -------------------------------------------------------------
    public void AcquireItem(Item item, int count = 1)
    {
        // 저장 X
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
                    SetSlotCount(i, count);

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
            // itemDetail.Open(slots[selectedSlotIndex].Item);  // !!! UNUSED

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
            // itemDetail.Close();     // 선택된 슬롯이 없으므로 detail ui 닫기 // !!! UNUSED
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
            // itemDetail.Close();     // 선택된 슬롯이 없으므로 detail ui 닫기  // !!! UNUSED
            return;
        }

        if (selectedSlotIndex == -1)
        {
            // 선택된 슬롯이 없는 경우
            UpdateInventoryActBtn();    // 인벤토리 버튼만 업데이트
        }
    }

    private void UpdateInventoryActBtn()
    {
        // 인벤토리 버튼 활성화 및 text 수정
        // 현재 인벤토리 모드 및 아이템 type에 따라 다름
        if (deleteItemMode)
        {
            goInventoryActBtn.SetActive(true);
            textInventoryActBtn.text = "삭제";
            return;
        }

        if (selectedSlotIndex == -1)
        {
            // 선택된 slot이 없는 경우
            goInventoryActBtn.SetActive(false);
            textInventoryActBtn.text = "";
            return;
        }

        if (slots[selectedSlotIndex].Item.ItemType == ItemType.Use)
        {
            // 사용 아이템인 경우
            goInventoryActBtn.SetActive(true);
            textInventoryActBtn.text = "사용";
        }

        if (slots[selectedSlotIndex].Item.ItemType == ItemType.Weapon 
        || slots[selectedSlotIndex].Item.ItemType == ItemType.Armor)
        {
            // 착용 장비인 경우
            if (slots[selectedSlotIndex].IsEquip)
            {
                // !!!
                goInventoryActBtn.SetActive(true);
                textInventoryActBtn.text = "장착 해제";
            }
            else
            {
                goInventoryActBtn.SetActive(true);
                textInventoryActBtn.text = "장착";
            }
            return;
        }
    }

    public void ClickInventoryActBtn()
    {
        // 인벤토리 버튼 클릭 이벤트
        if (deleteItemMode)
        {
            // 아이템 삭제하기
            Delete(selectedSlotIndexList);
            return;
        }

        if (selectedSlotIndex == -1)
        {
            // 선택된 아이템이 없는 경우, 버튼이 활성화되지 않아 호출도 안됨
            return;
        }

        if (slots[selectedSlotIndex].Item.ItemType == ItemType.Use)
        {
            // 사용 아이템인 경우
            // UseItemSystem으로 사용한 item id 및 slot index 전달
            useItemSystem.Use(slots[selectedSlotIndex].Item.Id, selectedSlotIndex);
            
            UpdateInventoryActBtn();
            return;
        }

        if (slots[selectedSlotIndex].Item.ItemType == ItemType.Weapon 
        || slots[selectedSlotIndex].Item.ItemType == ItemType.Armor)
        {
            // 착용 장비인 경우
            gameManager.SaveManager.Save.DeleteSlot(selectedSlotIndex); // 아이템 삭제 (저장 X)
            gameManager.Equip(slots[selectedSlotIndex].Item);   // 장착 후 저장
            
            ResetSelectSlot();
            LoadInventory();
            // !DELETE
            // int partNum = slots[selectedSlotIndex].Item.PartNum;
            // if (!slots[selectedSlotIndex].IsEquip)
            // {
            //     // 장착
            //     if (gameManager.SaveManager.Save.Equipped[partNum] != -1)
            //     {
            //         // 동일 파츠 아이템을 장착한 경우 UnEquip();
            //         int slotId = gameManager.SaveManager.Save.Equipped[partNum];
            //         int slotIndex = FindItemUsingSlotId(slotId);
            //         if (slotIndex != -1)
            //         {
            //             slots[slotIndex].UnEquip();     // only view
            //             gameManager.SaveManager.Save.Equipped[partNum] = -1;
            //             gameManager.Player.UnEquip(slots[slotIndex].Item);   // stat 업데이트
            //         }
            //     }
            //     slots[selectedSlotIndex].Equip();   // only view
            //     gameManager.SaveManager.Save.Equipped[partNum] = slots[selectedSlotIndex].Id;
            //     gameManager.Player.Equip(slots[selectedSlotIndex].Item);   // stat 업데이트
            //     gameManager.SaveManager.SaveData();
            // }
            // else
            // {
            //     // 장착 해제
            //     slots[selectedSlotIndex].UnEquip();     // only view
            //     gameManager.SaveManager.Save.Equipped[partNum] = -1;
            //     gameManager.Player.UnEquip(slots[selectedSlotIndex].Item);   // stat 업데이트
            //     gameManager.SaveManager.SaveData();
            // }
        }

        UpdateInventoryActBtn();
    }

    // Item detail tooltip
    public void ShowItemDetail(Item item, Vector3 pos)
    {
        itemDetail.Open(item, pos);
    }

    public void CloseItemDetail()
    {
        itemDetail.Close();
    }

    // -------------------------------------------------------------
    // Delete Item
    // -------------------------------------------------------------
    public void ClickDeleteBtn()
    {
        if (reinforceMode)
        {
            // 강화 모드에는 Delete 모드 진입 불가능
            // !!! 삭제 버튼 disable 시키기
            return;
        }

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

    public void Delete(int wantToDeleteItemIndex)
    {
        ResetSelectSlot();

        gameManager.SaveManager.Save.DeleteSlot(wantToDeleteItemIndex);
        SaveAndLoad();
    }

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
        SaveAndLoad();
    }

    // -------------------------------------------------------------
    // Update Count (ex. Use, Craft)
    // -------------------------------------------------------------
    public void UpdateItemCount(int slotIndex, int count = 1)
    {
        // 아이템 수량 조정 (1개 남으면 삭제)
        if (gameManager.SaveManager.Save.Slots[slotIndex].Count <= 1)
        {
            // 1개 남은 경우
            gameManager.SaveManager.Save.DeleteSlot(slotIndex);
            ResetSelectSlot();
            SaveAndLoad();      // 슬롯을 삭제하므로 Save and Load
        }
        else
        {
            SetSlotCount(slotIndex, -1);
            gameManager.SaveManager.SaveData();     // 슬롯 수량만 변경되므로 Load 없이 수정 가능
        }
    }

    public bool TryToUpdateItemCount(int slotIndex, int count = 1)
    {
        // 아이템 수량 조정 (UpdateItemCount 호출 후 Save and Load 해야 함)
        // Slot Delete, Add 모두 호출하는 경우 한번의 Load를 하기 위해 사용
        if (gameManager.SaveManager.Save.Slots[slotIndex].Count <= 1)
        {
            // 1개 남은 경우 fail = true
            return true;
        }
        
        SetSlotCount(slotIndex, -1);    // Save 없음
        
        return false;
    }

    private void SetSlotCount(int slotIndex, int count)
    {
        // slotIndex의 아이템 개수 수정 (+count)
        gameManager.SaveManager.Save.Slots[slotIndex].UpdateCount(count);   // save 데이터 수정 (별도로 save.SaveData()해야 저장이 됨)
        slots[slotIndex].SetSlotCount(count);   // 슬롯 view 수정
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
    // 아이템 강화
    // -------------------------------------------------------------
    public void StartReinforceMode(int slotIndex, ItemType itemType)
    {
        reinforceMode = true;
        // 사용한 주문서 정보
        scrollSlotId = gameManager.SaveManager.Save.Slots[slotIndex].Id;
        scrollType = itemType;

        OpenReinforceUI();
        ResetSelectSlot();
    }

    public void Reinforce(int slotIndex, int itemId)
    {
        bool deleted = false;
        if (!reinforceMode || scrollType == ItemType.None || scrollSlotId == -1)
        {
            // 강화 모드가 아닌 경우 취소
            return;
        }

        // 강화 확률, 확률 수정 시 아래 percent 코드 수정이 필요함
        float percent = slots[slotIndex].Level % 2 == 0 
            ? .5f
            : .33f;

        // 아이템 강화 시도
        if (Random.value < percent)
        {
            // 강화 성공
            slots[slotIndex].Upgrade(gameManager.ItemManager.Get(itemId + 1));
            gameManager.SaveManager.Save.Slots[slotIndex].SetItemId(itemId + 1);
            gameManager.SaveManager.Save.UpdateItemLevel(slotIndex);
            Debug.Log("강화 성공!");
        }
        else
        {
            // 아이템 삭제 (save만 수정)
            gameManager.SaveManager.Save.DeleteSlot(slotIndex);
            deleted = true;
            Debug.Log("강화 실패ㅠㅠ");
        }

        // 강화 주문서 수량 조정
        int scrollIndex = FindItemUsingSlotId(scrollSlotId);
        if (TryToUpdateItemCount(scrollIndex))
        {
            // 주문서 아이템이 삭제된 경우
            deleted = true;
            gameManager.SaveManager.Save.DeleteSlot(scrollIndex);   // 스크롤 아이템 삭제 (save만 수정)
            CloseReinforceUI();     // 강화 모드 종료
        }

        if (deleted)
        {
            // 스크롤 아이템 혹은 강화 아이템이 삭제된 경우
            SaveAndLoad();
            deleted = false;
        }
        else
        {
            // Save 저장, Load 없이 인벤토리와 save 동기화 되어있어야 함
            gameManager.SaveManager.SaveData();
        }
    }

    public void OpenReinforceUI()
    {
        go_reinforce.SetActive(true);
    }
    public void CloseReinforceUI()
    {
        go_reinforce.SetActive(false);
        reinforceMode = false;
        scrollSlotId = -1;
        scrollType = ItemType.None;
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
        // 슬롯 아이디로 슬롯 인덱스 찾기
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
        // itemDetail.Close();     // Close Item detail UI  // !!! UNUSED
        EndDeleteMode();        // 삭제 모드 종료
        CloseReinforceUI();     // Reinforce 모드 종료
    }
}
