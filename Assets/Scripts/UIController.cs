using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class UIController: MonoBehaviour
{
    // UI 조작 관리
    [SerializeField]
    private GameObject background;      // 배경
    [SerializeField]
    private Inventory inventory;        // 인벤토리
    [SerializeField]
    private Equipment equiment;         // 장착 정보
    [SerializeField]
    private ItemDetail itemDetail;      // 아이템 툴팁
    [SerializeField]
    private Shop shop;                  // 상점
    [SerializeField]
    private Craft craft;                // 아이템 제작
    [SerializeField]
    private GameObject go_reinforceUI;
    [SerializeField]
    private PopupMessage popupMessage;
    [SerializeField]
    private PopupAsk popupAsk;

    private bool isActivatedInventoryUI;
    private bool isActivatedEquipmentUI;
    private bool isActivatedShopUI;
    private bool isActivatedCraftUI;
    private bool isActivatedReinforceUI;

    // 플레이어 체력바
    [SerializeField]
    private Slider hpBar;

    [SerializeField]
    private TextMeshProUGUI hpBarText;      // Text Hp

    public ItemDetail ItemDetail { get { return itemDetail; } }

    private void Awake()
    {
        isActivatedInventoryUI = false;
        isActivatedEquipmentUI = false;
        isActivatedShopUI = false;
        isActivatedCraftUI = false;
        isActivatedReinforceUI = false;
    }
    
    private void Update()
    {
        KeyBoardAction();
    }

    private void KeyBoardAction()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            // Inventory & Equipment
            if (isActivatedEquipmentUI)
            {
                CloseEquipmentUI();
            }
            else
            {
                OpenEquipmentUI();
            }
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            if (isActivatedShopUI)
            {
                CloseShopUI();
            }
            else
            {
                OpenShopUI();
            }
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            if (isActivatedCraftUI)
            {
                CloseCraftUI();
            }
            else
            {
                OpenCraftUI();
            }
        }
    }

    public void UpdateStatUI(Stat characterStat)
    {
        equiment.UpdateStatText(characterStat);
    }

    // Inventory UI + background
    public void OpenInventoryUI(string type = "default")
    {
        background.SetActive(true);
        inventory.Open(type);
    }

    public void CloseInventoryUI()
    {
        background.SetActive(false);
        inventory.Close();
        itemDetail.Close();
    }

    // Equipment UI + Inventory UI
    public void OpenEquipmentUI()
    {
        isActivatedEquipmentUI = true;
        OpenInventoryUI();
        equiment.Open();
    }

    public void CloseEquipmentUI()
    {
        isActivatedEquipmentUI = false;
        equiment.Close();
        CloseInventoryUI();
    }

    // Shop UI + Inventory UI
    public void OpenShopUI()
    {
        isActivatedShopUI = true;
        OpenInventoryUI("Shop");
        shop.Open();
    }

    public void CloseShopUI()
    {
        isActivatedShopUI = false;
        shop.Close();
        CloseInventoryUI();
    }

    // Craft UI
    public void OpenCraftUI()
    {
        isActivatedCraftUI = true;
        OpenInventoryUI("Craft");
        craft.Open();
    }

    public void CloseCraftUI()
    {
        isActivatedCraftUI = false;
        craft.Close();
        CloseInventoryUI();
    }

    // Reinforce UI
    public void OpenReinforceUI()
    {
        inventory.OpenReinforceUI();
        isActivatedReinforceUI = true;
    }

    public void CloseReinforceUI()
    {
        inventory.CloseReinforceUI();
        isActivatedReinforceUI = false;
    }

    // 플레이어 체력바
    public void InitPlayerHpBar(int maxHp)
    {
        hpBar.value = 1f;
        hpBarText.text = string.Format("{0} / {0}", maxHp);
    }
    public void UpdatePlayerHpBar(int nowHp, int maxHp)
    {
        hpBar.value = (float)nowHp / (float)maxHp;
        hpBarText.text = string.Format("{0} / {1}", nowHp, maxHp);
    }

    // -------------------------------------------------------------
    // 팝업 메시지
    // -------------------------------------------------------------
    public void PopupMessage(string message, float time = 1f)
    {
        popupMessage.Popup(message, time);
    }
    public void PopupAsk(string type, string ask, string leftText, string rightText)
    {
        popupAsk.Popup(type, ask, leftText, rightText);
    }
}