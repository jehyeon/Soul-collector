using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;
// using LitJson;

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

    // 아이템 툴팁
    [SerializeField]
    private ItemDetail itemDetail;

    // 아이템 다중 선택 모드
    [SerializeField]
    private GameObject goMultiSelectModeOn;
    [SerializeField]
    private GameObject goMultiSelectModeOff;

    // 삭제 버튼
    [SerializeField]
    private Button btnDelete;

    // slots
    private InventorySlot[] slots;

    private bool multiSelectMode;               // 다중 선택 모드

    // 선택된 slot index
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
        deleteItemMode = false;
        reinforceMode = false;
    }

    private void Start()
    {
        MultiSelectModeOff();
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
                    gameManager.SaveManager.Save.Slots[i].Count,
                    gameManager.SaveManager.Save.Slots[i].Level
                );
            }
        }

        UpdateItemSlotCountUI();    // 인벤토리 로드 시, 슬롯 수량 UI 업데이트
    }

    private void SaveAndLoad()
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
        gameManager.Player.Heal(99999); // 세이브에 현재 체력 정보는 저장하지 않음 -> 최대 체력 스폰
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
    }

    // -------------------------------------------------------------
    // 인벤토리 슬롯 선택 #Select #선택
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
    private void UpdateInventoryActBtn()
    {
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
        itemDetail.Open(item, pos);
    }

    public void CloseItemDetail()
    {
        itemDetail.Close();
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
        // go_reinforce.SetActive(true);
    }
    public void CloseReinforceUI()
    {
        // go_reinforce.SetActive(false);
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

    public int GetItemAmount(int itemId)
    {
        if (itemId == 1627)
        {
            // gold
            return gameManager.SaveManager.Save.Gold;
        }
        
        foreach (InventorySlot item in slots)
        {
            if (itemId == item.Id)
            {
                return item.Count;
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
    public void Open()
    {
        this.gameObject.SetActive(true);
    }

    public void Close()
    {
        this.gameObject.SetActive(false);
        CloseReinforceUI();     // Reinforce 모드 종료
        MultiSelectModeOff();   // 다중 선택 모드 off
    }
}
