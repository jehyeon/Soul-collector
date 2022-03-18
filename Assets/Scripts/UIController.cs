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
    private Shop shop;                  // 상점
    [SerializeField]
    private GameObject go_craftUI;
    [SerializeField]
    private GameObject go_reinforceUI;
    [SerializeField]
    private PopupMessage popupMessage;
    [SerializeField]
    private PopupAsk popupAsk;

    private bool isActivatedStatUI;
    private bool isActivatedInventoryUI;
    private bool isActivatedItemUI;
    private bool isActivatedShopUI;
    private bool isActivatedCraftUI;
    private bool isActivatedReinforceUI;

    // 플레이어 체력바
    [SerializeField]
    private Slider hpBar;

    [SerializeField]
    private TextMeshProUGUI hpBarText;      // Text Hp

    private void Awake()
    {
        // cv = gameObject.transform.GetChild();
        isActivatedStatUI = false;
        isActivatedInventoryUI = false;
        isActivatedItemUI = false;
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
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (isActivatedStatUI)
            {
                CloseEquipmentUI();
            }
            else
            {
                OpenEquipmentUI();
            }
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            if (isActivatedInventoryUI)
            {
                CloseInventoryUI();
            }
            else
            {
                OpenInventoryUI();
            }
        }

        if (Input.GetKeyDown(KeyCode.H))
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

    // Stat UI
    public void OpenEquipmentUI()
    {
        equiment.Open();
        isActivatedStatUI = true;
    }

    public void CloseEquipmentUI()
    {
        equiment.Close();
        isActivatedStatUI = false;
    }

    public void UpdateStatUI(Stat characterStat)
    {
        // equiment.UpdateStatText(characterStat);
    }

    // Inventory UI
    public void OpenInventoryUI()
    {
        background.SetActive(true);
        inventory.Open();
        isActivatedInventoryUI = true;
    }

    public void CloseInventoryUI()
    {
        background.SetActive(false);
        inventory.Close();
        isActivatedInventoryUI = false;

        // Item detail 및 강화 UI도 닫음
        CloseReinforceUI();
    }

    // Shop UI
    public void OpenShopUI()
    {
        shop.Open();
        isActivatedShopUI = true;
        OpenInventoryUI();      // 상점 페이지 오픈 시 인벤토리도 오픈
        inventory.CloseReinforceUI();   // 상점 페이지 오픈 시 강화 모드 취소
    }

    public void CloseShopUI()
    {
        shop.Close();
        isActivatedShopUI = false;
    }

    // Craft UI
    public void OpenCraftUI()
    {
        go_craftUI.SetActive(true);
        isActivatedCraftUI = true;
    }

    public void CloseCraftUI()
    {
        go_craftUI.SetActive(false);
        isActivatedCraftUI = false;
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