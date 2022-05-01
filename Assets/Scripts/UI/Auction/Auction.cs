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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // -------------------------------------------------------------
    // 경매장 아이템 로드
    // -------------------------------------------------------------

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
        goBuyBtn.SetActive(true);
    }

    public void UnSelect()
    {
        slots[selectedAuctionItemIndex].UnSelect();
        selectedAuctionItemIndex = -1;
        goBuyBtn.SetActive(false);
    }
    
    // -------------------------------------------------------------
    // Auction UI
    // -------------------------------------------------------------
    public void Open()
    {
        this.gameObject.SetActive(true);
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
