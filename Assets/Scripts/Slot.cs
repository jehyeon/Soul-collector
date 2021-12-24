using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    public GameObject item;
    public int itemCount;
    public Image itemImage;
    public bool isEquip;

    [SerializeField]
    private Text text_Count;
    [SerializeField]
    private Image image_EquipImage;
    [SerializeField]
    private Image image_TempBackground;
    
    private void SetColor(float _alpha)
    {
        Color color = itemImage.color;
        color.a = _alpha;
        itemImage.color = color;

        image_TempBackground.color = color;
    }

    // 슬롯에 아이템 추가
    public void AddItem(GameObject _item, int _count = 1)
    {
        item = _item;
        itemCount = _count;
        itemImage.sprite = item.GetComponent<Item>().itemImage;

        // if (item.itemType != Item.ItemType.Equipment)
        // {
        //     // 장착 아이템이 아닌 경우 item count 활성화
        //     text_Count.text = itemCount.ToString();
        // }
        // else
        // {
        //     text_Count.text = "";
        // }

        // Temp
        TempColor(item.GetComponent<Item>().id);

        SetColor(1);
    }

    // 아이템 수량 변경
    public void SetSlotCount(int _count)
    {
        itemCount += _count;
        text_Count.text = itemCount.ToString();

        if (itemCount <= 0)
        {
            ClearSlot();
        }
    }

    // 슬롯 비우기
    private void ClearSlot()
    {
        item = null;
        itemCount = 0;
        itemImage.sprite = null;
        SetColor(0);

        text_Count.text = "";
    }

    private void TempColor(int itemCode)
    {
        // "#ffffff00", "#487B46FF", "#465D7BFF", "#7B4C46FF", "#74467BFF"
        
        switch (itemCode)
        {
            case 1:
                image_TempBackground.color = new Color(72/255f, 123/255f, 70/255f);
                break;
            case 2:
                image_TempBackground.color = new Color(70/255f, 93/255f, 123/255f);
                break;
            case 3:
                image_TempBackground.color = new Color(123/255f, 76/255f, 70/255f);
                break;
            case 4:
                image_TempBackground.color = new Color(116/255f, 70/255f, 123/255f);
                break;
        }
    }
}
