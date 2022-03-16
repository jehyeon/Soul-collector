using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    private GameManager gameManager;
    [SerializeField]
    private GameObject go_shop;

    [SerializeField]
    private GameObject pref_shopItem;

    // 아이템 상세 정보
    [SerializeField]
    private ItemDetail itemDetail;

    private int selectedShopItemId; // 현재 선택된 상점 아이템

    private bool isClick;
    private float clickedTime;
    private float buyingTime;
    // private int count;

    void Awake()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();

        selectedShopItemId = -1;
        clickedTime = 0;
        isClick = false;

        List<Dictionary<string, object>> data = CSVReader.Read("Shop");

        for (int id = 0; id < data.Count; id++)
        {
            GameObject item = Instantiate(pref_shopItem);
            item.transform.SetParent(this.transform);
            Item shopItem = gameManager.ItemManager.Get((int)data[id]["itemId"]);
            item.GetComponent<ShopItem>().SetShopItem(this.GetComponent<Shop>(), shopItem, id, (int)data[id]["price"]);
        }
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
        if (selectedShopItemId == -1)
        {
            // 아무것도 선택되지 않음
            return;
        }

        ShopItem shopItem = this.transform.GetChild(selectedShopItemId).GetComponent<ShopItem>();
        gameManager.Inventory.Buy(shopItem.Item, shopItem.Price);
    }

    public void UnSelect()
    {
        if (selectedShopItemId == -1 || this.transform.childCount == 0)
        {
            // 아무것도 선택되지 않음
            return;
        }

        this.transform.GetChild(selectedShopItemId).GetComponent<ShopItem>().UnSelect();
        selectedShopItemId = -1;
    }

    public void SetSelectedShopItemId(int _selectedShopItemId)
    {
        selectedShopItemId = _selectedShopItemId;
    }

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
        go_shop.SetActive(true);
    }

    public void Close()
    {
        go_shop.SetActive(false);
        isClick = false;
        clickedTime = 0f;
    }
    
    // Item detail tooltip
    public void ShowItemDetail(Item item, Vector3 pos)
    {
        itemDetail.Open(item, pos);
    }

    public void CloseItemDetail()
    {
        itemDetail.Close();
    }

}
