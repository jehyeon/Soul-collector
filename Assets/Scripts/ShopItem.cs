using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ShopItem : Slot, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    // select 된 shopItemId를 shop.cs에서 동기화하고
    // 구입 시 ShopItem[shopItemId]의 id와 price에 접근
    private int id;
    private int price;

    private Shop shop;

    [SerializeField]
    private TextMeshProUGUI text_itemPrice;

    [SerializeField]
    private GameObject go_selectedFrame;

    private bool isSelected;

    public int Price { get { return price; } }

    public void SetShopItem(Shop parentShop, Item itemFromItemManager, int shopItemId, int itemPrice)
    {
        shop = parentShop;
        Set(itemFromItemManager);

        id = shopItemId;
        price = itemPrice;
        
        text_itemPrice.text = string.Format("{0:#,###}", price).ToString();
    }

    // -------------------------------------------------------------
    // 상점 아이템 슬롯 마우스 이벤트
    // -------------------------------------------------------------
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

    public void OnPointerEnter(PointerEventData eventData)
    {
        // 마우스 오버
        if (item == null)
        {
            // 아이템이 없는 경우 그냥 return
            return;
        }

        shop.ShowItemDetail(item, transform.position);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // 마우스 오버 아웃
        shop.CloseItemDetail();
    }

    private void Select()
    {
        isSelected = true;
        go_selectedFrame.SetActive(isSelected);
        this.transform.parent.gameObject.GetComponent<Shop>().SetSelectedShopItemId(id);
    }

    public void UnSelect()
    {
        isSelected = false;
        go_selectedFrame.SetActive(isSelected);
    }
}
