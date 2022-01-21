using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
    }

    public void CloseShopUI()
    {
        go_shopUI.SetActive(false);
        isActivatedShopUI = false;
    }

    // Craft UI
    public void OpenCraftUI()
    {
        go_craftUI.SetActive(true);
        isActivatedCraftUI = true;
    }

    public void OpenCraftUI()
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
}