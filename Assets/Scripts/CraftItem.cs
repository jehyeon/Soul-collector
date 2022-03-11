using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class CraftItem : MonoBehaviour, IPointerClickHandler
{
    public int craftItemId;

    [SerializeField]
    private GameObject go_craft;

    [SerializeField]
    private Image itemImage;
    [SerializeField]
    private Image slotFrame;
    [SerializeField]
    private Image slotBackground;
    [SerializeField]
    private TextMeshProUGUI text_itemName;
    [SerializeField]
    private GameObject go_selectedFrame;

    private bool isSelected;
    private int craftItemIndex;

    public void Set(int index, Item item)
    {
        go_craft = this.transform.parent.parent.parent.gameObject;
        // item parameter의 값으로 수정
        craftItemIndex = index;
        craftItemId = item.Id;
        itemImage.sprite = item.ItemImage;
        slotFrame.sprite = item.ItemFrame;
        slotBackground.color = item.BackgroundColor;

        text_itemName.text = item.ItemName;
        text_itemName.color = item.FontColor;
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

    private void Select()
    {
        isSelected = true;
        go_selectedFrame.SetActive(isSelected);
        // go_craft.GetComponent<Craft>().SelectCraftItem(craftItemIndex);
    }

    public void UnSelect()
    {
        isSelected = false;
        go_selectedFrame.SetActive(isSelected);
    }
}
