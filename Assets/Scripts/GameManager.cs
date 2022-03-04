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

    private void Awake()
    {
        itemManager = new ItemManager();
        saveManager = new SaveManager();
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