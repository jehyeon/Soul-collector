using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class Slot : MonoBehaviour, IPointerClickHandler
{
    public Item item;
    public int itemCount;
    [SerializeField]
    private Image itemImage;
    [SerializeField]
    private Image slotBackground;
    [SerializeField]
    private Image slotFrame;
    public bool isEquip;
    public bool isSelected;

    [SerializeField]
    private Canvas cv;

    [SerializeField]
    private TextMeshProUGUI text_Count;     // Item Count
    [SerializeField]
    private Image image_EquipImage;
    [SerializeField]
    private GameObject go_selectedFrame;

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

        // Item icon
        itemImage.sprite = Resources.Load<Sprite>("Item Images/" + item.Id);
        SetColor(1);

        // Item rank
        Color backgroundColor;
        if (item.Rank == 2 || item.Rank == 7)
        {
            // green
            ColorUtility.TryParseHtmlString("#142E22FF", out backgroundColor);
            slotFrame.sprite = Resources.Load<Sprite>("sprites/frame_2");
        }
        else if (item.Rank == 3 || item.Rank == 8)
        {
            // blue
            ColorUtility.TryParseHtmlString("#021334FF", out backgroundColor);
            slotFrame.sprite = Resources.Load<Sprite>("sprites/frame_3");
        }
        else if (item.Rank == 4 || item.Rank == 9)
        {
            // red
            ColorUtility.TryParseHtmlString("#270A08FF", out backgroundColor);
            slotFrame.sprite = Resources.Load<Sprite>("sprites/frame_4");
        }
        else if (item.Rank == 5)
        {
            // purple
            ColorUtility.TryParseHtmlString("#210D34FF", out backgroundColor);
            slotFrame.sprite = Resources.Load<Sprite>("sprites/frame_5");
        }
        else
        {
            // Include rank 1, 6
            // default
            ColorUtility.TryParseHtmlString("#28241DFF", out backgroundColor);
            slotFrame.sprite = Resources.Load<Sprite>("sprites/frame_1");
        }

        // Slot background
        slotBackground.color = backgroundColor;

        if (item.ItemType > 11)
        {
            // 장착 아이템이 아닌 경우 item count 활성화
            text_Count.text = itemCount.ToString();
        }
        else
        {
            text_Count.text = "";
        }

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
        itemImage.sprite = null;
        SetColor(0);

        itemCount = 0;
        text_Count.text = "";

        Color backgroundColor;
        ColorUtility.TryParseHtmlString("#28241DFF", out backgroundColor);
        slotBackground.color = backgroundColor;

        slotFrame.sprite = Resources.Load<Sprite>("sprites/frame_1");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (item == null)
        {
            return;
        }

        if (isSelected)
        {
            // UnSelect();
            cv.GetComponent<Inventory>().CloseItemDetail();
        }
        else
        {
            cv.GetComponent<Inventory>().UpdateSelect(int.Parse(gameObject.name));
            Select();
            cv.GetComponent<Inventory>().OpenItemDetail(item, itemImage, slotFrame, slotBackground.color);
        }
    }

    private void Equip()
    {
        cv.GetComponent<Inventory>().EquipItemType(item.ItemType, int.Parse(gameObject.name));

        image_EquipImage.gameObject.SetActive(true);
        Debug.Log(item.ToString());
        cv.GetComponent<Inventory>().go_player.GetComponent<Stat>().Equip(item);
        cv.GetComponent<Inventory>().UpdateStatDetail();
        isEquip = true;
    }

    public void UnEquip()
    {
        cv.GetComponent<Inventory>().UnEquipItemType(item.ItemType);    // 해당 파츠 아이템을 UnEquip

        image_EquipImage.gameObject.SetActive(false);
        cv.GetComponent<Inventory>().go_player.GetComponent<Stat>().UnEquip(item);
        cv.GetComponent<Inventory>().UpdateStatDetail();
        isEquip = false;
    }

    private void Select()
    {
        isSelected = true;
        go_selectedFrame.SetActive(isSelected);
    }

    public void UnSelect()
    {
        // Inventory.cs 에서 Unselect 할때 쓰임
        isSelected = false;
        go_selectedFrame.SetActive(isSelected);
    }
}
