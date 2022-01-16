using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Craft : MonoBehaviour
{
    // Inventory, Item Manager
    private Inventory inventory;
    private ItemManager itemManager;

    // Craft table
    private List<Dictionary<string, object>> data;

    [SerializeField]
    private GameObject go_craftItemList;
    [SerializeField]
    private GameObject go_craftMaterialsUI;
    [SerializeField]
    private GameObject go_craftMaterialList;
    // Prefabs
    [SerializeField]
    private GameObject pref_craftItem;
    [SerializeField]
    private GameObject pref_itemMaterial;

    private int selectCraftItemIndex;

    void Start()
    {
        selectCraftItemIndex = -1;

        inventory = GameObject.Find("Canvas").GetComponent<Inventory>();
        itemManager = GameObject.Find("Item Manager").GetComponent<ItemManager>();
        data = CSVReader.Read("Craft");
        
        for (int i = 0; i < data.Count; i++)
        {
            GameObject craftItem = Instantiate(pref_craftItem);
            craftItem.transform.SetParent(go_craftItemList.transform);

            craftItem.GetComponent<CraftItem>().Set(i, itemManager.Get((int)data[i]["itemId"]));
        }
    }

    public void SelectCraftItem(int index)
    {
        selectCraftItemIndex = index;
        OpenCreateMaterials();
        LoadCreateMaterials(selectCraftItemIndex);
    }

    public void UnSelectCraftItem()
    {
        if (selectCraftItemIndex == -1)
        {
            return;
        }
        
        go_craftItemList.transform.GetChild(selectCraftItemIndex).GetComponent<CraftItem>().UnSelect();
        selectCraftItemIndex = -1;
    }

    private void OpenCreateMaterials()
    {
        go_craftMaterialsUI.SetActive(true);
    }
    private void CloseCreateMaterialsUI()
    {
        go_craftMaterialsUI.SetActive(false);
        ClearCreateMaterials();
    }

    private void LoadCreateMaterials(int createItemIndex)
    {
        // 비우고
        ClearCreateMaterials();

        // 생성
        string materials = data[createItemIndex]["materialId"].ToString();
        foreach (string material in materials.Split('/'))
        {
            int materialItemId = Int32.Parse(material.Split('|')[0]);
            int numberOfmaterial = Int32.Parse(material.Split('|')[1]);

            GameObject craftMaterialItem = Instantiate(pref_itemMaterial);
            craftMaterialItem.transform.SetParent(go_craftMaterialList.transform);

            int amount = inventory.GetAmountItem(materialItemId);

            craftMaterialItem.GetComponent<CraftMaterial>().Set(itemManager.Get(materialItemId), amount, numberOfmaterial, amount > numberOfmaterial);
            
            // Get from inventory
        }

        int gold = (int)data[createItemIndex]["gold"];
        if (gold > 0)
        {
            GameObject craftMaterialItem = Instantiate(pref_itemMaterial);
            craftMaterialItem.transform.SetParent(go_craftMaterialList.transform);

            craftMaterialItem.GetComponent<CraftMaterial>().Set(itemManager.Get(1627), inventory.saveManager.save.gold, gold, inventory.saveManager.save.gold > gold);
        }
    }

    private void ClearCreateMaterials()
    {
        // Destroy
        Transform[] childList = go_craftMaterialList.GetComponentsInChildren<Transform>(true);
        if (childList != null)
        {
            for (int i = 1; i < childList.Length; i++)
            {
                Destroy(childList[i].gameObject);
            }
        }
    }

    public void CraftItem()
    {
        if (selectCraftItemIndex == -1)
        {
            return;
        }

        Debug.Log("제작");
    }
}
