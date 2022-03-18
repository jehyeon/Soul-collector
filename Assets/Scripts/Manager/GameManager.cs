using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private ItemManager itemManager;
    private SaveManager saveManager;
    private DropManager dropManager;
    private CraftManager craftManager;
    private ShopManager shopManager;

    [SerializeField]
    private Player player;

    [SerializeField]
    private Inventory inventory;
    [SerializeField]
    private Equipment equipment;
    [SerializeField]
    private Craft craft;
    [SerializeField]
    private Shop shop;

    [SerializeField]
    private UIController uiController;

    public ItemManager ItemManager { get { return itemManager; } }
    public SaveManager SaveManager { get { return saveManager; } }
    public DropManager DropManager { get { return dropManager; } }
    public CraftManager CraftManager { get { return craftManager; } }
    public ShopManager ShopManager { get { return shopManager; } }
    public UIController UIController { get { return uiController; } }

    public Inventory Inventory { get { return inventory; } }
    public Player Player { get { return player; } }

    private void Awake()
    {
        itemManager = new ItemManager();
        saveManager = new SaveManager();
        dropManager = new DropManager();
        craftManager = new CraftManager();
        shopManager = new ShopManager();
    }

    private void Start()
    {
        // Load Inventory, Equip info, gold
        Inventory.StartInventory();
        equipment.InitEquipmentSlots();
        LoadEquipInfo();

        // Load Shop, Craft Info
        craft.InitCraftItemSlots();
        shop.InitShopItemSlots();
    }

    // -------------------------------------------------------------
    // Equipment, Inventory 아이템 선택, 장착 해제
    // 슬롯 선택 공유
    // -------------------------------------------------------------
    public void SelectSlotOnEquipment()
    {
        Debug.Log("장착정보 슬롯 클릭");
        // 인벤토리 unselect
        // 다중 모드 종료 + ResetSelect() + InventoryActBtn 업데이트 ("해제")
        inventory.MultiSelectModeOff();
        inventory.UpdateInventoryActBtn(true);
    }

    public void SelectSlotOnInventory()
    {
        Debug.Log("인벤토리 슬롯 클릭");
        // 장착 슬롯 unselect
        equipment.UnSelectSlot();
    }

    public void CallUnEquipOnInventory()
    {
        // inventory로부터 Unequip 요청
        // equipment slot이 selected인 경우에만 호출이 가능
        equipment.UnEquipItem();
        // 장착 해제 후 InventoryActBtn 업데이트
        inventory.UpdateInventoryActBtn();
    }

    // -------------------------------------------------------------
    // 아이템 획득
    // -------------------------------------------------------------
    public void GetItem(GameObject targetObject)
    {
        if (inventory.isFullInventory())
        {
            PopupMessage("인벤토리에 남은 공간이 없습니다.");
            return;
        }

        inventory.AcquireItem(itemManager.Get(targetObject.GetComponent<DroppedItem>().Id));
    }

    public void GetItem(int itemId)
    {
        if (inventory.isFullInventory())
        {
            PopupMessage("인벤토리에 남은 공간이 없습니다.");
            return;
        }

        inventory.AcquireItem(itemManager.Get(itemId));
    }

    // -------------------------------------------------------------
    // 아이템 장착
    // -------------------------------------------------------------
    public void Equip(Item equipping)
    {
        // view 수정, 이미 장착 아이템이 있는 경우
        // Equipment.cs에서 gameManager.UnEquip(item) 호출
        equipment.EquipItem(equipping); 

        // 장착 정보 수정 (저장 X)
        player.Equip(equipping);    // 스탯 추가
        saveManager.Save.Equipped[equipping.PartNum] = equipping.Id;
        saveManager.SaveData();
    }

    public void UnEquip(Item unEquipping)
    {
        player.UnEquip(unEquipping);    // 스탯 제거
        saveManager.Save.Equipped[unEquipping.PartNum] = -1;  // 장착 정보 수정 (저장 X)
        inventory.AcquireItem(unEquipping);     // 인벤토리 아이템 추가 (저장 X)
        saveManager.SaveData();
    }

    private void LoadEquipInfo()
    {
        // 게임 시작시 장착 정보 로드
        foreach (int itemId in saveManager.Save.Equipped)
        {
            if (itemId != -1)
            {
                // Equip(itemManager.Get(itemId));
                Item item = itemManager.Get(itemId);
                equipment.EquipItem(item); // view 수정
                player.Equip(item);   // 스탯 추가
            }
        }
    }

    // -------------------------------------------------------------
    // 팝업 메시지
    // -------------------------------------------------------------
    public void PopupMessage(string message, float time = 1f)
    {
        uiController.PopupMessage(message, time);
    }

    public void PopupAsk(string type, string ask, string leftText, string rightText)
    {
        uiController.PopupAsk(type, ask, leftText, rightText);
    }

    public void AnswerAsk(string type)
    {
        switch (type)
        {
            case "Delete":
                inventory.Delete();
                return;
        }
    }
}