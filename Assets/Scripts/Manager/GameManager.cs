using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameManager : MonoBehaviour
{
    private ItemManager itemManager;
    private SaveManager saveManager;
    private DropManager dropManager;
    private CraftManager craftManager;
    private ShopManager shopManager;

    private EnemySystem enemySystem;

    private int floor;
    public int Floor { get { return floor; } }

    [SerializeField]
    private Player player;
    [SerializeField]
    private SkillSystem skillSystem;
    [SerializeField]
    private QuickSlotSystem quickSlotSystem;

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
    public QuickSlotSystem QuickSlotSystem { get { return quickSlotSystem; } }
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

        // Load QuickSlot info
        quickSlotSystem.LoadQuickSlotSystem();

        // Load Shop, Craft Info
        craft.InitCraftItemSlots();
        shop.InitShopItemSlots();
    }

    // -------------------------------------------------------------
    // 씬 이동
    // -------------------------------------------------------------
    public void GoViliage()
    {
        LoadingSceneManager.LoadScene("Main");
        
        // 던전 Floor UI
        floor = 0;
        uiController.DeActivateFloorText();
    }

    private void GoDungeon()
    {
        LoadingSceneManager.LoadScene("Default Dungeon");
        // 던전 Floor UI
        floor = 1;
        uiController.ActivateFloorText(1);
    }

    private void GoNextFloor()
    {
        // 다음층으로 던전 생성
        LoadingSceneManager.LoadScene("Default Dungeon");   // 씬 리로드

        // 던전 Floor UI
        floor += 1;
        uiController.ActivateFloorText(floor);
    }

    // -------------------------------------------------------------
    // Dungeon
    // -------------------------------------------------------------
    public GameObject[] GetEnemyObjects()
    {
        if (enemySystem == null)
        {
            enemySystem = GameObject.Find("EnemySystem").GetComponent<EnemySystem>();
        }

        return enemySystem.GetEnemyObjects(floor);
    }

    // public void DeleteRandomEquippedItem()
    // {
    //     var equipped = from equipItemId in saveManager.Save.Equipped
    //                    where equipItemId != -1
    //                    select equipItemId;
    //     int[] equippedItems = equipped.ToArray();
    //     Debug.Log(equippedItems);
    //     if (equippedItems.Length == 0)
    //     {
    //         return;
    //     }
    //     int removeItemId = equippedItems[Random.Range(0, equippedItems.Length)];

    //     var deleted = from equipItemId in saveManager.Save.Equipped
    //                    where equipItemId != removeItemId
    //                    select equipItemId;
    //     Debug.Log(saveManager.Save.Equipped);
    //     saveManager.Save.Equipped = deleted.ToList();
    //     Debug.Log(saveManager.Save.Equipped);
    //     saveManager.SaveData();
    // }

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

    public void SelectSlotOnInventory(ItemType itemType)
    {
        // 장착 슬롯 unselect
        equipment.UnSelectSlot();

        if (itemType == ItemType.Use)
        {
            // 선택한 아이템이 사용 아이템인 경우
            equipment.ShowQuickSlotBtn();
        }
        else
        {
            equipment.CloseQuickSlotBtn();
        }
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
        dropSystem.DropItem(itemManager.Get(itemId), pos);
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

    private void BuyAuction()
    {
        // 골드 감소
        inventory.UpdateGold(-1 * auction.SelectedAuctionItem.price);
        saveManager.SaveData();

        // auction api에서 삭제
        apiManager.DeleteAuctionItem(auction.SelectedAuctionItem.userId, auction.SelectedAuctionItem.id);

        // push api에 추가
        // 구매자에게 아이템 전송
        apiManager.AddPush(saveManager.Save.UserId, auction.SelectedAuctionItem.itemId, "경매장에서 구입한 아이템입니다.");
        // 판매자에게 gold 전송
        apiManager.AddPush
        (
            auction.SelectedAuctionItem.userId, 
            0, 
            string.Format("아이템 판매 대금입니다.\n판매 금액: {0}", Mathf.FloorToInt((float)(auction.SelectedAuctionItem.price * 0.9))), 
            Mathf.FloorToInt((float)(auction.SelectedAuctionItem.price * 0.9))
        );
    }

    public void AddItemToAuction(int _price)
    {
        // auction reload
        AuctionItem auctionItem = new AuctionItem
        {
            userId = saveManager.Save.UserId,
            itemId = inventory.Slots[inventory.SelectedSlotIndex].Item.Id,
            rank = "common",
            type = "weapon",
            price = _price
        };
        auction.AddItemToAuction(auctionItem);

        // 경매장에 아이템 추가
        apiManager.AddItemToAuction(auctionItem.userId, auctionItem.itemId, auctionItem.price);
        
        // 인벤토리 아이템 삭제
        inventory.Delete();  // 선택한 아이템 삭제 및 save and load
        
        // !!! 임시로 버그 방지를 위해 아이템 등록 시 UI가 닫힘
        UIController.CloseUI();
    }

    public void CancelToSell()
    {
        apiManager.DeleteAuctionItem(saveManager.Save.UserId, auction.SelectedAuctionItem.id);
        apiManager.AddPush(saveManager.Save.UserId, auction.SelectedAuctionItem.itemId, "경매장 등록을 취소한 아이템입니다.");
    }

    // -------------------------------------------------------------
    // 푸시
    // -------------------------------------------------------------
    public void RequestPushList()
    {
        apiManager.GetPushList(saveManager.Save.UserId);
    }

    public void ResponsePushList(PushForAPI pushes)
    {
        push.LoadPushList(pushes);
    }

    public void ReceivePush()
    {
        apiManager.DeletePush(saveManager.Save.UserId, push.SelectedPushId);
    }

    // // -------------------------------------------------------------
    // // 스킬
    // // -------------------------------------------------------------
    // public List<int> LoadQuickSlotInfo()
    // {
    //     return saveManager.Save.QuickSlot;
    // }

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
    public EnemyHpBar InitHpBar(Vector3 initPos)
    {
        // In Default Dungeon scene
        if (enemyHpBarSystem == null)
        {
            enemyHpBarSystem = GameObject.Find("Enemy HP Bar System").GetComponent<EnemyHpBarSystem>();
        }

        return enemyHpBarSystem.InitHpBar(initPos);
    }

    // -------------------------------------------------------------
    // 팝업 메시지
    // -------------------------------------------------------------
    public void PopupMessage(string message, float time = 1f)
    {
        uiController.PopupMessage(message, time);
    }
    public void PopupMessageClose()
    {
        uiController.PopupMessageClose();
    }

    public void PopupAsk(string type, string ask, string leftText, string rightText)
    {
        uiController.PopupAsk(type, ask, leftText, rightText);
    }

    public void PopupSetCount(string type, string message, string leftText, string rightText, int maxCount, int defaultCount = 1)
    {
        uiController.PopupSetCount(type, message, leftText, rightText, maxCount, defaultCount);
    }

    public void AnswerAsk(string type)
    {
        switch (type)
        {
            case "Delete":
                inventory.Delete();
                return;
            case "Sell":
                inventory.Sell();
                return;
            // case "Craft":
            //     craft.CraftItem();
            //     return;
            case "Reinforce":
                reinforce.ReinforceItems();
                return;
            case "BuyAuction":
                BuyAuction();
                auction.BuyDone();
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

    public void AnswerCount(string type, int count)
    {
        switch (type)
        {
            case "Auction":
                AddItemToAuction(count);
                return;
            case "Shop":
                shop.Buy(count);
                return;
            case "Craft":
                craft.CraftItem(count);
                return;
            case "Use":
                inventory.Use(count);
                return;
        }
    }
}