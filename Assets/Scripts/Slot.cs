using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class Slot : MonoBehaviour, IPointerClickHandler
{
    // 슬롯 별
    public Item item;
    public int itemCount;
    [SerializeField]
    private Image itemImage;
    [SerializeField]
    private Image slotBackground;
    [SerializeField]
    private Image slotFrame;
    [SerializeField]
    private TextMeshProUGUI text_Count;
    [SerializeField]
    private Image image_EquipImage;
    [SerializeField]
    private GameObject go_selectedFrame;

    // 공통
    [SerializeField]
    private Canvas cv;
    [SerializeField]
    private Transform go_inventoryBtn;
    public bool isEquip;
    public bool isSelected;

    // 아이콘 이미지 활성화
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

        // Item icon, frame
        itemImage.sprite = item.ItemImage;
        slotFrame.sprite = item.ItemFrame;
        SetColor(1);

        // background, color
        slotBackground.color = item.BackgroundColor;
        // 폰트 컬러는 요기에서

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
            // 아이템이 아닌 경우 return
            return;
        }

        // 아이템 선택
        if (isSelected)
        {
            // UnSelect();
            cv.GetComponent<Inventory>().CloseItemDetail();
            HideInventoryBtn();     // 인벤토리 버튼 비활성화
        }
        else
        {
            cv.GetComponent<Inventory>().UpdateSelect(int.Parse(gameObject.name.Split('(')[1].Split(')')[0]));
            Select();
            cv.GetComponent<Inventory>().OpenItemDetail(item);
            SetInventoryBtn();      // 인벤토리 버튼 활성화
        }
    }

    // 장착
    public void Equip()
    {
        cv.GetComponent<Inventory>().EquipItemType(item.ItemType, int.Parse(gameObject.name));

        image_EquipImage.gameObject.SetActive(true);
        Debug.Log(item.ToString());
        cv.GetComponent<Inventory>().go_player.GetComponent<Stat>().Equip(item);
        cv.GetComponent<Inventory>().UpdateStatDetail();
        isEquip = true;

        SetInventoryBtn();
    }

    public void UnEquip()
    {
        cv.GetComponent<Inventory>().UnEquipItemType(item.ItemType);    // 해당 파츠 아이템을 UnEquip

        image_EquipImage.gameObject.SetActive(false);
        cv.GetComponent<Inventory>().go_player.GetComponent<Stat>().UnEquip(item);
        cv.GetComponent<Inventory>().UpdateStatDetail();
        isEquip = false;

        SetInventoryBtn();
    }

    // 선택
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
        
        HideInventoryBtn();
    }

    private void SetInventoryBtn()
    {
        string btnText = "";
        if (item.ItemType > 12)
        {
            HideInventoryBtn();
            return;
        }

        if (item.ItemType < 12)
        {
            // 장착 아이템
            if (isEquip)
            {
                btnText = "해제";
            }
            else
            {
                btnText = "장착";
            }
        }
        else if (item.ItemType == 12)
        {
            btnText = "사용";
        }

        go_inventoryBtn.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = btnText;
        go_inventoryBtn.gameObject.SetActive(true);
    }

    private void HideInventoryBtn()
    {
        // 장착, 사용 아이템이 아닌 경우
        go_inventoryBtn.gameObject.SetActive(false);
    }

    public void Use()
    {
        Debug.Log("사용");
    }
}
