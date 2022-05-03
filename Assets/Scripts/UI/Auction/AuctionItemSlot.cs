using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class AuctionItemSlot : Slot, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    private Auction auction;

    private bool isSelected;
    private bool isMyItem;

    private int index;

    public int Index { get { return index; } }
    public bool IsMyItem { get { return isMyItem; } }

    private AuctionItem datas;  // in ApiManager.cs
    public AuctionItem Datas { get { return datas; } }

    [SerializeField]
    private TextMeshProUGUI textItemPrice;
    [SerializeField]
    private TextMeshProUGUI textItemName;

    [SerializeField]
    private GameObject go_selectedFrame;
    [SerializeField]
    private GameObject myItemSyntax;

    // -------------------------------------------------------------
    // Init
    // -------------------------------------------------------------
    public void SetAuctionItem(Auction parentAuction, int auctionItemIndex, Item itemFromItemManager, AuctionItem fromServer, bool _isMyItem = false)
    {
        auction = parentAuction;
        Set(itemFromItemManager);

        index = auctionItemIndex;
        datas = fromServer;
        
        textItemName.text = itemFromItemManager.ItemName;
        textItemPrice.text = string.Format("{0:#,###}", datas.price).ToString();
        if (_isMyItem)
        {
            myItemSyntax.SetActive(true);
            isMyItem = true;
        }
    }

    // -------------------------------------------------------------
    // Select
    // -------------------------------------------------------------
    private void Select()
    {
        isSelected = true;
        go_selectedFrame.SetActive(true);
    }

    public void UnSelect()
    {
        isSelected = false;
        go_selectedFrame.SetActive(false);
    }

    // -------------------------------------------------------------
    // 경매장 아이템 슬롯 마우스 이벤트
    // -------------------------------------------------------------
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (isSelected)
            {
                auction.UnSelect();
            }
            else
            {
                // 기존 선택된 아이템을 unselect
                auction.Select(index);
                Select();
            }
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

        auction.ShowItemDetail(item, transform.position);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // 마우스 오버 아웃
        auction.CloseItemDetail();
    }
}
