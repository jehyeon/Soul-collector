using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;
// using LitJson;

public enum InventoryMode
{
    Shop,
    Reinforce,
    Auction,
    NotWork,
    Default    
}

public class Inventory : MonoBehaviour
{
    // Manager
    [SerializeField]
    private GameManager gameManager;
    private UseItemSystem useItemSystem;

    // Gold
    [SerializeField]
    private TextMeshProUGUI textGold;

    // 인벤토리 슬롯
    [SerializeField]
    private GameObject goInventorySlotParent;
    [SerializeField]
    private GameObject goInventorySlotPref;

    private Inventory myInventory;      // InventorySlot에게 자기 자신 인스턴스를 넘기기 위함

    // 인벤토리 동작 버튼 (장착, 장착해제, 사용)
    // !!! 퀵슬롯 등록 추가하기
    [SerializeField]
    private GameObject goInventoryActBtn;
    [SerializeField]
    private TextMeshProUGUI textInventoryActBtn;

    // 인벤토리 Slot Count
    [SerializeField]
    private TextMeshProUGUI textRemainInventorySlotCount;

    // 아이템 다중 선택 모드
    [SerializeField]
    private GameObject goMultiSelectModeUI;
    [SerializeField]
    private GameObject goMultiSelectModeOn;
    [SerializeField]
    private GameObject goMultiSelectModeOff;

    // 삭제 버튼
    [SerializeField]
    private Button btnDelete;

    // slots
    private InventorySlot[] slots;
    public InventorySlot[] Slots { get { return slots; } }

    public bool multiSelectMode;               // 다중 선택 모드 public for test

    // 선택된 slot index
    private int selectedSlotIndex = -1;              // 선택된 index (multiSelectMode == false)
    private List<int> selectedSlotIndexList;    // 선택된 index list (multiSelectMode == true)
    public int SelectedSlotIndex { get { return selectedSlotIndex; } }

    public InventoryMode mode;  // public for test 
    public InventoryMode Mode { get { return mode; } }

    // -------------------------------------------------------------
    // Init, Update
    // -------------------------------------------------------------
    void Awake()
    {
        // save로부터 초기화가 안되는 field 초기화
        selectedSlotIndex = -1;
        selectedSlotIndexList = new List<int>();
    }

    private void Start()
    {
        if (mode == InventoryMode.Reinforce)
        {
            MultiSelectModeOn();
        }
        else
        {
            MultiSelectModeOff();
        }
        UnEnableDeleteButton();
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
            GameObject inventorySlot = Instantiate(goInventorySlotPref);
            inventorySlot.transform.SetParent(goInventorySlotParent.transform);
            inventorySlot.GetComponent<InventorySlot>().Init(i, myInventory);
        }

        // 생성된 슬롯을 slots에 저장
        slots = goInventorySlotParent.GetComponentsInChildren<InventorySlot>();
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
                    gameManager.SaveManager.Save.Slots[i].Count
                );
            }
        }

        UpdateItemSlotCountUI();    // 인벤토리 로드 시, 슬롯 수량 UI 업데이트
    }

    public void SaveAndLoad()
    {
        // 현재 SaveManager.Save를 저장하고 Inventory 리로드
        gameManager.SaveManager.SaveData();
        LoadInventory();
    }

    public void StartInventory()
    {
        // GameManager Start()에서 실행
        UpdateGold(0);                  // Save 기준으로 Gold Text 업데이트
        InitInventorySlots();           // 인벤토리 크기에 맞게 Slot 생성
        LoadInventory();                // Save 기준으로 인벤토리 정보 로드
        useItemSystem = new UseItemSystem(gameManager); // 아이템 사용 기능 호출
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
        UpdateItemSlotCountUI();    // 아이템 슬롯에 새로 추가된 경우 slot count ui 업데이트
    }

    // -------------------------------------------------------------
    // 인벤토리 슬롯 선택 #Select #선택
    // -------------------------------------------------------------
    public void SelectSlot(int slotIndex)
    {
        gameManager.SelectSlotOnInventory();

        if (multiSelectMode)
        {
            selectedSlotIndexList.Add(slotIndex);
            if (mode == InventoryMode.Shop)
            {
                UpdateInventoryActBtn();
                return;
            }
        }
        else
        {
            if (selectedSlotIndex != -1)
            {
                slots[selectedSlotIndex].UnSelect();    // 기존에 선택된 슬롯 선택 해제
            }
            selectedSlotIndex = slotIndex;

            UpdateInventoryActBtn();
        }

        EnableDeleteButton();   // 삭제 버튼 활성화
    }

    public void UnSelectSlot(int slotIndex)
    {
        if (multiSelectMode)
        {
            selectedSlotIndexList.Remove(slotIndex);

            if (selectedSlotIndexList.Count <= 0)
            {
                // 선택된 슬롯이 없는 경우
                UnEnableDeleteButton();     // 삭제 버튼 비활성화
            }
        }
        else
        {
            selectedSlotIndex = -1;

            UpdateInventoryActBtn();
            UnEnableDeleteButton();
        }
    }

    public void UnSelectUsingSlotId(int slotId)
    {
        int slotIndex = FindItemUsingSlotId(slotId);
        if (slotIndex == -1)
        {
            // 해당 아이템이 없는 경우 error
            return;
        }

        // 해당 슬롯 unselect
        slots[slotIndex].UnSelect();
        UnSelectSlot(slotIndex);
    }

    private void ResetSelectSlot()
    {
        if (selectedSlotIndex != -1)
        {
            slots[selectedSlotIndex].UnSelect();
            UnSelectSlot(selectedSlotIndex);
            return;
        }
        
        if (selectedSlotIndexList == null)
        {
            selectedSlotIndexList = new List<int>();
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

    // 다중선택 모드
    public void MultiSelectModeOn()
    {
        multiSelectMode = true;
        goMultiSelectModeOn.SetActive(true);
        goMultiSelectModeOff.SetActive(false);

        if (selectedSlotIndex != -1)
        {
            // 이미 선택된 아이템이 있는 경우
            selectedSlotIndexList.Add(selectedSlotIndex);
            selectedSlotIndex = -1;
        }

        UpdateInventoryActBtn();
    }

    public void MultiSelectModeOff()
    {
        multiSelectMode = false;
        goMultiSelectModeOn.SetActive(false);
        goMultiSelectModeOff.SetActive(true);

        ResetSelectSlot();
    }

    // -------------------------------------------------------------
    // 인벤토리 UI action (Slot, Button)
    // -------------------------------------------------------------
    public void UpdateInventoryActBtn(bool selectingEquipmentSlot = false)
    {
        // checkingEquipmentSlot: 장착 정보 슬롯에서 check가 되어 있는 경우 (from gameManager)
        if (selectingEquipmentSlot)
        {
            goInventoryActBtn.SetActive(true);
            textInventoryActBtn.text = "해제";
            return;
        }

        if (mode == InventoryMode.NotWork || mode == InventoryMode.Reinforce)
        {
            // Inventory 버튼 text 갱신 및 삭제, 다중 선택 버튼 deactivate
            btnDelete.gameObject.SetActive(false);
            goMultiSelectModeUI.SetActive(false);
            return;
        }

        if (mode == InventoryMode.Auction)
        {
            // Inventory 버튼 text 갱신 및 삭제, 다중 선택 버튼 deactivate
            btnDelete.gameObject.SetActive(false);
            goMultiSelectModeUI.SetActive(false);
            if (selectedSlotIndex != -1)
            {
                Debug.Log(slots[selectedSlotIndex].Count);
                if (slots[selectedSlotIndex].Count > 1)
                {
                    // !!! 아이템 여러개 판매는 일단 미지원
                    goInventoryActBtn.SetActive(false);
                    textInventoryActBtn.text = "";
                    return;
                }
                // 선택된 아이템이 있는 경우
                goInventoryActBtn.SetActive(true);
                textInventoryActBtn.text = "판매";
            }
            else
            {
                goInventoryActBtn.SetActive(false);
                textInventoryActBtn.text = "";
            }
            return;
        }

        if (mode == InventoryMode.Shop)
        {   
            // 상점에서는 아이템 삭제 버튼 비활성화
            btnDelete.gameObject.SetActive(false);
            if (selectedSlotIndex == -1 && selectedSlotIndexList.Count == 0)
            {
                // 선택된 슬롯이 없는 경우
                goInventoryActBtn.SetActive(false);
                textInventoryActBtn.text = "";
                return;
            }

            goInventoryActBtn.SetActive(true);
            textInventoryActBtn.text = "판매";
            return;
        }

        btnDelete.gameObject.SetActive(true);
        goMultiSelectModeUI.SetActive(true);

        // 인벤토리 버튼 활성화 및 text 수정
        // 현재 인벤토리 모드 및 아이템 type에 따라 다름
        if (selectedSlotIndex == -1 || multiSelectMode)
        {
            // 선택된 slot이 없는 경우 or 아이템 다중 선택 모드인 경우
            goInventoryActBtn.SetActive(false);
            textInventoryActBtn.text = "";
            return;
        }

        if (slots[selectedSlotIndex].Item.ItemType == ItemType.Use)
        {
            // 사용 아이템인 경우
            goInventoryActBtn.SetActive(true);
            textInventoryActBtn.text = "사용";
            return;
        }

        if (slots[selectedSlotIndex].Item.ItemType == ItemType.Weapon 
        || slots[selectedSlotIndex].Item.ItemType == ItemType.Armor)
        {
            goInventoryActBtn.SetActive(true);
            textInventoryActBtn.text = "장착";
            return;
        }
    }

    public void ClickInventoryActBtn()
    {
        if (selectedSlotIndex == -1)
        {
            // 선택된 아이템이 없는 경우 버튼 활성화가 되지 않음
            // Only Equipment slot selected
            gameManager.CallUnEquipOnInventory();
            return;
        }

        if (mode == InventoryMode.Auction)
        {
            // 경매장에서 판매 버튼 클릭 시
            gameManager.PopupSetCount("Auction", "판매 가격을 설정해주세요.", "취소", "확인", 9999999);
            return;
        }

        if (mode == InventoryMode.Shop)
        {   
            gameManager.PopupAsk("Sell", "아이템을 판매하시겠습니까?", "아니요", "네");
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

            gameManager.UIController.PlayEquipSound(slots[selectedSlotIndex].Item.ItemType);

            ResetSelectSlot();
            LoadInventory();
        }

        UpdateInventoryActBtn();
    }

    // 마우스 우클릭 InventorySlot call
    public void RightClick(int slotIndex)
    {
        ResetSelectSlot();              // Select 초기화
        // 우클릭 슬롯 index로 ClickInventoryActBtn()
        selectedSlotIndex = slotIndex;
        ClickInventoryActBtn();
    }

    // Item detail tooltip
    public void ShowItemDetail(Item item, Vector3 pos)
    {
        gameManager.UIController.OpenItemDetail(item, pos);
    }

    public void CloseItemDetail()
    {
        gameManager.UIController.CloseItemDetail();
    }

    // -------------------------------------------------------------
    // Delete Item #Delete
    // -------------------------------------------------------------
    private void EnableDeleteButton()
    {
        btnDelete.interactable = true;
    }

    private void UnEnableDeleteButton()
    {
        btnDelete.interactable = false;
    }

    public void ClickDeleteBtn()
    {
        // 인벤토리 우측 하단 삭제 버튼 클릭 시
        if (selectedSlotIndex == -1 && selectedSlotIndexList.Count == 0)
        {
            // 선택된 슬롯이 없는 경우
            // gameManager.PopupMessage("삭제할 아이템을 선택하세요");
            return;

            // select이 없을 경우 버튼 비활성화 처리함
        }

        gameManager.PopupAsk("Delete", "아이템을 삭제하시겠습니까?", "아니요", "네");
    }
    public void Delete()
    {
        if (multiSelectMode)
        {
            Delete(selectedSlotIndexList);
        }
        else
        {
            Delete(selectedSlotIndex);
        }
    }

    public void Delete(int wantToDeleteItemIndex, bool save = true)
    {
        ResetSelectSlot();

        gameManager.SaveManager.Save.DeleteSlot(wantToDeleteItemIndex);
        if (save)
        {
            SaveAndLoad();
        }
    }

    private void Delete(List<int> wantToDeleteItemIndeices, bool save = true)
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
        if (save)
        {
            SaveAndLoad();
        }
    }
    // -------------------------------------------------------------
    // Sell
    // -------------------------------------------------------------
    public void Sell()
    {
        Debug.Log(slots[selectedSlotIndex].Item.Rank);
        int price = 100 * (int)Mathf.Pow(10, slots[selectedSlotIndex].Item.Rank);
        Debug.Log(price * slots[selectedSlotIndex].Count);
        gameManager.SaveManager.Save.DeleteSlot(selectedSlotIndex);
        UpdateGold(slots[selectedSlotIndex].Count * price);
        ResetSelectSlot();

        SaveAndLoad();
    }

    // -------------------------------------------------------------
    // Update Count (ex. Use, Craft) #Count
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

    public void UpdateItemCountUsingItemId(int itemId, int count, bool save = true)
    {
        foreach(InventorySlot slot in slots)
        {
            if (slot.Item == null)
            {
                return;
            }

            if (slot.Item.Id == itemId)
            {
                // 수량 확인
                if (slot.Count + count <= 0)
                {
                    if (slot.Count + count < 0)
                    {
                        Debug.Log("Error");
                    }

                    // 수량 업데이트 이후 count가 0 이하가 되는 경우
                    Delete(slot.Index, false);
                }
                else
                {
                    SetSlotCount(slot.Index, count);    // save 및 view 모두 수정
                }
            }
        }

        if (save)
        {
            SaveAndLoad();
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

    public void SetSlotCount(int slotIndex, int count)
    {
        // slotIndex의 아이템 개수 수정 (+count)
        gameManager.SaveManager.Save.Slots[slotIndex].UpdateCount(count);   // save 데이터 수정 (별도로 save.SaveData()해야 저장이 됨)
        slots[slotIndex].SetSlotCount(count);   // 슬롯 view 수정
    }

    private void UpdateItemSlotCountUI()
    {
        textRemainInventorySlotCount.text = string.Format("{0} / {1}",
            gameManager.SaveManager.Save.LastSlotIndex,
            gameManager.SaveManager.Save.InventorySize
        );
    }

    // -------------------------------------------------------------
    // 아이템 구매 (Decrease gold + Add item)
    // -------------------------------------------------------------
    public void Buy(Item item, int price, int count = 1)
    {
        for (int i = 0; i < count; i++)
        {
            // 아이템 추가, 골드 감소
            AcquireItem(item);
            UpdateGold(-1 * price);
        }
        // Save
        gameManager.SaveManager.SaveData();
    }

    // -------------------------------------------------------------
    // 아이템 강화
    // -------------------------------------------------------------
    public bool AddItemToReinforceSlot(Item wantToReinforceItem, int slotId, int count = 1)
    {
        // 강화 슬롯 아이템 추가
        return gameManager.Reinforce.Add(wantToReinforceItem, slotId, count);   
    }

    public void RemoveItemToReinforceSlot(int slotId, int itemId)
    {
        //  강화 슬롯 아이템 삭제
        gameManager.Reinforce.RemoveFromInventory(slotId, itemId);
    }

    public bool CheckItemForReinforce(Item item)
    {
        if (item.Level == 9)
        {
            // 강화 최대 등급인 경우 강화 불가능
            return false;
        }
        
        // 해당 아이템을 강화할 수 있는 지 확인
        if (gameManager.Reinforce.ScrollType == ScrollType.None)
        {
            // 기존에 설정된 ScrollType이 없는 경우
            if (item.ItemType == ItemType.Weapon)
            {
                // 무기 아이템인 경우
                gameManager.Reinforce.SetScrollType(ScrollType.Weapon);
                return true;
            }
            else if (item.ItemType == ItemType.Armor)
            {
                // 방어구 아이템인 경우
                gameManager.Reinforce.SetScrollType(ScrollType.Armor);
                return true;
            }
            else if(item.ItemType == ItemType.Use)
            {
                // 사용 아이템인 경우
                if (item.Id == 13)
                {
                    // 무기 강화 주문서
                    gameManager.Reinforce.SetScrollType(ScrollType.Weapon);
                    return true;
                }
                else if (item.Id == 14)
                {
                    // 방어구 강화 주문서
                    gameManager.Reinforce.SetScrollType(ScrollType.Armor);
                    return true;
                }
                // !!! 소울 스톤 강화는 나중에 추가하기
            }
            else
            {
                // 그 외 아이템은 강화 불가능
                return false;
            }
        }
        else if (gameManager.Reinforce.ScrollType == ScrollType.Weapon)
        {
            // 무기 강화 중인 경우
            if (item.ItemType == ItemType.Weapon || item.Id == 13)
            {
                // 무기만 선택 가능
                return true;
            }

            return false;
        }
        else if (gameManager.Reinforce.ScrollType == ScrollType.Armor)
        {
            // 방어구 강화 중인 경우
            if (item.ItemType == ItemType.Armor || item.Id == 14)
            {
                // 방어구만 선택 가능
                return true;
            }
            return false;
        }
        // !!! 소울 스톤 강화는 나중에 추가하기
        
        return false;
    }

    public void SuccessReinforce(int slotId, Item newItem)
    {
        // 강화 성공
        // +1 아이템으로 교체
        // Debug.Log(slotId);
        int slotIndex = FindItemUsingSlotId(slotId);
        // Debug.Log(slotIndex);
        // slots[slotIndex].Upgrade(newItem);
        gameManager.SaveManager.Save.Slots[slotIndex].SetItemId(newItem.Id);
    }

    public void FailReinforce(int slotId)
    {
        // 강화 실패
        int slotIndex = FindItemUsingSlotId(slotId);
        gameManager.SaveManager.Save.DeleteSlot(slotIndex);
    }

    public void UpdateScrollItem(int scrollSlotId, int useCount)
    {
        // 강화에 사용되는 scroll 아이템 수량 조절 및 삭제
        int slotIndex = FindItemUsingSlotId(scrollSlotId);
        if (slots[slotIndex].Count > useCount)
        {
            // 사용 하는 양보다 스크롤 양이 더 많은 경우
            SetSlotCount(slotIndex, -1 * useCount);
        }
        else
        {
            gameManager.SaveManager.Save.DeleteSlot(slotIndex);
        }
    }

    public void DoneReinforce(List<int> remainItemIds)
    {
        // 강화가 모두 끝난 경우
        SaveAndLoad();      // SaveManager.Save 기준으로 인벤토리 리로드

        // 선택한 슬롯 초기화 후 다시 선택
        ResetSelectSlot();

        foreach (int id in remainItemIds)
        {
            // !!! 인벤토리 용량이 커지면 레이턴시가 생길듯
            int slotIndex = FindItemUsingSlotId(id);
            slots[slotIndex].Select();     // view
            selectedSlotIndexList.Add(slotIndex);
        }
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

    public int GetRemainInventory()
    {
        return gameManager.SaveManager.Save.InventorySize - gameManager.SaveManager.Save.LastSlotIndex;
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

    public int GetItemAmount(int itemId)
    {
        if (itemId == 0)
        {
            // gold
            return gameManager.SaveManager.Save.Gold;
        }

        foreach (InventorySlot slot in slots)
        {
            if (slot.Item == null)
            {
                // 인벤토리 끝까지 아이템이 없는 경우
                return 0;
            }

            if (itemId == slot.Item.Id)
            {
                return slot.Count;
            }
        }

        return 0;
    }

    // -------------------------------------------------------------
    // Gold
    // -------------------------------------------------------------
    public void UpdateGold(int amount)
    {
        gameManager.SaveManager.Save.Gold += amount;
        textGold.text = string.Format("{0:#,0}", gameManager.SaveManager.Save.Gold).ToString();
    }

    // -------------------------------------------------------------
    // UI gameObject (use on UIController.cs)
    // -------------------------------------------------------------
    public void Open(string type = "default")
    {
        // 인벤토리가 호출된 경로에 따라 mode 설정
        switch (type)
        {
            case "Shop":
                // !!! 아이템 선택 시 판매 버튼 추가
                mode = InventoryMode.Shop;
                break;
            case "Craft":
                mode = InventoryMode.NotWork;
                break;
            case "Reinforce":
                mode = InventoryMode.Reinforce;
                MultiSelectModeOn();
                break;
            case "Auction":
                mode = InventoryMode.Auction;
                break;
        }

        this.gameObject.SetActive(true);
        UpdateInventoryActBtn();
    }

    public void Close()
    {
        this.gameObject.SetActive(false);
        MultiSelectModeOff();   // 다중 선택 모드 off

        // 모드 
        mode = InventoryMode.Default;
    }
}
