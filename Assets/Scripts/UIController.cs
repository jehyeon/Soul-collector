using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class UIController: MonoBehaviour
{
    // UI 조작 관리
    [SerializeField]
    private GameObject background;      // 배경
    [SerializeField]
    private Inventory inventory;        // 인벤토리
    [SerializeField]
    private Equipment equiment;         // 장착 정보
    [SerializeField]
    private Reinforce reinforce;        // 장비 강화
    [SerializeField]
    private ItemDetail itemDetail;      // 아이템 툴팁
    [SerializeField]
    private Shop shop;                  // 상점
    [SerializeField]
    private Craft craft;                // 아이템 제작
    [SerializeField]
    private Auction auction;            // 경매장
    [SerializeField]
    private Push push;                  // 푸시
    [SerializeField]
    private QuickSlotSystem quickSlot;  // 퀵슬롯

    // 팝업 메시지
    [SerializeField]
    private PopupMessage popupMessage;
    [SerializeField]
    private PopupAsk popupAsk;
    [SerializeField]
    private PopupSetCount popupSetCount;

    // 몬스터 체력바 및 데미지 표기
    [SerializeField]
    private GameObject damageTextParent;
    [SerializeField]
    private GameObject enemyHpBarParent;

    // 버프
    [SerializeField]
    private GameObject buffParent;

    // Sound
    private UIEffectSound sound;

    // for dungeon
    [SerializeField]
    private GameObject floorFrame;
    [SerializeField]
    private TextMeshProUGUI floorText;

    private bool isActivatedInventoryUI;
    private bool isActivatedEquipmentUI;
    private bool isActivatedShopUI;
    private bool isActivatedCraftUI;
    private bool isActivatedReinforceUI;
    private bool isActivatedAuctionUI;
    private bool isActivatedPushUI;

    // 플레이어 체력바
    [SerializeField]
    private Slider hpBar;

    [SerializeField]
    private TextMeshProUGUI hpBarText;      // Text Hp

    public ItemDetail ItemDetail { get { return itemDetail; } }
    public GameObject DamageTextParent { get { return damageTextParent; } }
    public GameObject EnemyHpBarParent {  get { return enemyHpBarParent; } }
    public GameObject BuffParent { get { return buffParent; } }

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        isActivatedInventoryUI = false;
        isActivatedEquipmentUI = false;
        isActivatedShopUI = false;
        isActivatedCraftUI = false;
        isActivatedReinforceUI = false;
        isActivatedAuctionUI = false;

        sound = GetComponent<UIEffectSound>();
    }
    
    private void Update()
    {
        KeyBoardAction();
    }

    private void KeyBoardAction()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            // Inventory & Equipment
            if (isActivatedPushUI)
            {
                ClosePushUI();
            }
            else
            {
                CloseUI();
                OpenPushUI();
            }
        }

        // if (Input.GetKeyDown(KeyCode.U))
        // {
        //     // Inventory & Equipment
        //     if (isActivatedAuctionUI)
        //     {
        //         CloseAuctionUI();
        //     }
        //     else
        //     {
        //         OpenAuctionUI();
        //     }
        // }

        if (Input.GetKeyDown(KeyCode.I))
        {
            // Inventory & Equipment
            if (isActivatedEquipmentUI)
            {
                CloseEquipmentUI();
            }
            else
            {
                CloseUI();
                OpenEquipmentUI();
            }
        }
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseUI();
        }
        // if (Input.GetKeyDown(KeyCode.O))
        // {
        //     // Inventory & Equipment
        //     if (isActivatedReinforceUI)
        //     {
        //         CloseReinforceUI();
        //     }
        //     else
        //     {
        //         OpenReinforceUI();
        //     }
        // }

        // if (Input.GetKeyDown(KeyCode.P))
        // {
        //     if (isActivatedShopUI)
        //     {
        //         CloseShopUI();
        //     }
        //     else
        //     {
        //         OpenShopUI();
        //     }
        // }

        // if (Input.GetKeyDown(KeyCode.C))
        // {
        //     if (isActivatedCraftUI)
        //     {
        //         CloseCraftUI();
        //     }
        //     else
        //     {
        //         OpenCraftUI();
        //     }
        // }
    }

    public void UpdateStatUI(Stat characterStat)
    {
        equiment.UpdateStatText(characterStat);
    }

    // Inventory UI + background
    public void OpenInventoryUI(string type = "default")
    {
        CloseQuickSlotUI();
        background.SetActive(true);
        inventory.Open(type);
    }

    public void CloseInventoryUI()
    {
        background.SetActive(false);
        inventory.Close();
        itemDetail.Close();
        OpenQuickSlotUI();
    }

    // Equipment UI + Inventory UI
    public void OpenEquipmentUI()
    {
        isActivatedEquipmentUI = true;
        OpenInventoryUI();
        equiment.Open();

        sound.PlayOpenInventorySound();
    }

    public void CloseEquipmentUI()
    {
        isActivatedEquipmentUI = false;
        equiment.Close();
        CloseInventoryUI();

        sound.PlayOpenInventorySound();
    }

    // Shop UI + Inventory UI
    public void OpenShopUI()
    {
        isActivatedShopUI = true;
        OpenInventoryUI("Shop");
        shop.Open();
    }

    public void CloseShopUI()
    {
        isActivatedShopUI = false;
        shop.Close();
        CloseInventoryUI();
    }

    // Auction UI + Inventory UI
    public void OpenAuctionUI()
    {
        isActivatedAuctionUI = true;
        OpenInventoryUI("Auction");
        auction.Open();
    }

    public void CloseAuctionUI()
    {
        isActivatedAuctionUI = false;
        auction.Close();
        CloseInventoryUI();
    }

    // Push UI
    public void OpenPushUI()
    {
        CloseQuickSlotUI();
        isActivatedPushUI = true;
        background.SetActive(true);
        push.Open();
    }

    public void ClosePushUI()
    {
        isActivatedPushUI = false;
        background.SetActive(false);
        push.Close();
        OpenQuickSlotUI();
    }

    // Craft UI + Inventory UI
    public void OpenCraftUI()
    {
        isActivatedCraftUI = true;
        OpenInventoryUI("Craft");
        craft.Open();
    }

    public void CloseCraftUI()
    {
        isActivatedCraftUI = false;
        craft.Close();
        CloseInventoryUI();
    }

    // Reinforce UI + Inventory UI
    public void OpenReinforceUI()
    {
        isActivatedReinforceUI = true;
        OpenInventoryUI("Reinforce");
        reinforce.Open();
    }

    public void CloseReinforceUI()
    {
        isActivatedReinforceUI = false;
        reinforce.Close();
        CloseInventoryUI();
    }

    // QuickSlot UI
    public void OpenQuickSlotUI()
    {
        quickSlot.Open();
    }

    public void CloseQuickSlotUI()
    {
        quickSlot.Close();
    }

    // Item Detail
    public void OpenItemDetail(Item item, Vector3 pos)
    {
        itemDetail.Open(item, pos);
        sound.PlaySelectItemSound();
    }
    public void CloseItemDetail()
    {
        itemDetail.Close();
    }

    public void CloseUI()
    {
        CloseEquipmentUI();
        CloseShopUI();
        CloseAuctionUI();
        ClosePushUI();
        CloseCraftUI();
        CloseReinforceUI();

        popupMessage.Close();
        popupSetCount.Close();
        popupAsk.Close();
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

    // -------------------------------------------------------------
    // Sound
    // -------------------------------------------------------------
    public void PlayEquipSound(ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.Weapon:
                sound.PlayEquipSwordSound();
                break;
            case ItemType.Armor:
                sound.PlayEquipMetalArmorSound();
                break;
        }
    }

    public void PlayGetItemSound()
    {
        sound.PlayGetItemSound();
    }

    // -------------------------------------------------------------
    // Dungeon floor
    // -------------------------------------------------------------
    public void ActivateFloorText(int floor)
    {
        floorText.text = string.Format("{0}F", floor);
        floorFrame.SetActive(true);
    }
    public void DeActivateFloorText()
    {
        floorFrame.SetActive(false);
    }

    // -------------------------------------------------------------
    // 팝업 메시지
    // -------------------------------------------------------------
    public void PopupMessage(string message, float time = 1f)
    {
        popupMessage.Popup(message, time);
    }

    public void PopupMessageClose()
    {
        popupMessage.Close();
    }

    public void PopupAsk(string type, string ask, string leftText, string rightText)
    {
        popupAsk.Popup(type, ask, leftText, rightText);
    }
    public void PopupSetCount(string type, string message, string leftText, string rightText, int maxCount, int defaultCount)
    {
        popupSetCount.Popup(type, message, leftText, rightText, maxCount, defaultCount);
    }
}