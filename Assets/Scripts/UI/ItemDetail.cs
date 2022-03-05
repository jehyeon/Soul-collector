using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemDetail : MonoBehaviour
{
    [SerializeField]
    private Slot slot;
    [SerializeField]
    private TextMeshProUGUI itemName;
    [SerializeField]
    private TextMeshProUGUI itemDes;

    public void Open(Item selectedItem)
    {
        slot.Set(selectedItem);
        itemName.text = selectedItem.ItemName;
        itemName.color = selectedItem.FontColor;
        itemDes.text = selectedItem.ToString();

        this.gameObject.SetActive(true);
    }

    public void Close()
    {
        this.gameObject.SetActive(false);
        Clear();
    }

    private void Clear()
    {
        slot.ClearSlot();
        itemName.text = "";
        itemName.color = Color.white;
        itemDes.text = "";
    }
}
