using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Push : MonoBehaviour
{
    private Push myPush;
    private int selectedPushItemIndex;
    private PushItemSlot[] slots;

    [SerializeField]
    private GameManager gameManager;

    [SerializeField]
    private GameObject goPushPref;
    [SerializeField]
    private GameObject goPushDetail;
    [SerializeField]
    private TextMeshProUGUI textPushDetailMessage;
    [SerializeField]
    private Slot pushDetailSlot;
    [SerializeField]
    private GameObject goPushList;

    // -------------------------------------------------------------
    // Push 아이템 로드
    // -------------------------------------------------------------
    public void LoadPushList(PushForAPI pushes)
    {
        // AuctionItemForAPI에 대한 정의는 ApiManager.cs에 있음

        if (myPush == null)
        {
            myPush = GetComponent<Push>();
        }

        int pushItemList = 0;
        foreach(PushItem item in pushes.result)
        {
            GameObject pushItem = Instantiate(goPushPref);    // !!! Object pool로 수정하기
            pushItem.transform.SetParent(goPushList.transform);
            pushItem.GetComponent<PushItemSlot>().SetPushItem(
                myPush,
                pushItemList,
                gameManager.ItemManager.Get(item.itemId),
                item.message
            );
            
            pushItemList += 1;
        }

        slots = goPushList.GetComponentsInChildren<PushItemSlot>();
    }

    // -------------------------------------------------------------
    // Select
    // -------------------------------------------------------------
    public void Select(int slotIndex)
    {
        if (selectedPushItemIndex != -1)
        {
            // 이미 선택된 다른 슬롯이 있는 경우
            slots[selectedPushItemIndex].UnSelect();
        }
        selectedPushItemIndex = slotIndex;
        OpenPushDetail(slots[selectedPushItemIndex].Item, slots[selectedPushItemIndex].Message);
        // goBuyBtn.SetActive(true);
    }

    public void UnSelect()
    {
        slots[selectedPushItemIndex].UnSelect();
        selectedPushItemIndex = -1;
        // goBuyBtn.SetActive(false);
    }

    // -------------------------------------------------------------
    // Push UI
    // -------------------------------------------------------------
    public void Open()
    {
        this.gameObject.SetActive(true);
        // !!! 푸시 오픈 시 gameManager를 통해 아이템 리스트를 요청
        // 이후 gameManager에서 Push.LoadPushList를 호출
        gameManager.RequestPushList();
    }

    public void Close()
    {
        this.gameObject.SetActive(false);
    }

    private void OpenPushDetail(Item pushItem, string message)
    {
        goPushDetail.SetActive(true);
        textPushDetailMessage.text = message;
        pushDetailSlot.Set(pushItem);
    }

    private void ClosePushDetail()
    {
        goPushDetail.SetActive(false);
    }

    // Item detail tooltip
    public void ShowItemDetail(Item item, Vector3 pos)
    {
        gameManager.UIController.OpenItemDetail(item, pos);
    }

    public void CloseItemDetail()
    {
        gameManager.UIController.CloseItemDetail();
    }
}
