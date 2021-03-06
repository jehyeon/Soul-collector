using UnityEngine;
using UnityEngine.UI;

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
    private Collect collect;
    [SerializeField]
    private BuffSystem buffSystem;
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
    private QuestSystem questSystem;

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
        // !!! ?????? ????????? ???????????? ?????? ?????????
        // saveManager.Save.UserId -> DB??? ????????? lastLogin ????????????
        apiManager.CheckUser(saveManager.Save.UserId);
        // -> DB??? ????????? saveManager ?????? ??????

        // Load Inventory, Equip info, gold
        Inventory.StartInventory();
        equipment.InitEquipmentSlots();
        LoadEquipInfo();

        // Load Collection info
        collect.InitCollection(this);

        // Load Skill info
        skillSystem.InitSkillSystem();
        LoadSkillInfo();

        // Load QuickSlot info
        quickSlotSystem.LoadQuickSlotSystem();

        // Load Shop, Craft Info
        craft.InitCraftItemSlots();
        shop.InitShopItemSlots();
        
        player.Heal(99999);     // ???????????? ?????? ?????? ????????? ???????????? ?????? -> ?????? ?????? ??????
    }

    // -------------------------------------------------------------
    // ??? ??????
    // -------------------------------------------------------------
    public void GoViliage()
    {
        LoadingSceneManager.LoadScene("Main");
        
        // ?????? Floor UI
        floor = 0;
    }

    private void GoDungeon()
    {
        LoadingSceneManager.LoadScene("Default Dungeon");
        // ?????? Floor UI
        floor = 1;
    }

    public void GoDungeon(int _floor)
    {
        LoadingSceneManager.LoadScene("Default Dungeon");
        // ?????? Floor UI
        floor = _floor;
    }

    private void GoNextFloor()
    {
        // ??????????????? ?????? ??????
        LoadingSceneManager.LoadScene("Default Dungeon");   // ??? ?????????

        // ?????? Floor UI
        floor += 1;
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
    // Equipment, Inventory ????????? ??????, ?????? ??????
    // ?????? ?????? ??????
    // -------------------------------------------------------------
    public void SelectSlotOnEquipment()
    {
        // ???????????? unselect
        // ?????? ?????? ?????? + ResetSelect() + InventoryActBtn ???????????? ("??????")
        inventory.MultiSelectModeOff();
        inventory.UpdateInventoryActBtn(true);
    }

    public void SelectSlotOnInventory(ItemType itemType)
    {
        // ?????? ?????? unselect
        equipment.UnSelectSlot();

        if (itemType == ItemType.Use)
        {
            // ????????? ???????????? ?????? ???????????? ??????
            equipment.ShowQuickSlotBtn();
        }
        else
        {
            equipment.CloseQuickSlotBtn();
        }
    }

    public void CallUnEquipOnInventory()
    {
        // inventory????????? Unequip ??????
        // equipment slot??? selected??? ???????????? ????????? ??????
        equipment.UnEquipItem();
        // ?????? ?????? ??? InventoryActBtn ????????????
        inventory.UpdateInventoryActBtn();
    }

    // -------------------------------------------------------------
    // ????????? ??????
    // -------------------------------------------------------------
    public bool GetItemCheckInventory(int itemId)
    {
        // auto save
        if (inventory.isFullInventory())
        {
            PopupMessage("??????????????? ?????? ????????? ????????????.");
            return false;
        }

        inventory.AcquireItem(itemManager.Get(itemId));
        saveManager.SaveData();
        return true;
    }

    public void GetItem(int itemId, int count)
    {
        if (itemId == 0)
        {
            inventory.UpdateGold(count);
        }
        else
        {
            inventory.AcquireItem(itemManager.Get(itemId), count);
        }
        saveManager.SaveData();
    }

    public void Drop(int dropId, Vector3 pos)
    {
        // ?????? ????????????
        inventory.UpdateGold(dropManager.RandomGold(dropId));
        saveManager.SaveData();
        
        int itemId = dropManager.RandomItem(dropId);
        if (itemId == -1)
        {
            // ????????? ?????? ??????
            return;
        }

        if (dropSystem == null)
        {
            dropSystem = GameObject.Find("Drop System").GetComponent<DropSystem>();
        }
        dropSystem.DropItem(itemManager.Get(itemId), pos);
    }

    // -------------------------------------------------------------
    // ????????? ??????
    // -------------------------------------------------------------
    public void Equip(Item equipping)
    {
        // view ??????, ?????? ?????? ???????????? ?????? ??????
        // Equipment.cs?????? gameManager.UnEquip(item) ??????
        equipment.EquipItem(equipping); 

        // ?????? ?????? ?????? (?????? X)
        player.Equip(equipping);    // ?????? ??????
        saveManager.Save.Equipped[equipping.PartNum] = equipping.Id;
        saveManager.SaveData();
    }

    public void UnEquip(Item unEquipping)
    {
        player.UnEquip(unEquipping);    // ?????? ??????
        saveManager.Save.Equipped[unEquipping.PartNum] = -1;  // ?????? ?????? ?????? (?????? X)
        inventory.AcquireItem(unEquipping);     // ???????????? ????????? ?????? (?????? X)
        saveManager.SaveData();
    }

    private void LoadEquipInfo()
    {
        // ?????? ????????? ?????? ?????? ??????
        foreach (int itemId in saveManager.Save.Equipped)
        {
            if (itemId != -1)
            {
                // Equip(itemManager.Get(itemId));
                Item item = itemManager.Get(itemId);
                equipment.EquipItem(item); // view ??????
                player.Equip(item);   // ?????? ??????
            }
        }
    }

    // -------------------------------------------------------------
    // ?????????
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
        // ?????? ??????
        inventory.UpdateGold(-1 * auction.SelectedAuctionItem.price);
        saveManager.SaveData();

        // auction api?????? ??????
        apiManager.DeleteAuctionItem(auction.SelectedAuctionItem.userId, auction.SelectedAuctionItem.id);

        // push api??? ??????
        // ??????????????? ????????? ??????
        apiManager.AddPush(saveManager.Save.UserId, auction.SelectedAuctionItem.itemId, "??????????????? ????????? ??????????????????.");
        // ??????????????? gold ??????
        apiManager.AddPush
        (
            auction.SelectedAuctionItem.userId, 
            0, 
            string.Format("????????? ?????? ???????????????.\n?????? ??????: {0}", Mathf.FloorToInt((float)(auction.SelectedAuctionItem.price * 0.9))), 
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

        // ???????????? ????????? ??????
        apiManager.AddItemToAuction(auctionItem.userId, auctionItem.itemId, auctionItem.price);
        
        // ???????????? ????????? ??????
        inventory.Delete();  // ????????? ????????? ?????? ??? save and load
        
        // !!! ????????? ?????? ????????? ?????? ????????? ?????? ??? UI??? ??????
        UIController.CloseUI();
    }

    public void CancelToSell()
    {
        apiManager.DeleteAuctionItem(saveManager.Save.UserId, auction.SelectedAuctionItem.id);
        apiManager.AddPush(saveManager.Save.UserId, auction.SelectedAuctionItem.itemId, "????????? ????????? ????????? ??????????????????.");
    }

    // -------------------------------------------------------------
    // ??????
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

    // -------------------------------------------------------------
    // Collection
    // -------------------------------------------------------------
    public void AddCollectionBuff(Stat stat)
    {
        Sprite buffImage = Resources.Load<Sprite>("Skill/0");
        // collection buff id??? 0?????? ??????
        buffSystem.AddBuff(0, buffImage, stat, -1);
    }

    public void AddPassiveSkillBuff(int skillId, Stat stat)
    {
        Sprite buffImage = Resources.Load<Sprite>(string.Format("Skill/{0}", skillId));
        buffSystem.AddBuff(skillId, buffImage, stat, -1);
    }
    // public void AddEmptyBuff(int skillId, Sprite buffImage)
    // {
    //     Stat emptyStat = new Stat(true);
    //     // ????????? ??????, ????????? skill component??? player??? ???????????? ??????
    //     // ?????? ?????? ?????????, ?????? view??? ??????
    //     buffSystem.AddBuff(skillId, buffImage, emptyStat, -1);
    // }

    // -------------------------------------------------------------
    // ??????
    // -------------------------------------------------------------
    private void LoadSkillInfo()
    {
        skillSystem.Activate(saveManager.Save.Skill);
    }

    public void ActivateSkill(int skillId)
    {
        saveManager.Save.Skill.Add(skillId);
        saveManager.SaveData();
        skillSystem.Activate(skillId);
    }
    // -------------------------------------------------------------
    // ????????? System
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
    // ?????? ?????????
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

    public void PopupReward(int itemId)
    {
        uiController.PopupReward(itemManager.Get(itemId));
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
            case "Collect":
                collect.RegisterCollection();
                return;
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
            case "Reward":
                questSystem.ClearQuest();
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