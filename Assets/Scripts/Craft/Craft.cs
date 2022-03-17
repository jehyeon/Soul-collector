using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Craft : MonoBehaviour
{
    [SerializeField]
    private GameManager gameManager;
    private Craft myCraft;

    [SerializeField]
    private GameObject goCraftItemList;
    [SerializeField]
    private GameObject goCraftMaterialsUI;
    [SerializeField]
    private GameObject goCraftMaterialList;
    // Prefabs
    [SerializeField]
    private GameObject prefCraftItem;
    [SerializeField]
    private GameObject prefCraftMaterial;
    [SerializeField]
    private ObjectPool craftMaterialSlotOP;

    private int selectCraftItemIndex;
    private CraftItemSlot[] slots;


    // -------------------------------------------------------------
    // Init
    // -------------------------------------------------------------
    void Start()
    {
        selectCraftItemIndex = -1;
        myCraft = GetComponent<Craft>();
    }

    public void InitCraftItemSlots()
    {
        if (myCraft == null)
        {
            myCraft = GetComponent<Craft>();
        }

        // 제작 아이템 리스트 가져오기
        for (int i = 0; i < gameManager.CraftManager.data.Count; i++)
        {
            // 오브젝트 풀링 적용 안함
            GameObject craftItem = Instantiate(prefCraftItem);
            craftItem.transform.SetParent(goCraftItemList.transform);

            craftItem.GetComponent<CraftItemSlot>().SetCraftItem(
                myCraft, 
                i, 
                gameManager.ItemManager.Get((int)gameManager.CraftManager.data[i]["itemId"]),
                gameManager.CraftManager.data[i]["materialId"].ToString(),
                (int)gameManager.CraftManager.data[i]["gold"]
            );
        }

        slots = goCraftItemList.GetComponentsInChildren<CraftItemSlot>();
    }

    // -------------------------------------------------------------
    // Select
    // -------------------------------------------------------------
    public void SelectCraftItem(int craftItemIndex)
    {
        if (selectCraftItemIndex != -1)
        {
            slots[selectCraftItemIndex].UnSelect();
        }
        
        selectCraftItemIndex = craftItemIndex;
        OpenMaterialUI();
    }

    public void UnSelect()
    {
        // CraftItemSlot에서 호출
        selectCraftItemIndex = -1;
        CloseMaterialUI();
    }

    // -------------------------------------------------------------
    // Material UI
    // -------------------------------------------------------------
    public void OpenMaterialUI()
    {
        // 선택된 craft item의 material을 load
        goCraftMaterialsUI.SetActive(true);
        LoadCraftMeterials(selectCraftItemIndex);
    }

    public void CloseMaterialUI()
    {
        goCraftMaterialsUI.SetActive(false);
        ClearCreateMaterials();
    }

    private void LoadCraftMeterials(int craftItemIndex)
    {
        foreach(CraftMaterial material in slots[craftItemIndex].Materials)
        {
            GameObject craftMaterialSlot = craftMaterialSlotOP.Get();
            craftMaterialSlot.transform.SetParent(goCraftMaterialList.transform);
            // !!! 느리면 GameObject OP -> CraftMaterialSlot OP로 수정하기
            craftMaterialSlot.GetComponent<CraftMaterialSlot>().SetCraftMaterial(
                gameManager.ItemManager.Get(material.Id),
                material,
                gameManager.Inventory.GetItemAmount(material.Id)
            );
        }
    }

    private void ClearCreateMaterials()
    {
        // foreach(Transform child in goCraftMaterialList.transform)
        int childCount = goCraftMaterialList.transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            craftMaterialSlotOP.Return(goCraftMaterialList.transform.GetChild(0).gameObject);
        }
    }

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
}
