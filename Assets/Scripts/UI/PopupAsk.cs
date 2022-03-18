using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PopupAsk : MonoBehaviour
{
    [SerializeField]
    private GameManager gameManager;
    // Confirm (Ask)
    [SerializeField]
    private TextMeshProUGUI textLeftBtn;
    [SerializeField]
    private TextMeshProUGUI textRightBtn;
    [SerializeField]
    private TextMeshProUGUI textAsk;

    string type;

    // Left button 클릭 시 창 닫고 아무 동작 X
    // Right button 동작은 GameManager에서 분기
    public void Popup(string askType, string ask, string leftText, string rightText)
    {
        // Popup Confirm
        // rightText가 빨간색으로 하이라이트된 버튼
        textAsk.text = ask;
        textLeftBtn.text = leftText;
        textRightBtn.text = rightText;

        type = askType;
        this.gameObject.SetActive(true);
    }

    private void Close()
    {
        textAsk.text = "";
        textLeftBtn.text = "";
        textRightBtn.text = "";
        type = "";

        this.gameObject.SetActive(false);
    }

    public void ClickLeft()
    {
        Close();
    }

    public void ClickRight()
    {
        gameManager.AnswerAsk(type);
        Close();
    }
}
