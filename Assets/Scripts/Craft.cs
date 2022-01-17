using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

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
    private List<CheckMaterial> checkMaterials;

    void Start()
    {
        selectCraftItemIndex = -1;
        checkMaterials = new List<CheckMaterial>();

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

            int amount = inventory.GetAmountItem(materialItemId);

            // Material game object 생성
            GameObject craftMaterialItem = Instantiate(pref_itemMaterial);
            craftMaterialItem.transform.SetParent(go_craftMaterialList.transform);
            craftMaterialItem.GetComponent<CraftMaterial>().Set(itemManager.Get(materialItemId), amount, numberOfmaterial, amount > numberOfmaterial);

            // materials 정보를 저장
            checkMaterials.Add(new CraftMaterial(materialItemId, amount, amount > numberOfmaterial));
        }

        int gold = (int)data[createItemIndex]["gold"];
        if (gold > 0)
        {
            GameObject craftMaterialItem = Instantiate(pref_itemMaterial);
            craftMaterialItem.transform.SetParent(go_craftMaterialList.transform);
            craftMaterialItem.GetComponent<CraftMaterial>().Set(itemManager.Get(1627), inventory.saveManager.save.gold, gold, inventory.saveManager.save.gold > gold);

            checkMaterials.Add(new CraftMaterial(1627, inventory.saveManager.save.gold, inventory.saveManager.save.gold > gold));
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

        // material 정보 초기화
        checkMaterials = new List<CheckMaterial>();
    }

    public void CraftItem()
    {
        if (selectCraftItemIndex == -1)
        {
            // 선택한 아이템이 없는 경우
            return;
        }

        if (checkMaterials.Count == 0)
        {
            // material 정보가 없는 경우
            return;
        }

        List<bool> existList = checkMaterials.Select(material => material._exists);
        if (existList.Find(exist => exist == false) < 0)
        {
            // 재료가 하나라도 부족한 경우
            return;
        }

        foreach (CheckMaterial material in checkMaterials)
        {
            int slotIndex = inventory.FindItemUsingItemId(material.Id);
            if (slotIndex == -1)
            {
                // 수량 변동이 생긴 경우
                return;
            }

            inventory.SetItemCount(slotIndex, -1 * meterial.Count);
        }

        // Add Item
    }
}
