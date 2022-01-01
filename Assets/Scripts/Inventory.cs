using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    private bool inventoryActivated = false;
    private bool statDetailActivated = false;

    [SerializeField]
    private GameObject go_statDetail;
    [SerializeField] 
    private GameObject go_inventoryBase;
    [SerializeField]
    private GameObject go_SlotsParent;
    [SerializeField]
    private GameObject go_itemDetail;
    [SerializeField]
    private Text text_gold;

    [SerializeField]
    public GameObject go_player;

    public int[] equipped;  // 장착 중인 슬롯의 id

    private Slot[] slots;
    private int index;

    void Awake()
    {
        slots = go_SlotsParent.GetComponentsInChildren<Slot>();
        index = 0;

        equipped = new int[12];
        // 장착 정보를 -1로 초기화
        for (int i = 0; i < equipped.Length; i++)
        {
            equipped[i] = -1;
        }
    }

    void Start()
    {
        UpdateStatDetail();
    }
    void Update()
    {
        UseInventory();
        UseStatDetail();
    }

    private void UseInventory()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (inventoryActivated)
            {
                CloseInventory();
            } 
            else
            {
                OpenInventory();
            }
        }
    }

    private void UseStatDetail()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (statDetailActivated)
            {
                CloseStatDetail();
            } 
            else
            {
                OpenStatDetail();
            }
        }
    }
    
    // Stat UI
    private void OpenStatDetail()
    {
        go_statDetail.SetActive(true);
        statDetailActivated = true;
    }
    public void CloseStatDetail()
    {
        go_statDetail.SetActive(false);
        statDetailActivated = false;
    }
    public void UpdateStatDetail()
    {
        go_statDetail.transform.GetChild(1).GetComponent<Text>().text = go_player.GetComponent<Player>()._stat.ToString();
    }

    // Inventory UI
    private void OpenInventory()
    {
        go_inventoryBase.SetActive(true);
        inventoryActivated = true;
    }

    public void CloseInventory()
    {
        go_inventoryBase.SetActive(false);
        inventoryActivated = false;

        // 아이템 디테일 창도 닫음
        CloseItemDetail();
    }

    public bool AcquireItem(int _id, int _count = 1)
    {
        // 아이템 테이블로부터 정보를 가져옴
        Item addingItem = gameObject.AddComponent<Item>();
        addingItem.LoadFromCSV(_id, "Item");

        if (addingItem.ItemType > 12)
        {
            // resource인 경우
            for (int i = 0; i < index; i++)
            {
                if (slots[i].item != null && slots[i].item.Id == addingItem.Id)
                {
                    // 같은 resource의 아이템이 인벤토리에 이미 있는 경우
                    // 수량만 바꿈
                    slots[i].SetSlotCount(_count);
                    
                    return true;
                }
            }
        }

        // 인벤토리 꽉 찼는지 확인
        if (index >= slots.Length)
        {
            return false;
        }

        slots[index].AddItem(addingItem, _count);
        index += 1;
        return true;
    }

    public void OpenItemDetail(Item item, Image itemImage)
    {
        go_itemDetail.transform.GetChild(1).GetComponent<Image>().sprite = itemImage.sprite;
        go_itemDetail.transform.GetChild(2).GetComponent<Text>().text = item.ItemName;
        go_itemDetail.transform.GetChild(3).GetComponent<Text>().text = item.ToString();

        go_itemDetail.SetActive(true);
    }

    private void CloseItemDetail()
    {
        go_itemDetail.SetActive(false);
        go_itemDetail.transform.GetChild(1).GetComponent<Image>().sprite = null;
        go_itemDetail.transform.GetChild(2).GetComponent<Text>().text = "";
        go_itemDetail.transform.GetChild(3).GetComponent<Text>().text = "";
    }

    public void UnEquipItemType(int itempType)
    {
        // itemType의 장비를 UnEquip
        if (equipped[itempType] == -1)
        {
            return;
        }
        else
        {
            slots[equipped[itempType]].UnEquip();
            equipped[itempType] = -1;
        }
    }

    public void EquipItemType(int itemType, int slotId)
    {
        equipped[itemType] = slotId;
    }

    public void UpdateGoldText(int gold)
    {
        text_gold.text = gold.ToString() + " gold";
    }
}
