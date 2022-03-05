using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class UIController: MonoBehaviour
{
    // on the canvas
    [SerializeField]
    private Canvas cv;
    [SerializeField]
    private GameObject go_statUI;
    [SerializeField]
    private GameObject go_inventoryUI;
    [SerializeField]
    private GameObject go_itemUI;
    [SerializeField]
    private GameObject go_shopUI;
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

    // Inventory UI
    public void OpenInventoryUI()
    {
        go_inventoryUI.SetActive(true);
        isActivatedInventoryUI = true;
    }

    public void CloseInventoryUI()
    {
        go_inventoryUI.SetActive(false);
        isActivatedInventoryUI = false;

        // Item detail 및 강화 UI도 닫음
        CloseItemUI();
        CloseReinforceUI();
    }

    // Item UI
    public void OpenItemUI()
    {
        go_itemUI.SetActive(true);
        isActivatedItemUI = true;
    }

    public void CloseItemUI()
    {
        go_itemUI.SetActive(false);
        isActivatedItemUI = false;
    }

    // Shop UI
    public void OpenShopUI()
    {
        go_shopUI.SetActive(true);
        isActivatedShopUI = true;
        OpenInventoryUI();
    }

    public void CloseShopUI()
    {
        go_shopUI.SetActive(false);
        isActivatedShopUI = false;
        CloseInventoryUI();
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
        go_reinforceUI.SetActive(true);
        isActivatedReinforceUI = true;
    }

    public void CloseReinforceUI()
    {
        go_reinforceUI.SetActive(false);
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