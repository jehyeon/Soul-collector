using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class CraftItemSlot : Slot, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    private Craft craft;

    [SerializeField]
    private GameObject goSelectedFrame;
    [SerializeField]
    private TextMeshProUGUI textCraftItemName;

    private int index;
    private bool isSelected;
    private List<CraftMaterial> materials;
    public List<CraftMaterial> Materials { get { return materials; } }

    public void SetCraftItem(Craft parentCraft, int craftItemIndex, Item craftItem, string craftMaterials, int gold = 0)
    {
        // Set Craft Item
        craft = parentCraft;
        index = craftItemIndex;
        textCraftItemName.text = craftItem.ItemName;
        textCraftItemName.color = craftItem.FontColor;
        Set(craftItem);

        // Set Craft Meterials
        materials = new List<CraftMaterial>();
        foreach (string materialInfo in craftMaterials.Split('/'))
        {
            materials.Add(new CraftMaterial(materialInfo.Split('|')));
        }
        if (gold != 0)
        {
            materials.Add(new CraftMaterial(gold));
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // 기존 선택된 아이템을 unselect
        // go_craft.GetComponent<Craft>().UnSelectCraftItem();

        if (isSelected)
        {
            UnSelect();
        }
        else
        {
            Select();
        }
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

    private void Select()
    {
        isSelected = true;
        goSelectedFrame.SetActive(true);

        craft.SelectCraftItem(index);
    }

    public void UnSelect()
    {
        isSelected = false;
        goSelectedFrame.SetActive(false);

        craft.UnSelect();
    }
}
