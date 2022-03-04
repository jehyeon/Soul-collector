using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : Slot
{
    private int slotIndex;

    private int slotId;
    public bool isEquip;
    public bool isSelected;
    public int upgradeLevel;

    [SerializeField]
    private Image image_equipped;
    [SerializeField]
    private GameObject go_selectedFrame;

    private void Start()
    {
        slotIndex = -1;
        if (gameObject.name != "Item Image")
        {
            slotIndex = int.Parse(gameObject.name.Split('(')[1].Split(')')[0]);
        }
    }

    public override void ClearSlot()
    {
        base.ClearSlot();

        UnEquip();
    }

    public void UnEquip()
    {
        if (!isEquip)
        {
            return;
        }

        image_equipped.gameObject.SetActive(false);
        isEquip = false;
    }
}