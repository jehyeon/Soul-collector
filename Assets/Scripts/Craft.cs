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

    private bool isClick;
    private float clickedTime;
    private float craftingTime;

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

    // private void Update()
    // {
    //     if (isClick)
    //     {
    //         clickedTime += Time.deltaTime;

    //         if (clickedTime > 1.5f)
    //         {
    //             // 1.5초 이상 누르고 있으면 0.1초마다 아이템 구입
    //             craftingTime += Time.deltaTime;
    //             if (craftingTime > .1f)
    //             {
    //                 CraftItem();
    //                 craftingTime = 0f;
    //             }
    //         }
    //     }
    // }

    // public void SelectCraftItem(int index)
    // {
    //     selectCraftItemIndex = index;
    //     OpenCreateMaterials();
    //     LoadCreateMaterials(selectCraftItemIndex);
    // }

    // public void UnSelectCraftItem()
    // {
    //     if (selectCraftItemIndex == -1)
    //     {
    //         return;
    //     }
        
    //     go_craftItemList.transform.GetChild(selectCraftItemIndex).GetComponent<CraftItem>().UnSelect();
    //     selectCraftItemIndex = -1;
    // }

    // private void OpenCreateMaterials()
    // {
    //     go_craftMaterialsUI.SetActive(true);
    // }
    // private void CloseCreateMaterialsUI()
    // {
    //     go_craftMaterialsUI.SetActive(false);
    //     ClearCreateMaterials();
    // }

    // private void LoadCreateMaterials(int createItemIndex)
    // {
    //     // 비우고
    //     ClearCreateMaterials();

    //     // 생성
    //     string materials = data[createItemIndex]["materialId"].ToString();
    //     foreach (string material in materials.Split('/'))
    //     {
    //         int materialItemId = Int32.Parse(material.Split('|')[0]);
    //         int numberOfmaterial = Int32.Parse(material.Split('|')[1]);

    //         int amount = inventory.GetAmountItem(materialItemId);

    //         // Material game object 생성
    //         GameObject craftMaterialItem = Instantiate(pref_itemMaterial);
    //         craftMaterialItem.transform.SetParent(go_craftMaterialList.transform);
    //         craftMaterialItem.GetComponent<CraftMaterial>().Set(itemManager.Get(materialItemId), amount, numberOfmaterial, amount > numberOfmaterial);

    //         // materials 정보를 저장
    //         checkMaterials.Add(new CheckMaterial(materialItemId, numberOfmaterial, amount > numberOfmaterial));
    //     }
    //     int gold = (int)data[createItemIndex]["gold"];
    //     if (gold > 0)
    //     {
    //         GameObject craftMaterialItem = Instantiate(pref_itemMaterial);
    //         craftMaterialItem.transform.SetParent(go_craftMaterialList.transform);
    //         craftMaterialItem.GetComponent<CraftMaterial>().Set(itemManager.Get(1627), inventory.saveManager.Save.Gold, gold, inventory.saveManager.Save.Gold > gold);

    //         checkMaterials.Add(new CheckMaterial(1627, gold, inventory.saveManager.Save.Gold > gold));
    //     }
    // }

    // private void ClearCreateMaterials()
    // {
    //     // Destroy
    //     Transform[] childList = go_craftMaterialList.GetComponentsInChildren<Transform>(true);
    //     if (childList != null)
    //     {
    //         for (int i = 1; i < childList.Length; i++)
    //         {
    //             Destroy(childList[i].gameObject);
    //         }
    //     }

    //     // material 정보 초기화
    //     checkMaterials = new List<CheckMaterial>();
    // }

    // public void CraftItem()
    // {
    //     if (!inventory.CheckEnoughInventory())
    //     {
    //         // 인벤토리에 남은 칸이 있는지 확인
    //         // 인벤토리가 꽉 찼을 때, 동일한 리소스 아이템이 인벤토리에 있어도 제작이 안됨 (P2)
    //         return;
    //     }

    //     if (selectCraftItemIndex == -1)
    //     {
    //         // 선택한 아이템이 없는 경우
    //         return;
    //     }

    //     if (checkMaterials.Count == 0)
    //     {
    //         // material 정보가 없는 경우
    //         return;
    //     }

    //     var existList = checkMaterials.Select(material => material.Exist);
    //     // if (existList.Find(exist => exist == false) < 0)
    //     foreach (bool exist in existList)
    //     {
    //         if (exist == false)
    //         {
    //             // 재료가 하나라도 부족한 경우
    //             return;
    //         }
    //     }

    //     foreach (CheckMaterial material in checkMaterials)
    //     {
    //         if (material.Id == 1627)
    //         {
    //             // 재료가 gold인 경우
    //             inventory.UpdateGold(-1 * material.Count);
    //             // inventory.Save();
    //         }
    //         else
    //         {
    //             // 재료 id가 인벤토리에 있으면 수량 제거
    //             int slotIndex = inventory.FindItemUsingItemId(material.Id);
    //             if (slotIndex == -1)
    //             {
    //                 // 수량 변동이 생긴 경우
    //                 return;
    //             }
    //             inventory.SetItemCount(slotIndex, -1 * material.Count);
    //         }
    //     }
    //     // 현재 아이템만 차감됨 Add Item
    //     inventory.AcquireItem((int)data[selectCraftItemIndex]["itemId"]);

    //     // 제작 후 재료 창 reload, 사용한 재료 갱신
    //     LoadCreateMaterials(selectCraftItemIndex);
    // }

    // public void ButtonDown()
    // {
    //     isClick = true;
    // }

    // public void ButtonUp()
    // {
    //     isClick = false;
    //     clickedTime = 0f;
    //     craftingTime = 0f;
    // }    
}
