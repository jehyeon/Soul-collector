using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Auction : MonoBehaviour
{
    private Auction myAuction;

    [SerializeField]
    private GameManager gameManager;
    [SerializeField]
    private GameObject goAuctionBuyList;
    [SerializeField]
    private GameObject goAuctionSellList;

    [SerializeField]
    private GameObject goAuctionItemPref;
    [SerializeField]
    private GameObject goBuyBtn;
    [SerializeField]
    private GameObject goSellBtn;
    [SerializeField]
    private GameObject goCancelToSellBtn;

    private int selectedAuctionItemIndex; // 현재 선택된 상점 아이템

    private AuctionItemSlot[] slots;

    // -------------------------------------------------------------
    // 경매장 아이템 로드
    // -------------------------------------------------------------
    public void LoadAuctionItemList(AuctionItemForAPI auctionItemList)
    {
        // AuctionItemForAPI에 대한 정의는 ApiManager.cs에 있음

        if (myAuction == null)
        {
            myAuction = GetComponent<Auction>();
        }

        int auctionItemIndex = 0;
        foreach(AuctionItem item in auctionItemList.result)
        {
            GameObject auctionItem = Instantiate(goAuctionItemPref);    // !!! Object pool로 수정하기
            auctionItem.transform.SetParent(goAuctionBuyList.transform);
            auctionItem.GetComponent<AuctionItemSlot>().SetAuctionItem(
                myAuction,
                auctionItemIndex,
                gameManager.ItemManager.Get(item.itemId),
                item.price
            );
            
            auctionItemIndex += 1;
        }

        slots = goAuctionBuyList.GetComponentsInChildren<AuctionItemSlot>();
    }

    // -------------------------------------------------------------
    // Select
    // -------------------------------------------------------------
    public void Select(int slotIndex)
    {
        if (selectedAuctionItemIndex != -1)
        {
            // 이미 선택된 다른 슬롯이 있는 경우
            slots[selectedAuctionItemIndex].UnSelect();
        }
        selectedAuctionItemIndex = slotIndex;
        // goBuyBtn.SetActive(true);
    }

    public void UnSelect()
    {
        slots[selectedAuctionItemIndex].UnSelect();
        selectedAuctionItemIndex = -1;
        // goBuyBtn.SetActive(false);
    }
    
    // -------------------------------------------------------------
    // Auction UI
    // -------------------------------------------------------------
    public void Open()
    {
        this.gameObject.SetActive(true);
        // !!! 경매장 오픈 시 gameManager를 통해 아이템 리스트를 요청
        // 이후 gameManager에서 Action.LoadAuctionItemList를 호출
        gameManager.RequestAuctionItemList();
    }

    public void Close()
    {
        this.gameObject.SetActive(false);
    }

    // Item detail tooltip
    public void ShowItemDetail(Item item, Vector3 pos)
    {
        gameManager.UIController.OpenItemDetail(item, pos);
    }

    public void CloseItemDetail()
    {
        gameManager.UIController.CloseItemDetail();
    }
}
