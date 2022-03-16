using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PopupMessage : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI message;

    public void Popup(string messageText)
    {
        // 1초 뒤 없어지는 팝업 메시지
        message.text = messageText;
        this.gameObject.SetActive(true);
        Invoke("Close", 1f);
    }

    private void Close()
    {
        this.gameObject.SetActive(false);
    }
}
