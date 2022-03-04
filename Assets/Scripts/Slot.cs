using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class Slot : MonoBehaviour
{
    // 슬롯 별
    private Item item;
    private int itemCount;

    // 아이템 이미지
    [SerializeField]
    private Image img_item;

    // 아이템 등급 배경색
    [SerializeField]
    private Image img_background;

    // 아이템 프레임
    [SerializeField]
    private Image img_frame;

    // 아이템 개수 or 강화 등급 표기
    [SerializeField]
    private TextMeshProUGUI text_count;

    public int ItemCount { get { return itemCount; } }

    private void SetColor(float alpha)
    {
        // 1이면 아이템 이미지 활성화, 0이면 비활성화
        Color color = img_item.color;
        color.a = alpha;
        img_item.color = color;
    }

    // 슬롯에 아이템 추가
    public void Set(Item itemFromItemManager, int count = 1)
    {
        item = itemFromItemManager;

        // slot view 설정
        // Item icon, frame
        img_item.sprite = item.ItemImage;
        img_frame.sprite = item.ItemFrame;
        SetColor(1);

        // background, color
        img_background.color = item.BackgroundColor;

        itemCount = count;

        // if (item.ItemType > 11)
        // {
        //     // 장착 아이템이 아닌 경우 item count 활성화
        //     text_Count.text = itemCount.ToString();
        // }
        // else
        // {
        //     // 장착 아이템인 경우 강화 수치 표기
        //     if (item.ItemName.IndexOf("+") > -1)
        //     {
        //         upgradeLevel = int.Parse(item.ItemName.Split(' ')[0].Split('+')[1]);
        //         text_Count.text = "+" + upgradeLevel;

        //         Color itemCountColor;
        //         ColorUtility.TryParseHtmlString("#FFFF00FF", out itemCountColor);
        //         text_Count.color = itemCountColor;
        //     }
        //     else
        //     {
        //         text_Count.text = "";
        //     }
        // }
    }

    // 강화
    // public void Upgrade()
    // {
    //     item = itemManager.Get(item.Id + 1);

    //     // 장착 아이템인 경우 강화 수치 표기
    //     if (item.ItemName.IndexOf("+") > -1)
    //     {
    //         upgradeLevel = int.Parse(item.ItemName.Split(' ')[0].Split('+')[1]);
    //         text_Count.text = "+" + upgradeLevel;
    //         Color fontColor;
    //         ColorUtility.TryParseHtmlString("#FFFF00FF", out fontColor);
    //         text_Count.color = fontColor;
    //     }
    //     else
    //     {
    //         text_Count.text = "";
    //         Color backgroundColor;
    //         ColorUtility.TryParseHtmlString("#FFFFFFFF", out backgroundColor);
    //         text_Count.color = backgroundColor;
    //     }
    // }

    // 아이템 수량 변경
    public void SetSlotCount(int _count)
    {
        itemCount = _count;
        text_count.text = itemCount.ToString();
    }

    // 슬롯 비우기
    public virtual void ClearSlot()
    {
        // 아이템, 아이템 이미지 초기화
        item = null;
        img_item.sprite = null;
        SetColor(0);

        // background, slot frame 초기화
        Color backgroundColor;
        ColorUtility.TryParseHtmlString("#28241DFF", out backgroundColor);
        img_background.color = backgroundColor;
        img_frame.sprite = Resources.Load<Sprite>("sprites/frame_1");

        // count 초기화
        itemCount = 0;
        text_count.text = "";
        Color itemCountColor;
        ColorUtility.TryParseHtmlString("#FFFFFFFF", out itemCountColor);
        text_count.color = itemCountColor;

        // // 슬롯 unselect
        // UnSelect();

        // // 장착 해제
        // UnEquip();
    }

    // public void OnPointerClick(PointerEventData eventData)
    // {
    //     if (item == null)
    //     {
    //         // 아이템이 아닌 경우 return
    //         return;
    //     }

    //     if (cv.GetComponent<Inventory>().reinforceMode)
    //     {
    //         if (item.Rank > 5)
    //         {
    //             // 강화 불가능한 등급 (ex. 9강, resource)
    //             return;
    //         }

    //         // 강화 모드인 경우
    //         int scrollType = cv.GetComponent<Inventory>().scrollType;
            
    //         if (scrollType == 15)
    //         {
    //             // 무기 강화
    //             if (!(itemId >= 0 && itemId <= 499))
    //             {
    //                 // 무기가 아닌 경우
    //                 return;
    //             }
    //             cv.GetComponent<Inventory>().Rush(inventoryIndex);
    //         }
    //         else if (scrollType == 16)
    //         {
    //             // 방어구 강화
    //             if (!(itemId >= 500 && itemId <= 1299))
    //             {
    //                 // 방어구가 아닌 경우
    //                 return;
    //             }
    //             cv.GetComponent<Inventory>().Rush(inventoryIndex);
    //         }
    //         else if (scrollType == 17)
    //         {
    //             // 장신구 강화
    //             if (!(itemId >= 1300 && itemId <= 1600))
    //             {
    //                 // 장신구가 아닌 경우
    //                 return;
    //             }
    //             cv.GetComponent<Inventory>().Rush(inventoryIndex);
    //         }
    //         else if (scrollType == 18)
    //         {
    //             // 소울스톤 강화
    //             if (!(itemId >= 1608 && itemId <= 1613))
    //             {
    //                 // 소울스톤이 아닌 경우
    //                 return;
    //             }

    //             // temp
    //         }

    //         return;
    //     }

    //     // 아이템 선택
    //     if (isSelected)
    //     {
    //         // UnSelect();
    //         cv.GetComponent<Inventory>().CloseItemDetail();
    //         HideInventoryBtn();     // 인벤토리 버튼 비활성화
    //     }
    //     else
    //     {
    //         cv.GetComponent<Inventory>().UpdateSelect(inventoryIndex);
    //         Select();
    //         cv.GetComponent<Inventory>().OpenItemDetail(item);
    //         SetInventoryBtn();      // 인벤토리 버튼 활성화
    //     }
    // }

    // // 장착
    // public void Equip()
    // {
    //     cv.GetComponent<Inventory>().EquipItemType(item.ItemType, inventoryIndex);

    //     image_EquipImage.gameObject.SetActive(true);
    //     cv.GetComponent<Inventory>().go_player.GetComponent<Stat>().Equip(item);
    //     cv.GetComponent<Inventory>().UpdateStatDetail();
    //     isEquip = true;

    //     SetInventoryBtn();
    // }



    // 선택
    // private void Select()
    // {
    //     isSelected = true;
    //     go_selectedFrame.SetActive(isSelected);
    // }

    // public void UnSelect()
    // {
    //     // Inventory.cs 에서 Unselect 할때 쓰임
    //     isSelected = false;
    //     go_selectedFrame.SetActive(isSelected);
        
    //     HideInventoryBtn();
    //     cv.GetComponent<Inventory>().UpdateSelect(-1);  // inventory selectedIndex -1로 초기화
    // }

    // private void SetInventoryBtn()
    // {
    //     string btnText = "";
    //     if (item.ItemType == 12)
    //     {
    //         // 제작 재료 아이템
    //         HideInventoryBtn();
    //         return;
    //     }

    //     if (item.ItemType < 12)
    //     {
    //         // 장착 아이템
    //         if (isEquip)
    //         {
    //             btnText = "해제";
    //         }
    //         else
    //         {
    //             btnText = "장착";
    //         }
    //     }
    //     else if (item.ItemType > 12)
    //     {
    //         // 사용 아이템
    //         btnText = "사용";
    //     }

    //     go_inventoryBtn.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = btnText;
    //     go_inventoryBtn.gameObject.SetActive(true);
    // }

    // private void HideInventoryBtn()
    // {
    //     // 장착, 사용 아이템이 아닌 경우
    //     go_inventoryBtn.gameObject.SetActive(false);
    // }
}
