using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;
// using LitJson;

public class Inventory : MonoBehaviour
{
    private GameManager gameManager;
    
    // 캐릭터 체력 및 Gold
    [SerializeField]
    private TextMeshProUGUI text_gold;      // Text Gold
    [SerializeField]
    private GameObject go_slotParent;

    // 인벤토리 Slot, 골드, Save
    private InventorySlot[] slots;
    private int selectedSlotIndex;      // 선택된 index
    public int scrollSlotIndex;     // 주문서가 저장된 slot index
    public int scrollType;          // scrollSlotIndex의 주문서가 어떤 주문서 타입인지
    public bool reinforceMode;

    void Awake()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        // save로부터 초기화가 안되는 field 초기화
        slots = go_slotParent.GetComponentsInChildren<InventorySlot>();
        selectedSlotIndex = -1;
        reinforceMode = false;
        scrollSlotIndex = -1;
        scrollType = -1;

        // dropManager = gameObject.AddComponent<DropManager>();
    }

    void Start()
    {
        LoadInventory();
    }
    

    
    // ------------------------

    // Item detail UI
    // !!! Need to move
    // public void OpenItemDetail(Item item)
    // {
    //     img_itemDetailImage.sprite = item.ItemImage;
    //     img_itemDetailFrame.sprite = item.ItemFrame;
    //     img_itemDetailBackground.color = item.BackgroundColor;
    //     text_itemDetailName.text = item.ItemName;
    //     text_itemDetailName.color = item.FontColor;
    //     text_itemDetailDes.text = item.ToString();

    //     go_itemDetail.SetActive(true);
    // }

    // public void CloseItemDetail()
    // {
    //     go_itemDetail.SetActive(false);

    //     if (selectedSlotIndex != -1)
    //     {
    //         // 선택한 슬롯이 있는 경우 UnSelect()
    //         slots[selectedSlotIndex].UnSelect();
    //         // selectedSlotIndex는 -1로 초기화 됨
    //     }
    // }
    // // -------------------------

    // 인벤토리 꽉 찼는지 확인
    public bool isFullInventory()
    {
        if (gameManager.SaveManager.Save.LastSlotIndex >= gameManager.SaveManager.Save.InventorySize - 1)
        {
            return true;
        }
        
        return false;
    }

    // 아이템 획득
    public void AcquireItem(Item item, int count = 1)
    {
        // 인벤토리 Full이면 해당 함수 호출 안됨
        
        if (item.ItemType == ItemType.Material || item.ItemType == ItemType.Use)
        {
            // 중첩 가능한 아이템만 확인
            // Inventory slot save 데이터에 동일한 아이템이 있는지 확인
            for (int i = 0; i < gameManager.SaveManager.Save.LastSlotIndex; i++)
            {
                if (gameManager.SaveManager.Save.Slots[i].Id == item.Id)
                {
                    // 수량만 변경
                    slots[i].SetSlotCount(count);  // only view
                    gameManager.SaveManager.Save.Slots[i].UpdateCount(count);
                }
            }
        }
        else
        {
            slots[gameManager.SaveManager.Save.LastSlotIndex].Set(item, count);     // only view
            gameManager.SaveManager.Save.AddSlot(item.Id, count);
        }
    }

    // UnEquip
//     cv.GetComponent<Inventory>().UnEquipItemType(item.ItemType);    // 해당 파츠 아이템을 UnEquip
    //     cv.GetComponent<Inventory>().go_player.GetComponent<Stat>().UnEquip(item);
    //     cv.GetComponent<Inventory>().UpdateStatDetail();
    //      slot.UnEquip()  // 그냥 equip표시만 없앰
    //     SetInventoryBtn();


    // public void UnEquipItemType(int itemType)
    // {
    //     saveManager.Save.Equipped[itemType] = -1;
    //     saveManager.SaveData();
    // }

    // public void EquipItemType(int itemType, int slotId)
    // {
    //     if (saveManager.Save.Equipped[itemType] != -1)
    //     {
    //         // 해당 파츠를 착용하고 있으면 먼저 UnEquip
    //         slots[saveManager.Save.Equipped[itemType]].UnEquip();
    //     }

    //     saveManager.Save.Equipped[itemType] = slotId;
    //     saveManager.SaveData();
    // }

    // public void UpdateGold(int droppedGold)
    // {
    //     saveManager.Save.Gold += droppedGold;
    //     text_gold.text = saveManager.Save.GoldText;
    //     saveManager.SaveData();
    // }

    private void LoadInventory()
    {
        // SaveManager slot과 동기화

        // inventory clear 후 아이템 재생성
        for (int i = 0; i < gameManager.SaveManager.Save.LastSlotIndex; i++)
        {   
            // 인벤토리 크기만큼 한번만 loop
            slots[i].ClearSlot();
            Item loaded = gameManager.ItemManager.Get(gameManager.SaveManager.Save.Slots[i].Id);
            slots[i].Set(loaded, gameManager.SaveManager.Save.Slots[i].Count);
        }

        // 장착 정보 로드
    }

    // public void UpdateSelect(int slotIndex)
    // {
    //     if (slotIndex != -1 && selectedSlotIndex != -1)
    //     {
    //         // 전에 장착한 아이템이 있는 경우
    //         slots[selectedSlotIndex].UnSelect();
    //     }
    //     selectedSlotIndex = slotIndex;
    // }

    // public void ClickInventoryBtn()
    // {
    //     if (selectedSlotIndex == -1)
    //     {
    //         return;
    //     }

    //     if (slots[selectedSlotIndex].Item.ItemType == 12)
    //     {
    //         // 장비, 사용 아이템이 아닌 경우
    //         return;
    //     }

    //     if (slots[selectedSlotIndex].Item.ItemType < 12)
    //     {
    //         // selectedSlotIndex 의 slot이 선택 되었을 때
    //         if (slots[selectedSlotIndex].isEquip)
    //         {
    //             slots[selectedSlotIndex].UnEquip();
    //         }
    //         else
    //         {
    //             slots[selectedSlotIndex].Equip();
    //         }
    //     }
    //     else if (slots[selectedSlotIndex].Item.ItemType > 12)
    //     {
    //         if (index > 59 && slots[selectedSlotIndex].Item.ItemType == 13)
    //         {
    //             // 상자 아이템인 경우 인벤토리가 꽉찰 때 사용 안됨
    //             return;
    //         }
    //         Use(slots[selectedSlotIndex].ItemId, slots[selectedSlotIndex].Item.ItemType);
    //     }

    //     return;
    // }

    // public void Buy(int itemId, int price)
    // {
    //     if (gold < price)
    //     {
    //         // 돈이 부족하다는 pop_up 띄우기
    //         return;
    //     }


    //     // 우선 한개만 구입가능함
    //     // 인벤토리 초과 시  false return
    //     bool result = AcquireItem(itemId, 1);
    //     if (result)
    //     {
    //         // 정상 구매인 경우에만 돈 차감
    //         UpdateGold(-1 * price);
    //     }
    // }

    // public void Delete(int slotIndex = -1)
    // {
    //     // save의 배열이 바뀌게 되므로 Reload()를 해줘야 함
    //     // All slot clear -> Load()

    //     // Select slot item 삭제, 개수 선택 미구현
    //     if (slotIndex == -1)
    //     {
    //         // 파라미터 없이 호출되었을 때에는 selectedSlotIndex를 삭제

    //         if (selectedSlotIndex == -1)
    //         {
    //             // 선택한 Slot이 없는 경우 삭제 안됨
    //             return;    
    //         }

    //         slotIndex = selectedSlotIndex;
    //     }       

    //     // 아이템 삭제 후 save
    //     saveManager.Delete(slotIndex);
    //     saveManager.Save();

    //     // 선택한 ItemDetail 닫음
    //     CloseItemDetail();

    //     LoadInventory();
    // }

    // public void Use(int itemId, int itemType)
    // {
    //     // 13 - 뽑기 아이템, 14 - 체력 회복 아이템, 15 - 강화 아이템
    //     if (itemType == 14)
    //     {
    //         // 우선 테이블을 쓰지 않음
    //         if (itemId == 1620)
    //         {
    //             go_player.GetComponent<Stat>().Heal(50);
    //             slots[selectedSlotIndex].SetSlotCount(-1);
    //             if (slots[selectedSlotIndex].ItemCount < 1)
    //             {
    //                 Delete();
    //             }
    //         }
    //         else if (itemId == 1621)
    //         {
    //             go_player.GetComponent<Stat>().Heal(200);
    //             slots[selectedSlotIndex].SetSlotCount(-1);
    //             if (slots[selectedSlotIndex].ItemCount < 1)
    //             {
    //                 Delete();
    //             }
    //         }
    //     }
    //     else if (itemType == 13)
    //     {
    //         if (itemId == 1623)
    //         {
    //             // 방어구 상자
    //             GambleBox(1);
    //         }
    //         else if (itemId == 1625)
    //         {
    //             // 무기 상자
    //             GambleBox(2);
    //         }
    //     }
    //     else if (itemType >= 15 && itemType <= 18)
    //     {
    //         Reinforce(itemType);
    //     }
    // }

    // public void GambleBox(int dropId)
    // {
    //     slots[selectedSlotIndex].SetSlotCount(-1);
    //     if (slots[selectedSlotIndex].ItemCount < 1)
    //     {
    //         Delete();
    //     }
    //     int itemId = dropManager.RandomItem(dropId);
    //     AcquireItem(itemId);    // 추가 후 저장
    // }

    // public void Reinforce(int _scrollType)
    // {
    //     // 강화 모드
    //     reinforceMode = true;
    //     scrollSlotIndex = selectedSlotIndex;    // 마지막에 선택한 슬롯의 index
    //     scrollType = _scrollType;

    //     // UI 모두 닫고, unselect
    //     OpenReinforceUI();
    //     CloseItemDetail();
    //     CloseShop();
    //     CloseStatDetail();
    // }

    // public void OpenReinforceUI()
    // {
    //     go_reinforce.SetActive(true);
    // }
    // public void CloseReinforceUI()
    // {
    //     go_reinforce.SetActive(false);
    //     reinforceMode = false;
    //     scrollSlotIndex = -1;
    //     scrollType = -1;
    // }

    // public void Rush(int slotIndex)
    // {
    //     if (scrollSlotIndex == -1)
    //     {
    //         // Error
    //         return;
    //     }

    //     slots[scrollSlotIndex].SetSlotCount(-1);
    //     if (slots[scrollSlotIndex].ItemCount < 1) 
    //     {
    //         // scroll이 없어지면서 slotIndex가 바뀔 수도 있음
    //         int beforeSlotId = saveManager.save.slots[slotIndex].slotId;

    //         Delete(scrollSlotIndex);
    //         CloseReinforceUI();       // 스크롤 인덱스가 초기화

    //         slotIndex = FindItemUsingSlotId(beforeSlotId);  // slotIndex 업데이트
    //     }
    //     else
    //     {
    //         // 스크롤 갯수가 1 이상 남아있는 경우 save 업데이트
    //         saveManager.UpdateCount(scrollSlotIndex, -1);
    //     }

    //     float rand = Random.value;
    //     float percent = 0f;
    //     if (slots[slotIndex].upgradeLevel % 2 == 0)
    //     {
    //         percent = .5f;
    //     }
    //     else
    //     {
    //         percent = .33f;
    //     }

    //     if (rand < percent)
    //     {
    //         // 강화 성공
    //         slots[slotIndex].Upgrade();
    //         saveManager.save.slots[slotIndex].id += 1;
    //         saveManager.Save();

    //         LoadInventory();
    //     }
    //     else
    //     {
    //         // 강화 실패
    //         int beforeScrollSlotId = -1;
    //         if (scrollSlotIndex != -1)
    //         {
    //             // 강화 아이템이 없어지면서 scrollSlotIndex가 바뀔 수도 있음
    //             beforeScrollSlotId = saveManager.save.slots[scrollSlotIndex].slotId;
    //         }

    //         Delete(slotIndex);

    //         scrollSlotIndex = FindItemUsingSlotId(beforeScrollSlotId);  // scrollSlotIndex 업데이트
    //     }
    // }

    // public void SetItemCount(int slotIndex, int count)
    // {
    //     slots[slotIndex].SetSlotCount(count);
    //     if (slots[slotIndex].ItemCount < 1) 
    //     {
    //         Delete(slotIndex);
    //     }
    //     else
    //     {
    //         Save();
    //     }
    // }

    // public int FindItemUsingItemId(int itemId)
    // {
    //     if (itemId == -1)
    //     {
    //         return -1;
    //     }

    //     for (int i = 0; i < index; i++)
    //     {
    //         if (slots[i].ItemId == itemId)
    //         {
    //             return i;
    //         }
    //     }

    //     return -1;
    // }

    // private int FindItemUsingSlotId(int slotId)
    // {
    //     if (slotId == -1)
    //     {
    //         return -1;
    //     }

    //     for (int i = 0; i < index; i++)
    //     {
    //         if (saveManager.save.slots[i].slotId == slotId)
    //         {
    //             return i;
    //         }
    //     }

    //     return -1;
    // }

    // public int GetAmountItem(int itemId)
    // {
    //     for (int i = 0; i < saveManager.save.slotIndex; i++)
    //     {
    //         if (saveManager.save.slots[i].id == itemId)
    //         {
    //             return saveManager.save.slots[i].count;
    //         }
    //     }
        
    //     return 0;
    // }

    // public bool CheckEnoughInventory()
    // {
    //     if (index >= slots.Length)
    //     {
    //         return false;
    //     }
        
    //     return true;
    // }
}
