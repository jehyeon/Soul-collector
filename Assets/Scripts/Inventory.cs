using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public bool inventoryActivated = false;
    [SerializeField] 
    private GameObject go_inventoryBase;
    [SerializeField]
    private GameObject go_SlotsParent;

    private Slot[] slots;
    private int index;

    void Start()
    {
        slots = go_SlotsParent.GetComponentsInChildren<Slot>();
        index = 0;    
    }

    void Update()
    {
        UseInventory();
    }

    private void UseInventory()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (inventoryActivated)
            {
                CloseInventory();
            } 
            else
            {
                OpenInventory();
            }
        }
    }
    
    private void OpenInventory()
    {
        go_inventoryBase.SetActive(true);
        inventoryActivated = true;
    }

    public void CloseInventory()
    {
        go_inventoryBase.SetActive(false);
        inventoryActivated = false;
    }

    public bool AcquireItem(GameObject _item, int _count = 1)
    {
        if (index >= slots.Length)
        {
            return false;
        }

        slots[index].AddItem(_item, _count);
        index += 1;
        return true;
    }
}
