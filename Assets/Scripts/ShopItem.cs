using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ShopItem : MonoBehaviour, IPointerClickHandler
{
    // select 된 shopItemId를 shop.cs에서 동기화하고
    // 구입 시 ShopItem[shopItemId]의 id와 price에 접근
    private int shopItemId;
    public int itemId;
    public int price;

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

    [SerializeField]
    private GameObject go_selectedFrame;

    private bool isSelected;
    public void Set(int _shopItemId, int _id, int _price)
    {
        shopItemId = _shopItemId;
        itemId = _id;
        price = _price;
        // item = gameObject.AddComponent<Item>();
        // item.LoadFromCSV(itemId, "Item");
        item = GameObject.Find("Item Manager").GetComponent<ItemManager>().Get(itemId);
        
        itemImage.sprite = item.ItemImage;
        slotFrame.sprite = item.ItemFrame;
        slotBackground.color = item.BackgroundColor;

        text_itemName.text = item.ItemName;
        text_itemName.color = item.FontColor;
        text_itemPrice.text = string.Format("{0:#,###}", price).ToString();
    }

    public int GetId()
    {
        return itemId;
    }

    // 클릭 이벤트
    public void OnPointerClick(PointerEventData eventData)
    {
        // 기존 선택된 아이템을 unselect
        this.transform.parent.gameObject.GetComponent<Shop>().UnSelect();
        
        if (isSelected)
        {
            UnSelect();
        }
        else
        {
            Select();
        }
    }

    private void Select()
    {
        isSelected = true;
        go_selectedFrame.SetActive(isSelected);
        this.transform.parent.gameObject.GetComponent<Shop>().SetSelectedShopItemId(shopItemId);
    }

    public void UnSelect()
    {
        isSelected = false;
        go_selectedFrame.SetActive(isSelected);
    }
}
