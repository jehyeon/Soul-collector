using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class PushItemSlot : Slot, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    private Push push;
    private int index;
    private bool isSelected;

    private PushItem pushItem;
    public PushItem PushItem { get { return pushItem; } }

    [SerializeField]
    private GameObject go_selectedFrame;
    [SerializeField]
    private TextMeshProUGUI goPushMessage;

    // -------------------------------------------------------------
    // Init
    // -------------------------------------------------------------
    public void SetPushItem(Push parentPush, int pushItemIndex, Item itemFromItemManager, PushItem fromServer)
    {
        push = parentPush;
        Set(itemFromItemManager);

        index = pushItemIndex;
        pushItem = fromServer;
        goPushMessage.text = pushItem.message;
    }

    // -------------------------------------------------------------
    // Select
    // -------------------------------------------------------------
    private void Select()
    {
        isSelected = true;
        go_selectedFrame.SetActive(true);
    }

    public void UnSelect()
    {
        isSelected = false;
        go_selectedFrame.SetActive(false);
    }
    
    // -------------------------------------------------------------
    // Push 아이템 슬롯 마우스 이벤트
    // -------------------------------------------------------------
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (isSelected)
            {
                push.UnSelect();
                UnSelect();
            }
            else
            {
                // 기존 선택된 아이템을 unselect
                push.Select(index);
                Select();
            }
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

        push.ShowItemDetail(item, transform.position);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // 마우스 오버 아웃
        push.CloseItemDetail();
    }
}
