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
    private GameObject goCraftItemParent;
    [SerializeField]
    private GameObject goCraftMaterialsUI;
    [SerializeField]
    private GameObject goCraftMaterialParent;
    // Prefabs
    [SerializeField]
    private GameObject prefCraftItem;
    [SerializeField]
    private GameObject prefCraftMaterial;
    [SerializeField]
    private ObjectPool craftMaterialSlotOP;
    [SerializeField]
    private GameObject goCraftBtn;

    private int selectCraftItemIndex;
    private CraftItemSlot[] slots;
    private List<CraftMaterial> materials;      // 현재 선택한 제작 아이템 material 정보 (필요 개수, 가지고 있는 개수)

    // -------------------------------------------------------------
    // Init
    // -------------------------------------------------------------
    void Start()
    {
        selectCraftItemIndex = -1;
        myCraft = GetComponent<Craft>();
        CloseMaterialUI();
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
            craftItem.transform.SetParent(goCraftItemParent.transform);

            craftItem.GetComponent<CraftItemSlot>().SetCraftItem(
                myCraft, 
                i, 
                gameManager.ItemManager.Get((int)gameManager.CraftManager.data[i]["itemId"]),
                gameManager.CraftManager.data[i]["materialId"].ToString(),
                (int)gameManager.CraftManager.data[i]["gold"]
            );
        }

        slots = goCraftItemParent.GetComponentsInChildren<CraftItemSlot>();
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
        goCraftBtn.SetActive(true);
    }

    public void UnSelect()
    {
        // CraftItemSlot에서 호출
        selectCraftItemIndex = -1;
        CloseMaterialUI();
        goCraftBtn.SetActive(false);
    }

    public void UnSelectFromCraft()
    {
        if (selectCraftItemIndex != -1)
        {
            slots[selectCraftItemIndex].UnSelect();
        }
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
        materials = slots[craftItemIndex].Materials;
        foreach(CraftMaterial material in materials)
        {
            // 인벤토리로부터 해당 아이템의 수량을 가져옴
            int myCount = gameManager.Inventory.GetItemAmount(material.Id);
            material.SetCount(myCount);

            // Material Slot 설정
            GameObject craftMaterialSlot = craftMaterialSlotOP.Get();
            craftMaterialSlot.transform.SetParent(goCraftMaterialParent.transform);

            // !!! 느리면 GameObject OP -> CraftMaterialSlot OP로 수정하기
            craftMaterialSlot.GetComponent<CraftMaterialSlot>().SetCraftMaterial(
                myCraft,
                gameManager.ItemManager.Get(material.Id),
                material,
                myCount
            );
        }
    }

    private void ClearCreateMaterials()
    {
        // foreach(Transform child in goCraftMaterialParent.transform)
        int childCount = goCraftMaterialParent.transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            craftMaterialSlotOP.Return(goCraftMaterialParent.transform.GetChild(0).gameObject);
        }
    }

    // -------------------------------------------------------------
    // 아이템 제작
    // -------------------------------------------------------------
    public void ClickCraft()
    {
        // 제작 버튼 클릭 시
        if (gameManager.Inventory.isFullInventory())
        {
            // 무조건 하나 이상이 비워져 있어야 함
            gameManager.PopupMessage("인벤토리에 남은 공간이 없습니다.");
            return;
        }

        if (selectCraftItemIndex == -1)
        {
            // 선택한 아이템이 없는 경우 버튼이 활성화 되지도 않음
            return;
        }

        // notExist: 재료 아이템 수량이 하나라도 부족하면 true
        bool notExist = materials.Any(material => material.Exist == false);
        if (notExist)
        {
            gameManager.PopupMessage("재료 아이템이 부족합니다.");
            return;
        }

        // gameManager.PopupAsk("Craft", "아이템을 제작하시겠습니까?", "아니요", "네");
        int maxCount = slots[selectCraftItemIndex].Item.ItemType == ItemType.Use || slots[selectCraftItemIndex].Item.ItemType == ItemType.Material
            ? 999
            : gameManager.Inventory.GetRemainInventory();

        gameManager.PopupSetCount("Craft", "제작할 아이템의 수량을 입력해주세요,", "취소", "제작"
            , maxCount, 1);
    }

    public void CraftItem(int count = 1)
    {
        bool lastCheck = false;
        if (count > 1)
        {
            // count가 1개 이상인 경우 다시 수량 체크
            foreach (CraftMaterial material in materials)
            {
                if (material.Count < material.RequiredNumber * count)
                {
                    lastCheck = true;
                    break;
                }
            }
        }

        if (lastCheck)
        {
            gameManager.PopupMessage("재료 아이템이 부족합니다.");
            return;
        }

        // materials: 현재 선택된 제작 아이템 재료 정보
        foreach (CraftMaterial material in materials)
        {
            if (material.Id == 0)
            {
                // 재료가 gold인 경우
                gameManager.Inventory.UpdateGold(-1 * material.RequiredNumber * count);
            }
            else
            {
                // material id의 아이템을 material required number만큼 차감
                // 아이템의 존재 여부 및 수량이 충분한지 ClickCraft()에서 확인
                gameManager.Inventory.UpdateItemCountUsingItemId(material.Id, -1 * material.RequiredNumber * count, false);
            }
        }

        // 제작 아이템 추가
        for (int i = 0; i < count; i++)
        {
            gameManager.Inventory.AcquireItem(slots[selectCraftItemIndex].Item);
        }
        gameManager.Inventory.SaveAndLoad();

        // 제작 후 재료 창 reload, 사용한 재료 갱신
        ClearCreateMaterials();
        LoadCraftMeterials(selectCraftItemIndex);
    }

    // -------------------------------------------------------------
    // Item detail tooltip
    // -------------------------------------------------------------
    public void ShowItemDetail(Item item, Vector3 pos)
    {
        gameManager.UIController.OpenItemDetail(item, pos);
    }

    public void CloseItemDetail()
    {
        gameManager.UIController.CloseItemDetail();
    }

    // -------------------------------------------------------------
    // UI gameObject (use on UIController.cs)
    // -------------------------------------------------------------
    public void Open()
    {
        this.gameObject.SetActive(true);
    }

    public void Close()
    {
        UnSelectFromCraft();
        this.gameObject.SetActive(false);
    }
}
