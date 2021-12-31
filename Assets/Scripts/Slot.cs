using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IPointerClickHandler
{
    Item item;
    public int itemCount;
    public Image itemImage;
    public bool isEquip;

    [SerializeField]
    private Canvas cv;

    [SerializeField]
    private Text text_Count;
    [SerializeField]
    private Image image_EquipImage;
    
    // 더블 클릭
    private float currentClickTime;

    private void SetColor(float _alpha)
    {
        Color color = itemImage.color;
        color.a = _alpha;
        itemImage.color = color;
    }

    // 슬롯에 아이템 추가
    public void AddItem(int _id, int _count = 1)
    {
        item = gameObject.AddComponent<Item>();
        item.LoadFromCSV(_id, "Item");
        itemCount = _count;
        itemImage.sprite = Resources.Load<Sprite>("Item Images/" + _id);

        // if (item.itemType != Item.ItemType.Equipment)
        // {
        //     // 장착 아이템이 아닌 경우 item count 활성화
        //     text_Count.text = itemCount.ToString();
        // }
        // else
        // {
        //     text_Count.text = "";
        // }

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

    public void OnPointerClick(PointerEventData eventData)
    {
        if (item == null)
        {
            return;
        }

        if ((Time.time - currentClickTime) < 0.3f)
        {
            if (!isEquip)
            {
                Equip();
            }
            else
            {
                UnEquip();
            }
            currentClickTime = -1;
        }
        else
        {
            currentClickTime = Time.time;
        }

        cv.GetComponent<Inventory>().OpenItemDetail(item, itemImage);
    }

    private void Equip()
    {
        image_EquipImage.gameObject.SetActive(true);
        cv.GetComponent<Inventory>().go_player.GetComponent<Stat>().Equip(item);

        isEquip = true;
    }

    private void UnEquip()
    {
        image_EquipImage.gameObject.SetActive(false);
        cv.GetComponent<Inventory>().go_player.GetComponent<Stat>().UnEquip(item);
        
        isEquip = false;
    }
}
