using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class Slot : MonoBehaviour
{
    // 슬롯 별
    protected Item item;
    protected int itemCount;

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
    protected TextMeshProUGUI text_count;

    public Item Item { get { return item; } }
    public int Count { get { return itemCount; } }

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
        itemCount = count;

        // slot view 설정
        // Item icon, frame
        img_item.sprite = item.ItemImage;
        img_frame.sprite = item.ItemFrame;
        SetColor(1);

        // background, color
        img_background.color = item.BackgroundColor;

        if (text_count != null)
        {
            if (itemCount <= 1)
            {
                text_count.text = "";
            }
            else
            {
                text_count.text = itemCount.ToString();
            }
        }
    }

    // 아이템 수량 변경
    public void SetSlotCount(int count)
    {
        itemCount += count;
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
        if (text_count != null)
        {
            // slot에 item count가 없는 경우도 있음
            text_count.text = "";
            Color itemCountColor;
            ColorUtility.TryParseHtmlString("#FFFFFFFF", out itemCountColor);
            text_count.color = itemCountColor;
        }
    }
}