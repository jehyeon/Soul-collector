using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private ItemManager itemManager;
    private SaveManager saveManager;
    private DropManager dropManager;

    [SerializeField]
    private Player player;

    [SerializeField]
    private Inventory inventory;
    [SerializeField]
    private Equipment equipment;

    public ItemManager ItemManager { get { return itemManager; } }
    public SaveManager SaveManager { get { return saveManager; } }
    public DropManager DropManager { get { return dropManager; } }

    public Inventory Inventory { get { return inventory; } }
    public Player Player { get { return player; } }

    private void Awake()
    {
        itemManager = new ItemManager();
        saveManager = new SaveManager();
        dropManager = new DropManager();
    }

    private void Start()
    {
        // Load Inventory, Equip info, gold
        Inventory.StartInventory();
        equipment.InitEquipmentSlots();
        LoadEquipInfo();
    }

    // -------------------------------------------------------------
    // 아이템 획득
    // -------------------------------------------------------------
    public void GetItem(GameObject targetObject)
    {
        if (inventory.isFullInventory())
        {
            InventoryIsFull();
            return;
        }

        inventory.AcquireItem(itemManager.Get(targetObject.GetComponent<DroppedItem>().Id));
    }

    public void GetItem(int itemId)
    {
        if (inventory.isFullInventory())
        {
            InventoryIsFull();
            return;
        }

        inventory.AcquireItem(itemManager.Get(itemId));
    }

    private void InventoryIsFull()
    {
        // !!!
        Debug.Log("Inventory is full.");
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
}