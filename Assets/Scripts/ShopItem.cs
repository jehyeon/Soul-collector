using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItem : MonoBehaviour
{
    private int itemId;
    private int price;
    private Item item;
    [SerializeField]
    private Image itemImage;
    [SerializeField]
    private Image slotFrame;
    [SerializeField]
    private Image slotBackground;
    [SerializeField]
    private TextMeshProUGUI text_itemName;
    [SerializeField]
    private TextMeshProUGUI text_itemPrice;

    public void Set(int _id, int _price)
    {
        itemId = _id;
        price = _price;
        item = gameObject.AddComponent<Item>();
        item.LoadFromCSV(itemId, "Item");
        
        itemImage.sprite = item.ItemImage;
        slotFrame.sprite = item.ItemFrame;
        slotBackground.color = item.BackgroundColor;

        text_itemName.text = item.ItemName;
        text_itemName.color = item.FontColor;
        text_itemPrice.text = price.ToString();     // "," 추가하기
    }

    public int GetId()
    {
        return itemId;
    }
}
