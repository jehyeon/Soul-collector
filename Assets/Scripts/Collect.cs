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
    private Slot itemSlot;
    [SerializeField]
    private TextMeshProUGUI itemText;
    [SerializeField]
    private TextMeshProUGUI requireNumber;

    private int attackCollectionIndex;
    private int defenseCollectionIndex;

    private string[] attackCollectionType = {"기본 데미지", "치명타 확률", "공격속도"};
    private string[] attackCollectionSign = {"", "%", "%"};
    private string[] defenseCollectionType = {"데미지 감소", "HP 자동 회복", "최대 HP"};

    public void InitCollection(GameManager gm, int savedAttackCollectionIndex, int savedDefenseCollectionIndex)
    {
        gameManager = gm;

        // collection 진행도를 save로부터 가져옴
        attackCollectionIndex = savedAttackCollectionIndex;
        defenseCollectionIndex = savedDefenseCollectionIndex;

        attackCollections = attackCollectionList.GetComponentsInChildren<Collection>();
        defenseCollections = defenseCollectionList.GetComponentsInChildren<Collection>();

        // Load
        for (int index = 0; index < attackCollections.Length; index++)
        {
            attackCollections[index].Set(this, index);
            attackCollections[index].UpdateText(string.Format("{0} {1}{2}", 
                attackCollectionType[index % 3], index / 3 + 1, attackCollectionSign[index % 3]
            ));

            if (index <= attackCollectionIndex)
            {
                attackCollections[index].ActivateView();
            }
        }

        for (int index = 0; index < defenseCollections.Length; index++)
        {
            int offset = index % 3 == 2
                ? 10
                : 1;

            defenseCollections[index].Set(this, index);
            defenseCollections[index].UpdateText(string.Format("{0} {1}", 
                defenseCollectionType[index % 3], (index / 3 + 1) * offset
            ));

            if (index <= defenseCollectionIndex)
            {
                defenseCollections[index].ActivateView();
            }
        }
    }

    // Select
    public void Select(CollectionType type, int index)
    {
        if (type == CollectionType.Attack)
        {
            UpdateMaterialUI(attackCollections[index]);
        }
        else if (type == CollectionType.Defense)
        {
            UpdateMaterialUI(defenseCollections[index]);
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
        requireNumber.text = "1 / 1";
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
