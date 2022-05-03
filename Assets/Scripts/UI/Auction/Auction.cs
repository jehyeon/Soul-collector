using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AuctionMode
{
    Buy,
    Sell
}

public class Auction : MonoBehaviour
{
    private Auction myAuction;
    private AuctionMode mode = AuctionMode.Buy;

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
    private GameObject goCancelToSellBtn;

    private int selectedAuctionItemIndex;   // 현재 선택된 경매장 아이템
    private AuctionItem selectedAuctionItem;             // 현재 선택된 경매장 아이템
    public AuctionItem SelectedAuctionItem { get { return selectedAuctionItem; } }
    public AuctionItemSlot[] buySlots;
    public AuctionItemSlot[] sellSlots;

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
        int myAuctionItemIndex = 0;
        foreach(AuctionItem item in auctionItemList.result)
        {
            if (item.userId == gameManager.SaveManager.Save.UserId)
            {
                // 경매장에 등록된 아이템이 내 아이템인 경우
                // 아이템 판매 리스트에도 추가하기 (not good)
                GameObject myItem = Instantiate(goAuctionItemPref);    // !!! Object pool로 수정하기
                myItem.transform.SetParent(goAuctionSellList.transform);
                myItem.GetComponent<AuctionItemSlot>().SetAuctionItem(
                    myAuction,
                    myAuctionItemIndex,
                    gameManager.ItemManager.Get(item.itemId),
                    item,
                    true
                );      // true 옵션 적용

                myAuctionItemIndex += 1;
            }

            GameObject auctionItem = Instantiate(goAuctionItemPref);    // !!! Object pool로 수정하기
            auctionItem.transform.SetParent(goAuctionBuyList.transform);
            auctionItem.GetComponent<AuctionItemSlot>().SetAuctionItem(
                myAuction,
                auctionItemIndex,
                gameManager.ItemManager.Get(item.itemId),
                item,
                item.userId == gameManager.SaveManager.Save.UserId
            );
            
            auctionItemIndex += 1;
        }

        buySlots = goAuctionBuyList.GetComponentsInChildren<AuctionItemSlot>();
        sellSlots = goAuctionSellList.GetComponentsInChildren<AuctionItemSlot>();
    }

    public void ClearAuctionList()
    {
        // !!! 임시
        for (int i = 0; i < buySlots.Length; i++)
        {
            Destroy(buySlots[i].gameObject);
        }
        for (int i = 0; i < sellSlots.Length; i++)
        {
            Destroy(sellSlots[i].gameObject);
        }
        buySlots = null;
        sellSlots = null;
    }
    // -------------------------------------------------------------
    // 경매장 아이템 추가
    // -------------------------------------------------------------
    public void AddItemToAuction(AuctionItem item)
    {
        // slot index를 최신으로 바꾸기 위함
        buySlots = goAuctionBuyList.GetComponentsInChildren<AuctionItemSlot>();
        sellSlots = goAuctionSellList.GetComponentsInChildren<AuctionItemSlot>();

        GameObject auctionItemToBuy = Instantiate(goAuctionItemPref);    // !!! Object pool로 수정하기
        auctionItemToBuy.transform.SetParent(goAuctionBuyList.transform);
        auctionItemToBuy.GetComponent<AuctionItemSlot>().SetAuctionItem(
            myAuction,
            buySlots.Length,
            gameManager.ItemManager.Get(item.itemId),
            item,
            true
        );

        GameObject auctionItemToSell = Instantiate(goAuctionItemPref);    // !!! Object pool로 수정하기
        auctionItemToSell.transform.SetParent(goAuctionSellList.transform);
        auctionItemToSell.GetComponent<AuctionItemSlot>().SetAuctionItem(
            myAuction,
            sellSlots.Length,
            gameManager.ItemManager.Get(item.itemId),
            item,
            true
        );

        // not good
        buySlots = goAuctionBuyList.GetComponentsInChildren<AuctionItemSlot>();
        sellSlots = goAuctionSellList.GetComponentsInChildren<AuctionItemSlot>();
    }

    // -------------------------------------------------------------
    // 아이템 구매, 판매 취소
    // -------------------------------------------------------------
    public void Buy()
    {
        // AuctionMode.Buy일 때만 호출 가능
        if (selectedAuctionItemIndex == -1)
        {
            return;
        }
        // buySlots[]

        if (gameManager.SaveManager.Save.Gold < buySlots[selectedAuctionItemIndex].Datas.price)
        {
            // 판매 금액이 소지 금액보다 많은 경우
            gameManager.PopupMessage("골드가 부족합니다.");
            return;
        }

        gameManager.PopupAsk("BuyAuction", "아이템을 구매하시겠습니까?", "아니요", "네");
    }

    public void BuyDone()
    {
        // 경매장 슬롯 deactive
        buySlots[selectedAuctionItemIndex].gameObject.SetActive(false);
        UnSelect();
    }

    public void CancleToSell()
    {
        // AuctionMode.Sell일 때만 호출 가능
        if (selectedAuctionItemIndex == -1)
        {
            return;
        }
        Debug.Log(SelectedAuctionItem.id);
        Debug.Log(SelectedAuctionItem.userId);
        Debug.Log(SelectedAuctionItem.itemId);
        gameManager.CancelToSell();

        foreach(AuctionItemSlot item in buySlots)
        {
            // 구매 목록에서 지우기
            if (item.Datas.id == sellSlots[selectedAuctionItemIndex].Datas.id)
            {
                item.gameObject.SetActive(false);
                continue;
            }
        }

        // 판매 목록에서 지우기
        sellSlots[selectedAuctionItemIndex].gameObject.SetActive(false);
        UnSelect();
    }

    // -------------------------------------------------------------
    // Select
    // -------------------------------------------------------------
    public void Select(int slotIndex)
    {
        AuctionItemSlot[] slots;
        if (mode == AuctionMode.Buy)
        {
            slots = buySlots;
        }
        else
        {
            slots = sellSlots;
        }

        if (selectedAuctionItemIndex != -1)
        {
            // 이미 선택된 다른 슬롯이 있는 경우
            slots[selectedAuctionItemIndex].UnSelect();
        }
        selectedAuctionItemIndex = slotIndex;
        selectedAuctionItem = slots[selectedAuctionItemIndex].Datas;
        if (!slots[selectedAuctionItemIndex].IsMyItem && mode == AuctionMode.Buy)
        {
            // 구매 모드일 때, 내 등록 아이템이 아닌 경우
            goBuyBtn.SetActive(true);
        }
        else if (slots[selectedAuctionItemIndex].IsMyItem && mode == AuctionMode.Sell)
        {
            // 판매 모드 일 때, 내 등록 아이템인 경우
            goCancelToSellBtn.SetActive(true);
        }
        else
        {
            goBuyBtn.SetActive(false);
            goCancelToSellBtn.SetActive(false);
        }
    }

    public void UnSelect()
    {
        if (selectedAuctionItemIndex == -1)
        {
            return;
        }

        AuctionItemSlot[] slots;
        if (mode == AuctionMode.Buy)
        {
            slots = buySlots;
        }
        else
        {
            slots = sellSlots;
        }

        slots[selectedAuctionItemIndex].UnSelect();
        selectedAuctionItemIndex = -1;
        goBuyBtn.SetActive(false);
        goCancelToSellBtn.SetActive(false);
    }
    
    // -------------------------------------------------------------
    // Mode
    // -------------------------------------------------------------
    public void SelectMode(int index)
    {
        // SelectMode(AuctionMode selectedMode)는 Click event에서 안보여서 임시로 만듬
        if (index == 0)
        {
            SelectMode(AuctionMode.Buy);
        }
        else
        {
            SelectMode(AuctionMode.Sell);
        }
    }
    public void SelectMode(AuctionMode selectedMode)
    {
        if (mode == selectedMode)
        {
            return;
        }
        
        if (selectedAuctionItemIndex != -1)
        {
            // 선택된 아이템이 있는 경우
            UnSelect();
        }

        mode = selectedMode;
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
        SelectMode(AuctionMode.Buy);
        GetComponent<UITab>().Open(0);
    }

    public void Close()
    {
        this.gameObject.SetActive(false);
        ClearAuctionList();
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
