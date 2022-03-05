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
}