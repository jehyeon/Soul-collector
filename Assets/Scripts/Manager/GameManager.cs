using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private ItemManager itemManager;
    private SaveManager saveManager;
    private DropManager dropManager;
    private CraftManager craftManager;
    private ShopManager shopManager;

    private int floor;
    public int Floor { get { return floor; } }

    [SerializeField]
    private Player player;
    [SerializeField]
    private SkillSystem skillSystem;

    [SerializeField]
    private Inventory inventory;
    [SerializeField]
    private Equipment equipment;
    [SerializeField]
    private Craft craft;
    [SerializeField]
    private Shop shop;
    [SerializeField]
    private Reinforce reinforce;
    [SerializeField]
    private Auction auction;
    [SerializeField]
    private Push push;

    [SerializeField]
    private UIController uiController;

    [SerializeField]
    private ApiManager apiManager;

    private DropSystem dropSystem;
    private EnemyHpBarSystem enemyHpBarSystem;

    public ItemManager ItemManager { get { return itemManager; } }
    public SaveManager SaveManager { get { return saveManager; } }
    public DropManager DropManager { get { return dropManager; } }
    public CraftManager CraftManager { get { return craftManager; } }
    public ShopManager ShopManager { get { return shopManager; } }
    public UIController UIController { get { return uiController; } }

    public Inventory Inventory { get { return inventory; } }
    public Reinforce Reinforce { get { return reinforce; } }
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
        // !!! 유저 정보를 서버에서 체크 해야함
        // saveManager.Save.UserId -> DB에 있으면 lastLogin 업데이트
        apiManager.CheckUser(saveManager.Save.UserId);
        // -> DB에 없으면 saveManager 새로 생성

        // Load Inventory, Equip info, gold
        Inventory.StartInventory();
        equipment.InitEquipmentSlots();
        LoadEquipInfo();
        player.Heal(99999);     // 세이브에 현재 체력 정보는 저장하지 않음 -> 최대 체력 스폰

        // Load Skill info
        skillSystem.InitSkillSystem();
        LoadSkillInfo();

        // Load Shop, Craft Info
        craft.InitCraftItemSlots();
        shop.InitShopItemSlots();
    }

    // -------------------------------------------------------------
    // 씬 이동
    // -------------------------------------------------------------
    private void GoViliage()
    {
        SceneManager.LoadScene("Main");
        PlayerReset();

        // 던전 Floor UI
        floor = 0;
        uiController.DeActivateFloorText();
    }

    private void GoDungeon()
    {
        SceneManager.LoadScene("Default Dungeon");
        PlayerReset();

        // 던전 Floor UI
        floor = 1;
        uiController.ActivateFloorText(1);
    }

    private void GoNextFloor()
    {
        // 다음층으로 던전 생성
        SceneManager.LoadScene("Default Dungeon");      // 씬 리로드
        PlayerReset();

        // 던전 Floor UI
        floor += 1;
        uiController.ActivateFloorText(floor);
    }

    private void PlayerReset()
    {
        player.transform.position = Vector3.zero;
    }

    // -------------------------------------------------------------
    // Equipment, Inventory 아이템 선택, 장착 해제
    // 슬롯 선택 공유
    // -------------------------------------------------------------
    public void SelectSlotOnEquipment()
    {
        // 인벤토리 unselect
        // 다중 모드 종료 + ResetSelect() + InventoryActBtn 업데이트 ("해제")
        inventory.MultiSelectModeOff();
        inventory.UpdateInventoryActBtn(true);
    }

    public void SelectSlotOnInventory()
    {
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
    public bool GetItemCheckInventory(int itemId)
    {
        // auto save
        if (inventory.isFullInventory())
        {
            PopupMessage("인벤토리에 남은 공간이 없습니다.");
            return false;
        }

        inventory.AcquireItem(itemManager.Get(itemId));
        saveManager.SaveData();
        return true;
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

    public void Drop(int dropId, Vector3 pos)
    {
        // 골드 업데이트
        inventory.UpdateGold(dropManager.RandomGold(dropId));
        saveManager.SaveData();
        
        int itemId = dropManager.RandomItem(dropId);
        if (itemId == -1)
        {
            // 아이템 드롭 없음
            return;
        }

        if (dropSystem == null)
        {
            dropSystem = GameObject.Find("Drop System").GetComponent<DropSystem>();
        }
        dropSystem.DropItem(itemId, pos);
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
    // 경매장
    // -------------------------------------------------------------
    public void RequestAuctionItemList()
    {
        apiManager.GetAuctionItemList();
    }

    public void ResponseAuctionItemList(AuctionItemForAPI auctionItemList)
    {
        auction.LoadAuctionItemList(auctionItemList);
    }

    // -------------------------------------------------------------
    // 경매장
    // -------------------------------------------------------------
    public void RequestPushList()
    {
        apiManager.GetPushList(saveManager.Save.UserId);
    }

    public void ResponsePushList(PushForAPI pushes)
    {
        push.LoadPushList(pushes);
    }

    // -------------------------------------------------------------
    // 스킬
    // -------------------------------------------------------------
    private void LoadSkillInfo()
    {
        skillSystem.Activate(saveManager.Save.Skill);
    }

    // -------------------------------------------------------------
    // 체력바 System
    // -------------------------------------------------------------
    public EnemyHpBar InitHpBar()
    {
        // In Default Dungeon scene
        if (enemyHpBarSystem == null)
        {
            enemyHpBarSystem = GameObject.Find("Enemy HP Bar System").GetComponent<EnemyHpBarSystem>();
        }

        return enemyHpBarSystem.InitHpBar();
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
            case "Shop":
                shop.Buy();
                return;
            case "Craft":
                craft.CraftItem();
                return;
            case "Reinforce":
                reinforce.ReinforceItems();
                return;
            case "GoDungeon":
                GoDungeon();
                return;
            case "GoViliage":
                GoViliage();
                return;
            case "GoNextFloor":
                GoNextFloor();
                return;
        }
    }
}