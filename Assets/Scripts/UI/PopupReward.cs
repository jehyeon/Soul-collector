using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PopupReward : MonoBehaviour
{
    [SerializeField]
    private GameManager gameManager;
    // 메시지 알림
    [SerializeField]
    private Slot slot;

    public void Popup(Item item)
    {
        // 잠시 뒤 없어지는 팝업 메시지
        slot.Set(item);
        this.gameObject.SetActive(true);
        Time.timeScale = 0;
    }

    public void Close()
    {
        this.gameObject.SetActive(false);
        Time.timeScale = 1;
    }

    public void ClickOK()
    {
        Time.timeScale = 1;
        if (gameManager.Inventory.isFullInventory())
        {
            gameManager.PopupMessage("인벤토리에 남은 공간이 없습니다.");
        }
        else
        {
            gameManager.AnswerAsk("Reward");
        }
        this.gameObject.SetActive(false);
    }
}
