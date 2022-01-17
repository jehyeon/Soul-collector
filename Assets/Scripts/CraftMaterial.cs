using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class CraftMaterial : MonoBehaviour
{
    [SerializeField]
    private Image itemImage;
    [SerializeField]
    private Image slotFrame;
    [SerializeField]
    private Image slotBackground;
    [SerializeField]
    private TextMeshProUGUI text_itemName;
    [SerializeField]
    private TextMeshProUGUI text_requireNumberText;

    private int materialItemId;
    private int requireNumber;

    public void Set(Item item, int haved, int require, bool ready=true)
    {
        requireNumber = require;

        // item parameter의 값으로 수정
        // craftItemIndex = index;
        materialItemId = item.Id;
        itemImage.sprite = item.ItemImage;
        slotFrame.sprite = item.ItemFrame;
        slotBackground.color = item.BackgroundColor;

        text_itemName.text = item.ItemName;
        text_itemName.color = item.FontColor;

        if (!ready)
        {
            // 아이템이 부족한 경우 수량 text 색상 변경
            Color fontColor;
            ColorUtility.TryParseHtmlString("#F85858FF", out fontColor);
            text_requireNumberText.color = fontColor;
        }

        string text1 = string.Format("{0:#,###}", haved).ToString();
        if (text1 == "")
        {
            text1 = "0";
        }

        string text2 = string.Format("{0:#,###}", require).ToString();
        if (text2 == "")
        {
            text2 = "0";
        }

        text_requireNumberText.text = text1 + " / " + text2;
    }
}
