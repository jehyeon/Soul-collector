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
        inventory = GameObject.Find("Canvas").GetComponent<Inventory>();
        itemManager = GameObject.Find("Item Manager").GetComponent<ItemManager>();
        data = CSVReader.Read("Craft");
        
        for (int i = 0; i < data.Count; i++)
        {
            GameObject craftItem = Instantiate(pref_craftItem);
            craftItem.transform.SetParent(go_craftItemList.transform);

            craftItem.GetComponent<CraftItem>().Set(i, itemManager.Get((int)data[i]["itemId"]));

            LoadCreateMaterials(i);
        }
    }

    public void SelectCraftItem(int index)
    {
        selectCraftItemIndex = index;
    }

    public void UnSelectCraftItem()
    {
        Debug.Log(this.transform.GetChild(2).transform.GetChild(0).transform);
        Debug.Log(this.transform.GetChild(2).transform.GetChild(0).transform.childCount);
        Debug.Log(go_craftItemList.transform);
        Debug.Log(go_craftItemList.transform.childCount);
        // this.transform.GetChild(selectCraftItemIndex).GetComponent<CraftItem>().UnSelect();
    }

    private void OpenCreateMaterials()
    {
        go_craftMaterialsUI.SetActive(true);
    }
    private void CloseCreateMaterialsUI()
    {
        go_craftMaterialsUI.SetActive(false);
    }

    private void LoadCreateMaterials(int createItemIndex)
    {
        string materials = data[createItemIndex]["materialId"].ToString();
        foreach (string material in materials.Split('/'))
        {
            int materialItemId = Int32.Parse(material.Split('|')[0]);
            int numberOfmaterial = Int32.Parse(material.Split('|')[1]);

            Debug.Log(materialItemId);
            Debug.Log(numberOfmaterial);
        }
        // Instantiate
    }

    private void ClearCreateMaterials()
    {
        // Destroy
    }

    public void CraftItem()
    {
        Debug.Log("제작");
    }
}
