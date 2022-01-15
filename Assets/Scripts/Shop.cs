using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    [SerializeField]
    private GameObject pref_shopItem;
    [SerializeField]
    private Canvas cv;

    private int selectedShopItemId; // 현재 선택된 상점 아이템

    private bool isClick;
    private float clickedTime;
    private float buyingTime;
    // private int count;

    void Awake()
    {
        selectedShopItemId = -1;
        clickedTime = 0;
        isClick = false;
        // count = 0;
        List<Dictionary<string, object>> data = CSVReader.Read("Shop");

        for (int id = 0; id < data.Count; id++)
        {
            GameObject item = Instantiate(pref_shopItem);
            item.transform.SetParent(this.transform);
            item.GetComponent<ShopItem>().Set(id, (int)data[id]["itemId"], (int)data[id]["price"]);
        }
    }

    private void Update()
    {
        if (isClick)
        {
            clickedTime += Time.deltaTime;

            if (clickedTime > 1.5f)
            {
                // 1.5초 이상 누르고 있으면 0.1초마다 아이템 구입
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

        ShopItem item = this.transform.GetChild(selectedShopItemId).GetComponent<ShopItem>();
        
        cv.GetComponent<Inventory>().Buy(item.itemId, item.price);
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
}
