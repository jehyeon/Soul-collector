using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class CraftMaterialSlot : Slot, IPointerEnterHandler, IPointerExitHandler
{
    private Craft craft;
    private CraftMaterial material;
    private int remain;

    // Text
    [SerializeField]
    private TextMeshProUGUI textItemName;
    [SerializeField]
    private TextMeshProUGUI textRequireNumberText;

    private int materialItemId;
    private int requireNumber;

    public void SetCraftMaterial(Craft myCraft, Item craftMaterialItem, CraftMaterial craftMaterial, int amount)
    {
        craft = myCraft;
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

    public void OnPointerEnter(PointerEventData eventData)
    {
        // 마우스 오버
        if (item == null)
        {
            // 아이템이 없는 경우 그냥 return
            return;
        }

        craft.ShowItemDetail(item, transform.position);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // 마우스 오버 아웃
        craft.CloseItemDetail();
    }
}
