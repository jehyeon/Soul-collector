using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class CraftMaterialSlot : Slot
{
    private CraftMaterial material;
    private int remain;

    // Text
    [SerializeField]
    private TextMeshProUGUI textItemName;
    [SerializeField]
    private TextMeshProUGUI textRequireNumberText;

    private int materialItemId;
    private int requireNumber;

    public void SetCraftMaterial(Item craftMaterialItem, CraftMaterial craftMaterial, int amount)
    {
        Set(craftMaterialItem);
        textItemName.text = item.ItemName;
        textItemName.color = item.FontColor;

        material = craftMaterial;
        remain = amount;

        Color fontColor;
        if (material.RequiredNumber > remain)
        {
            // 아이템이 부족한 경우
            // Red
            ColorUtility.TryParseHtmlString("#F85858FF", out fontColor);
            textRequireNumberText.color = fontColor;
        }
        else
        {
            // Default White
            ColorUtility.TryParseHtmlString("#FFFFFFFF", out fontColor);
            textRequireNumberText.color = fontColor;
        }

        string text1 = string.Format("{0:#,###}", remain).ToString();
        if (text1 == "")
        {
            text1 = "0";
        }

        string text2 = string.Format("{0:#,###}", material.RequiredNumber).ToString();
        if (text2 == "")
        {
            text2 = "0";
        }

        textRequireNumberText.text = text1 + " / " + text2;
    }
}
