using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private ItemManager itemManager;
    private SaveManager saveManager;

    [SerializeField]
    private Inventory inventory;
    public ItemManager ItemManager { get { return itemManager; } }
    public SaveManager SaveManager { get { return saveManager; } }
    public Inventory Inventory { get { return inventory; } }

    private void Awake()
    {
        itemManager = new ItemManager();
        saveManager = new SaveManager();
    }

    private void Start()
    {
        // Load Gold UI
        inventory.UpdateGold(0);
    }

    // 아이템 획득
    public void GetItem(GameObject targetObject)
    {
        if (inventory.isFullInventory())
        {
            // !!! 아이템이 꽉 참
            return;
        }

        inventory.AcquireItem(itemManager.Get(targetObject.GetComponent<DroppedItem>().Id));
    }    
}