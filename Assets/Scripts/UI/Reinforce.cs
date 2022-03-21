using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum ScrollType
{
    Weapon,
    Armor,
    SoulStone,
    None
}

public class Reinforce : MonoBehaviour
{
    [SerializeField]
    private GameManager gameManager;            // 게임 매니저
    [SerializeField]
    private GameObject goReinforceBtn;          // 강화 버튼
    [SerializeField]
    private GameObject goClearBtn;              // 강화 아이템 모두 비우기 버튼
    [SerializeField]
    private GameObject goSlotParent;            // Slot 상위 오브젝트
    [SerializeField]
    private GameObject goSlotPref;              // Slot prefab
    [SerializeField]
    private ObjectPool reinforceSlotOP;         // Slot Object pool
    [SerializeField]
    private ReinforceSlot scroll;               // Scroll Slot

    private Reinforce myReinforce;              // 자기 자신 Instance
    private ScrollType selectedScrollType;      // 선택된 Scroll type
    private int reinforcingItemCount;           // 강화하려는 아이템 수 (Clear되었는 지 확인하는 용도)
    private List<ReinforceSlot> slots;
    public ScrollType ScrollType { get { return selectedScrollType; } }

    // -------------------------------------------------------------
    // Init
    // -------------------------------------------------------------
    private void Awake()
    {
        selectedScrollType = ScrollType.None;
        myReinforce = GetComponent<Reinforce>();
    }

    private void Start()
    {
        // 강화 슬롯 생성
        for (int i = 0; i < 20; i++)
        {
            GameObject slot = reinforceSlotOP.Get();
            slot.transform.SetParent(goSlotParent.transform);
        }

        slots = goSlotParent.GetComponentsInChildren<ReinforceSlot>().ToList();
        reinforcingItemCount = 0;
    }

    // -------------------------------------------------------------
    // 아이템 강화
    // -------------------------------------------------------------
    public void ClickReinforce()
    {
        if (scroll.Count < reinforcingItemCount)
        {
            // 스크롤 개수가 강화하려는 아이템보다 적은 경우
            gameManager.PopupMessage("강화 주문서가 부족합니다.");
            return;
        }

        gameManager.PopupAsk("Reinforce", "아이템을 강화하시겠습니까?", "아니요", "네");
    }

    public void ReinforceItems()
    {
        List<int> success = new List<int>();

        // 강화 주문서 수량 조절
        // save 및 inventory view
        gameManager.Inventory.UpdateScrollItem(scroll.InventorySlotId, reinforcingItemCount);
        // reinforce view
        if (scroll.Count > reinforcingItemCount)
        {
            scroll.SetSlotCount(-1 * reinforcingItemCount);
            success.Add(scroll.InventorySlotId); 
        }
        else
        {
            scroll.Clear();
        }

        // foreach(ReinforceSlot slot in slots)
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].Item == null)
            {
                break;
            }

            // 강화 확률, 확률 수정 시 아래 percent 코드 수정이 필요함
            float percent = slots[i].Item.Level % 2 == 0 
                ? .5f
                : .33f;

            // 아이템 강화 시도
            if (Random.value < percent)
            {
                // 강화 성공
                Item nextItem = gameManager.ItemManager.Get(slots[i].Item.Id + 1);
                slots[i].Upgrade(nextItem);
                gameManager.Inventory.SuccessReinforce(slots[i].InventorySlotId, nextItem);
                success.Add(slots[i].InventorySlotId);
            }
            else
            {
                // 강화 실패
                gameManager.Inventory.FailReinforce(slots[i].InventorySlotId);
                Remove(slots[i]);
                i--;
            }
        }

        // 저장 후 리로드
        IsClearedReinforcingItems();
        gameManager.Inventory.DoneReinforce(success);
    }

    // -------------------------------------------------------------
    // Set Scroll
    // -------------------------------------------------------------
    public void SetScrollType(ScrollType scrollType)
    {
        selectedScrollType = scrollType;
    }

    // -------------------------------------------------------------
    // 슬롯 추가, 삭제
    // -------------------------------------------------------------
    public bool Add(Item item, int inventorySlotId, int count = 1)
    {
        Debug.Log(item.Id);
        Debug.Log(inventorySlotId);
        if (CheckSrollItem(item))
        {
            // 추가하려는 아이템이 주문서인 경우
            scroll.SetReinforceSlot(myReinforce, item, inventorySlotId, count);
            IsClearedReinforcingItems();
            return true;
        }

        foreach (ReinforceSlot slot in slots)
        {
            // 마지막 Reinforce slot에 아이템 추가
            if (slot.Item == null)
            {
                if (myReinforce == null)
                {
                    myReinforce = GetComponent<Reinforce>();
                }
                slot.SetReinforceSlot(myReinforce, item, inventorySlotId);
                reinforcingItemCount += 1;
                IsClearedReinforcingItems();
                return true;
            }
        }

        // 강화칸이 꽉 참
        gameManager.PopupMessage("강화 슬롯 공간이 부족합니다.");
        return false;
    }

    public void Remove(ReinforceSlot slot, int inventorySlotId = -1)
    {
        // Reinforce slot에서 unselect하는 경우

        if (CheckSrollItem(slot.Item))
        {
            // 삭제하려는 아이템이 주문서인 경우
            gameManager.Inventory.UnSelectUsingSlotId(scroll.InventorySlotId);
            scroll.Clear();
            IsClearedReinforcingItems();
            return;
        }

        if (inventorySlotId != -1)
        {
            // slotId가 넘어오는 경우만
            // gameManager를 통해 inventory unselect
            gameManager.Inventory.UnSelectUsingSlotId(inventorySlotId);
        }

        // 기존 슬롯 Return 후 새로 Get
        reinforceSlotOP.Return(slot.gameObject);
        slot.Clear();
        slots.Remove(slot);
        GameObject newSlot = reinforceSlotOP.Get();
        newSlot.transform.SetParent(goSlotParent.transform);
        slots.Add(newSlot.GetComponent<ReinforceSlot>());

        reinforcingItemCount -= 1;
        if (inventorySlotId != -1)
        {
            IsClearedReinforcingItems();
        }
    }

    public void RemoveFromInventory(int slotId, int itemId)
    {
        if (CheckSrollItem(itemId))
        {
            scroll.Clear();
            IsClearedReinforcingItems();
            return;
        }

        // Inventory slot에서 unselect하는 경우
        foreach(ReinforceSlot slot in slots)
        {
            if (slot.InventorySlotId == slotId)
            {
                slot.Clear();
                reinforceSlotOP.Return(slot.gameObject);  // 슬롯 Return
                slots.Remove(slot);
                break;
            }
        }

        // 새 슬롯 Get
        GameObject newSlot = reinforceSlotOP.Get();
        newSlot.transform.SetParent(goSlotParent.transform);
        slots.Add(newSlot.GetComponent<ReinforceSlot>());

        reinforcingItemCount -= 1;
        IsClearedReinforcingItems();
    }
    
    private bool CheckSrollItem(Item item)
    {
        // 스크롤 아이템인지 확인
        if (item.Id == 1615 || item.Id == 1616 || item.Id == 1617 || item.Id == 1618)
        {
            return true;
        }

        return false;
    }

    private bool CheckSrollItem(int itemId)
    {
        if (itemId == 1615 || itemId == 1616 || itemId == 1617 || itemId == 1618)
        {
            return true;
        }

        return false;
    }

    public void ClearSlots()
    {
        // 강화 중인 아이템 목록 초기화
        if (scroll.Item != null)
        {
            gameManager.Inventory.UnSelectUsingSlotId(scroll.InventorySlotId);
            scroll.Clear();
        }

        foreach (ReinforceSlot slot in slots)
        {
            if (slot.Item == null)
            {
                break;
            }

            gameManager.Inventory.UnSelectUsingSlotId(slot.InventorySlotId);
            slot.Clear();
        }

        reinforcingItemCount = 0;
        IsClearedReinforcingItems();
    }

    private void IsClearedReinforcingItems()
    {
        if (reinforcingItemCount == 0 && scroll.Item == null)
        {
            // 강화 대기 중인 아이템이 없고 등록된 scroll이 없는 경우
            selectedScrollType = ScrollType.None;
            goReinforceBtn.SetActive(false);
            goClearBtn.SetActive(false);
        }
        else
        {
            // reinforce slot에 아이템이 하나라도 있는 경우 모두 비우기 버튼 활성화
            goClearBtn.SetActive(true);
        }

        if (reinforcingItemCount > 0 && scroll.Item != null)
        {
            // 주문서와 아이템이 모두 있는 경우에만 강화 버튼 활성화
            goReinforceBtn.SetActive(true);
        }
    }
    // -------------------------------------------------------------
    // UI Control
    // -------------------------------------------------------------
    public void Open()
    {
        this.gameObject.SetActive(true);
    }

    public void Close()
    {
        ClearSlots();
        this.gameObject.SetActive(false);
    }

    // Item detail tooltip
    public void ShowItemDetail(Item item, Vector3 pos)
    {
        gameManager.UIController.ItemDetail.Open(item, pos);
    }

    public void CloseItemDetail()
    {
        gameManager.UIController.ItemDetail.Close();
    }
}