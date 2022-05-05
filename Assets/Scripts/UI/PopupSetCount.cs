using UnityEngine;
using TMPro;

public class PopupSetCount : MonoBehaviour
{
    private string type;
    private int max;

    [SerializeField]
    private GameManager gameManager;
    // Confirm (Ask)
    [SerializeField]
    private TextMeshProUGUI textLeftBtn;
    [SerializeField]
    private TextMeshProUGUI textRightBtn;
    [SerializeField]
    private TextMeshProUGUI textMessage;
    [SerializeField]
    private TMP_InputField inputCount;
    
    public void Popup(string countType, string message, string leftText, string rightText, int maxCount, int defaultCount)
    {
        // Popup Confirm
        // rightText가 빨간색으로 하이라이트된 버튼
        textMessage.text = message;
        textLeftBtn.text = leftText;
        textRightBtn.text = rightText;

        type = countType;
        this.gameObject.SetActive(true);
        max = maxCount;
        inputCount.text = defaultCount.ToString();
        Time.timeScale = 0;
    }

    private void Close()
    {
        textMessage.text = "";
        textLeftBtn.text = "";
        textRightBtn.text = "";
        inputCount.text = "";
        type = "";
        Time.timeScale = 1;

        this.gameObject.SetActive(false);
    }

    public void ClickLeft()
    {
        Close();
    }

    public void ClickRight()
    {
        if (inputCount.text == "" || int.Parse(inputCount.text) == 0)
        {
            return;
        }
        gameManager.AnswerCount(type, int.Parse(inputCount.text));
        Close();
    }

    public void CheckInputCount()
    {
        if (inputCount.text != "" && int.Parse(inputCount.text) > max)
        {
            inputCount.text = max.ToString();
        }
    }
}
