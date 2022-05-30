using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Collect : MonoBehaviour
{
    private GameManager gameManager;

    [SerializeField]
    private GameObject attackCollectionList;
    
    [SerializeField]
    private GameObject defenseCollectionList;

    private Collection[] attackCollections;
    private Collection[] defenseCollections;

    // Material UI
    [SerializeField]
    private GameObject materialGameObject;
    [SerializeField]
    private TextMeshProUGUI statText;
    [SerializeField]
    private GameObject activateBtn;
    [SerializeField]
    private GameObject alreadyActivated;
    [SerializeField]
    private GameObject notBeforeCollectionActivated;
    [SerializeField]
    private GameObject materialItemSlot;
    [SerializeField]
    private Slot itemSlot;
    [SerializeField]
    private TextMeshProUGUI itemText;
    [SerializeField]
    private TextMeshProUGUI requireNumber;

    private Stat totalStat;
    private int selectedIndex = -1;
    private CollectionType selectedType;

    private string[] attackCollectionType = {"기본 데미지", "치명타 확률", "공격속도"};
    private string[] attackCollectionSign = {"", "%", "%"};
    private string[] defenseCollectionType = {"데미지 감소", "HP 자동 회복", "최대 HP"};

    public void InitCollection(GameManager gm)
    {
        gameManager = gm;

        attackCollections = attackCollectionList.GetComponentsInChildren<Collection>();
        defenseCollections = defenseCollectionList.GetComponentsInChildren<Collection>();

        // Load
        for (int index = 0; index < attackCollections.Length; index++)
        {
            attackCollections[index].Set(this, CollectionType.Attack, index);
            attackCollections[index].UpdateText(string.Format("{0} {1}{2}", 
                attackCollectionType[index % 3], index / 3 + 1, attackCollectionSign[index % 3]
            ));

            if (index <= gameManager.SaveManager.Save.AttackCollection)
            {
                attackCollections[index].ActivateView();
            }
        }

        for (int index = 0; index < defenseCollections.Length; index++)
        {
            int offset = index % 3 == 2
                ? 10
                : 1;

            defenseCollections[index].Set(this, CollectionType.Defense, index);
            defenseCollections[index].UpdateText(string.Format("{0} {1}", 
                defenseCollectionType[index % 3], (index / 3 + 1) * offset
            ));

            if (index <= gameManager.SaveManager.Save.DefenseCollection)
            {
                defenseCollections[index].ActivateView();
            }
        }

        if (gameManager.SaveManager.Save.AttackCollection != -1 && gameManager.SaveManager.Save.DefenseCollection != -1)
        {
            ApproveCollectionStat();
        }
    }

    public void ApproveCollectionStat()
    {
        // 스탯 초기화
        totalStat = new Stat(true);

        for (int index = 0; index <= gameManager.SaveManager.Save.AttackCollection; index++)
        {
            totalStat.SumForCollect(index % 3, index / 3 + 1);
        }

        for (int index = 0; index <= gameManager.SaveManager.Save.DefenseCollection; index++)
        {
            int offset = index % 3 == 2
                ? 10
                : 1;
            totalStat.SumForCollect(index % 3 + 3, (index / 3 + 1) * offset);
        }

        // 컬렉션 버프 업데이트
        gameManager.AddCollectionBuff(totalStat);
    }

    // Select
    public void Select(CollectionType _type, int index)
    {
        if (selectedIndex != -1)
        {
            // 그냥 둘다 unselect
            attackCollections[selectedIndex].UnSelect();
            defenseCollections[selectedIndex].UnSelect();
        }

        selectedIndex = index;
        selectedType = _type;

        if (selectedType == CollectionType.Attack)
        {
            UpdateMaterialUI(attackCollections[selectedIndex]);
        }
        else if (selectedType == CollectionType.Defense)
        {
            UpdateMaterialUI(defenseCollections[selectedIndex]);
        }
    }

    // Material UI
    private void UpdateMaterialUI(Collection collection)
    {
        materialGameObject.SetActive(true);
        statText.text = collection.StatText;
        Item item = gameManager.ItemManager.Get(9);
        itemSlot.Set(item);
        itemText.text = item.ItemName;

        if (collection.Activated)
        {
            // 이미 활성화된 컬렉션
            materialItemSlot.SetActive(false);
            notBeforeCollectionActivated.SetActive(false);
            alreadyActivated.SetActive(true);
            // 등록 버튼 비활성화
            activateBtn.SetActive(false);
        }
        else
        {
            Collection[] selectedCollections = selectedType == CollectionType.Attack
                ? attackCollections
                : defenseCollections;

            if (collection.Index > 0 && !selectedCollections[collection.Index - 1].Activated)
            {
                // 이전 단계 컬렉션이 등록되지 않은 경우
                alreadyActivated.SetActive(false);
                materialItemSlot.SetActive(false);
                notBeforeCollectionActivated.SetActive(true);

                // 등록 버튼 비활성화
                activateBtn.SetActive(false);
            }
            else
            {
                alreadyActivated.SetActive(false);
                notBeforeCollectionActivated.SetActive(false);
                materialItemSlot.SetActive(true);
                // 비활성화 컬렉션
                Color fontColor;
                int remainItemAmount = gameManager.Inventory.GetItemAmount(9);
                requireNumber.text = string.Format("{0} / {1}", remainItemAmount, collection.Index + 1);
                if (remainItemAmount > collection.Index)
                {
                    ColorUtility.TryParseHtmlString("#B1ABB2FF", out fontColor);
                    requireNumber.color = fontColor;
                }
                else
                {
                    ColorUtility.TryParseHtmlString("#F85858FF", out fontColor);
                    requireNumber.color = fontColor;
                }

                // 등록 버튼 활성화
                activateBtn.SetActive(true);
            }
        }
    }

    public void ClickRegister()
    {
        if (selectedIndex == -1)
        {
            return;
        }

        Collection collection = selectedType == CollectionType.Attack
            ? attackCollections[selectedIndex]
            : defenseCollections[selectedIndex];
        
        int requireNumber = collection.Index + 1;

        if (gameManager.Inventory.GetItemAmount(9) < requireNumber)
        {
            gameManager.PopupMessage("컬렉션 등록에 필요한 아이템이 부족합니다.");
            return;
        }

        gameManager.PopupAsk("Collect", "컬렉션에 등록하시겠습니까?", "아니요", "네");
    }

    public void RegisterCollection()
    {
        if (selectedIndex == -1)
        {
            return;
        }

        Collection collection = selectedType == CollectionType.Attack
            ? attackCollections[selectedIndex]
            : defenseCollections[selectedIndex];
        
        int requireNumber = collection.Index + 1;

        collection.ActivateView();

        if (collection.Type == CollectionType.Attack)
        {
            gameManager.SaveManager.Save.AttackCollection += 1;
        }
        else
        {
            gameManager.SaveManager.Save.DefenseCollection += 1;
        }

        gameManager.Inventory.UpdateItemCountUsingItemId(9, -1 * requireNumber);
        gameManager.SaveManager.SaveData();
        UpdateMaterialUI(collection);

        ApproveCollectionStat();
    }

    // Collect UI
    public void Open()
    {
        this.gameObject.SetActive(true);
    }

    public void Close()
    {
        this.gameObject.SetActive(false);
    }
}
