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

    private int slotIndex;

    // 공통
    [SerializeField]
    private Canvas cv;
    [SerializeField]
    private Transform go_inventoryBtn;

    public bool isEquip;
    public bool isSelected;
    public int upgradeLevel;

    private void Start()
    {
        slotIndex = -1;
        if (gameObject.name != "Item Image")
        {
            slotIndex = int.Parse(gameObject.name.Split('(')[1].Split(')')[0]);
        }
    }

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
        item = GameObject.Find("Item Manager").GetComponent<ItemManager>().Get(_id);
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
            // 장착 아이템인 경우 강화 수치 표기
            if (item.ItemName.IndexOf("+") > -1)
            {
                upgradeLevel = int.Parse(item.ItemName.Split(' ')[0].Split('+')[1]);
                text_Count.text = "+" + upgradeLevel;

                Color itemCountColor;
                ColorUtility.TryParseHtmlString("#FFFF00FF", out itemCountColor);
                text_Count.color = itemCountColor;
            }
            else
            {
                text_Count.text = "";
            }
        }

    }

    // 강화
    public void Upgrade()
    {
        item = GameObject.Find("Item Manager").GetComponent<ItemManager>().Get(item.Id + 1);

        // 장착 아이템인 경우 강화 수치 표기
        if (item.ItemName.IndexOf("+") > -1)
        {
            upgradeLevel = int.Parse(item.ItemName.Split(' ')[0].Split('+')[1]);
            text_Count.text = "+" + upgradeLevel;
            Color fontColor;
            ColorUtility.TryParseHtmlString("#FFFF00FF", out fontColor);
            text_Count.color = fontColor;
        }
        else
        {
            text_Count.text = "";
            Color backgroundColor;
            ColorUtility.TryParseHtmlString("#FFFFFFFF", out backgroundColor);
            text_Count.color = backgroundColor;
        }
    }

    // 아이템 수량 변경
    public void SetSlotCount(int _count)
    {
        itemCount += _count;
        text_Count.text = itemCount.ToString();
    }

    // 슬롯 비우기
    public void ClearSlot()
    {
        // 아이템, 아이템 이미지 초기화
        item = null;
        itemImage.sprite = null;
        SetColor(0);

        // background, slot frame 초기화
        Color backgroundColor;
        ColorUtility.TryParseHtmlString("#28241DFF", out backgroundColor);
        slotBackground.color = backgroundColor;
        slotFrame.sprite = Resources.Load<Sprite>("sprites/frame_1");

        // count 초기화
        itemCount = 0;
        text_Count.text = "";
        Color itemCountColor;
        ColorUtility.TryParseHtmlString("#FFFFFFFF", out itemCountColor);
        text_Count.color = itemCountColor;

        // 슬롯 unselect
        UnSelect();

        // 장착 해제
        UnEquip();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (item == null)
        {
            // 아이템이 아닌 경우 return
            return;
        }

        if (cv.GetComponent<Inventory>().reinforceMode)
        {
            if (item.Rank > 5)
            {
                // 강화 불가능한 등급 (ex. 9강, resource)
                return;
            }

            // 강화 모드인 경우
            int scrollType = cv.GetComponent<Inventory>().scrollType;
            
            if (scrollType == 15)
            {
                // 무기 강화
                if (!(item.Id >= 0 && item.Id <= 499))
                {
                    // 무기가 아닌 경우
                    return;
                }
                cv.GetComponent<Inventory>().Rush(slotIndex);
            }
            else if (scrollType == 16)
            {
                // 방어구 강화
                if (!(item.Id >= 500 && item.Id <= 1299))
                {
                    // 방어구가 아닌 경우
                    return;
                }
                cv.GetComponent<Inventory>().Rush(slotIndex);
            }
            else if (scrollType == 17)
            {
                // 장신구 강화
                if (!(item.Id >= 1300 && item.Id <= 1600))
                {
                    // 장신구가 아닌 경우
                    return;
                }
                cv.GetComponent<Inventory>().Rush(slotIndex);
            }
            else if (scrollType == 18)
            {
                // 소울스톤 강화
                if (!(item.Id >= 1608 && item.Id <= 1613))
                {
                    // 소울스톤이 아닌 경우
                    return;
                }

                // temp
            }

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
            cv.GetComponent<Inventory>().UpdateSelect(slotIndex);
            Select();
            cv.GetComponent<Inventory>().OpenItemDetail(item);
            SetInventoryBtn();      // 인벤토리 버튼 활성화
        }
    }

    // 장착
    public void Equip()
    {
        cv.GetComponent<Inventory>().EquipItemType(item.ItemType, slotIndex);

        image_EquipImage.gameObject.SetActive(true);
        cv.GetComponent<Inventory>().go_player.GetComponent<Stat>().Equip(item);
        cv.GetComponent<Inventory>().UpdateStatDetail();
        isEquip = true;

        SetInventoryBtn();
    }

    public void UnEquip()
    {
        if (!isEquip)
        {
            return;
        }
        
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
        cv.GetComponent<Inventory>().UpdateSelect(-1);  // inventory selectedIndex -1로 초기화
    }

    private void SetInventoryBtn()
    {
        string btnText = "";
        if (item.ItemType == 12)
        {
            // 제작 재료 아이템
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
        else if (item.ItemType > 12)
        {
            // 사용 아이템
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
}
