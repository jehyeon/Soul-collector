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

    private ShopItemSlot[] slots;

    void Start()
    {
        selectedShopItemIndex = -1;
        myShop = GetComponent<Shop>();
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

    public void ClickBuy()
    {
        // 1개도 구입 못하는 경우
        if (gameManager.SaveManager.Save.Gold < slots[selectedShopItemIndex].Price)
        {
            gameManager.PopupMessage("골드가 부족합니다.");
            return;
        }

        if (gameManager.Inventory.isFullInventory())
        {
            gameManager.PopupMessage("인벤토리에 남은 공간이 없습니다.");
            return;
        }

        int maxCount = slots[selectedShopItemIndex].Item.ItemType == ItemType.Use || slots[selectedShopItemIndex].Item.ItemType == ItemType.Material
            ? 999
            : gameManager.Inventory.GetRemainInventory();

        // gameManager.PopupAsk("Shop", "아이템을 구매하시겠습니까?", "아니요", "네");
        gameManager.PopupSetCount("Shop", "구매할 아이템의 수량을 입력해주세요,", "취소", "구매"
            , maxCount, 1);
    }

    public void Buy(int count = 1)
    {
        if (selectedShopItemIndex == -1)
        {
            // 아무것도 선택되지 않음
            return;
        }

        // 인벤토리 남은 공간 이상의 수량 구입이 불가능
        if (gameManager.SaveManager.Save.Gold < slots[selectedShopItemIndex].Price * count)
        {
            gameManager.PopupMessage("골드가 부족합니다.");
            return;
        }
        gameManager.Inventory.Buy(slots[selectedShopItemIndex].Item, slots[selectedShopItemIndex].Price, count);
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
        slots[selectedShopItemIndex].UnSelect();
        selectedShopItemIndex = -1;
        goBuyBtn.SetActive(false);
    }

    // Shop UI
    public void Open()
    {
        this.gameObject.SetActive(true);
    }

    public void Close()
    {
        if (selectedShopItemIndex != -1)
        {
            UnSelect();
        }
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
