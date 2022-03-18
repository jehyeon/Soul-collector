using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    private Shop myShop;

    [SerializeField]
    private GameManager gameManager;
    [SerializeField]
    private GameObject goShopItemParent;

    [SerializeField]
    private GameObject goShopItemPref;
    [SerializeField]
    private GameObject goBuyBtn;

    private int selectedShopItemIndex; // 현재 선택된 상점 아이템

    private bool isClick;
    private float clickedTime;
    private float buyingTime;
    // private int count;

    private ShopItemSlot[] slots;

    void Start()
    {
        selectedShopItemIndex = -1;
        clickedTime = 0;
        myShop = GetComponent<Shop>();
        isClick = false;
    }

    public void InitShopItemSlots()
    {
        if (myShop == null)
        {
            myShop = GetComponent<Shop>();
        }

        for (int id = 0; id < gameManager.ShopManager.data.Count; id++)
        {
            GameObject item = Instantiate(goShopItemPref);
            item.transform.SetParent(goShopItemParent.transform);
            item.GetComponent<ShopItemSlot>().SetShopItem(
                myShop, 
                gameManager.ItemManager.Get((int)gameManager.ShopManager.data[id]["itemId"]), 
                id, 
                (int)gameManager.ShopManager.data[id]["price"]
            );
        }

        slots = goShopItemParent.GetComponentsInChildren<ShopItemSlot>();
    }

    private void Update()
    {
        if (isClick)
        {
            clickedTime += Time.deltaTime;

            if (clickedTime > 1f)
            {
                // 1초 이상 누르고 있으면 0.1초마다 아이템 구입
                buyingTime += Time.deltaTime;
                if (buyingTime > .1f)
                {
                    Buy();
                    buyingTime = 0f;
                }
            }
        }
    }

    public void Buy()
    {
        if (selectedShopItemIndex == -1)
        {
            // 아무것도 선택되지 않음
            return;
        }

        gameManager.Inventory.Buy(slots[selectedShopItemIndex].Item, slots[selectedShopItemIndex].Price);
    }

    // -------------------------------------------------------------
    // Select
    // -------------------------------------------------------------
    public void Select(int slotIndex)
    {
        if (selectedShopItemIndex != -1)
        {
            // 이미 선택된 다른 슬롯이 있는 경우
            slots[selectedShopItemIndex].UnSelect();
        }
        selectedShopItemIndex = slotIndex;
        goBuyBtn.SetActive(true);
    }

    public void UnSelect()
    {
        if (selectedShopItemIndex == -1)
        {
            // 아무것도 선택되지 않음
            return;
        }

        slots[selectedShopItemIndex].UnSelect();
        selectedShopItemIndex = -1;
        goBuyBtn.SetActive(false);
    }

    // !!!
    public void ButtonDown()
    {
        isClick = true;
    }

    public void ButtonUp()
    {
        isClick = false;
        clickedTime = 0f;
        buyingTime = 0f;
    }

    // Shop UI
    public void Open()
    {
        this.gameObject.SetActive(true);
    }

    public void Close()
    {
        this.gameObject.SetActive(false);
        isClick = false;
        clickedTime = 0f;
    }
    
    // Item detail tooltip
    public void ShowItemDetail(Item item, Vector3 pos)
    {
        gameManager.UIController.ItemDetail.Open(item, pos);
    }

    public void CloseItemDetail()
    {
        gameManager.UIController.ItemDetail.Close();
    }

}
