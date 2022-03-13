using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Equipment : MonoBehaviour
{
    private int selectedSlotIndex;

    [SerializeField]
    private GameManager gameManager;

    // 장착 Slot 프리팹
    [SerializeField]
    private GameObject goEquipmentSlotPref;
    [SerializeField]
    private GameObject goSlotParent;

    // 버튼
    [SerializeField]
    private GameObject goUnEquipBtn;

    // 스탯 정보
    [SerializeField]
    private TextMeshProUGUI goAttakStatText;
    [SerializeField]
    private TextMeshProUGUI goDefenseStatText;

    private EquipmentSlot[] slots;

    // -------------------------------------------------------------
    // Init
    // -------------------------------------------------------------
    private void Start()
    {
        selectedSlotIndex = -1;
    }

    public void InitEquipmentSlots()
    {
        // 장비 슬롯 12개 생성
        for (int i = 0; i < 12; i++)
        {
            GameObject equipmentSlot = Instantiate(goEquipmentSlotPref);
            equipmentSlot.transform.SetParent(goSlotParent.transform);
            equipmentSlot.GetComponent<EquipmentSlot>().Init(i, this.gameObject.GetComponent<Equipment>());
        }

        // 생성된 슬롯을 slots에 저장
        slots = goSlotParent.GetComponentsInChildren<EquipmentSlot>();
    }

    // -------------------------------------------------------------
    // 아이템 장착
    // -------------------------------------------------------------
    public void EquipItem(Item item)
    {
        if (slots[item.PartNum].Item != null)
        {
            // 아이템이 있는 경우
            // 슬롯 view 업데이트는 EquipmentSlot에서 처리
            gameManager.UnEquip(slots[item.PartNum].Item);
        }

        // 장착 번호에 맞게 장착
        slots[item.PartNum].Equip(item);
    }

    public void UnEquipItem()
    {
        if (selectedSlotIndex == -1)
        {
            // 선택된 장비가 없는 경우
            return;
        }

        gameManager.UnEquip(slots[selectedSlotIndex].Item);
        slots[selectedSlotIndex].UnEquip();
        UnSelectSlot();
    }

    public void UnEquipItem(Item item)
    {
        slots[item.PartNum].UnEquip();
    }
    
    // -------------------------------------------------------------
    // Slot 선택 (장착 슬롯에서 호출)
    // -------------------------------------------------------------
    public void SelectSlot(int slotIndex)
    {
        if (selectedSlotIndex != -1)
        {
            slots[selectedSlotIndex].UnSelect();    // 기존에 선택된 슬롯 선택 해제
        }

        selectedSlotIndex = slotIndex;
        goUnEquipBtn.SetActive(true);
    }

    public void UnSelectSlot()
    {
        slots[selectedSlotIndex].UnSelect();
        selectedSlotIndex = -1;
        goUnEquipBtn.SetActive(false);
    }

    // -------------------------------------------------------------
    // 스탯 UI 업데이트
    // -------------------------------------------------------------
    public void UpdateStatText(Stat stat)
    {
        // 임시
        goAttakStatText.text = stat.ToString();
        // goDefenseStatText = stat.ToString();
    }

    // -------------------------------------------------------------
    // UI gameObject (use on UIController.cs)
    // -------------------------------------------------------------
    public void Open()
    {
        this.gameObject.SetActive(true);
    }

    public void Close()
    {
        this.gameObject.SetActive(false);
        goUnEquipBtn.SetActive(false);
        selectedSlotIndex = -1;
    }    
}
