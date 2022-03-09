using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class UIController: MonoBehaviour
{
    // on the canvas
    [SerializeField]
    private GameObject go_statUI;
    [SerializeField]
    private TextMeshProUGUI textStat;

    [SerializeField]
    private Inventory inventory;
    [SerializeField]
    private Shop shop;
    [SerializeField]
    private GameObject go_craftUI;
    [SerializeField]
    private GameObject go_reinforceUI;

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
                CloseStatUI();
            }
            else
            {
                OpenStatUI();
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

        if (Input.GetKeyDown(KeyCode.S))
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
    public void OpenStatUI()
    {
        go_statUI.SetActive(true);
        isActivatedStatUI = true;
    }

    public void CloseStatUI()
    {
        go_statUI.SetActive(false);
        isActivatedStatUI = false;
    }

    public void UpdateStatUI(Stat characterStat)
    {
        // 게임 시작, 아이템 장착, 해제
        textStat.text = characterStat.ToString();
    }

    // Inventory UI
    public void OpenInventoryUI()
    {
        inventory.Open();
        isActivatedInventoryUI = true;
    }

    public void CloseInventoryUI()
    {
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
    
}