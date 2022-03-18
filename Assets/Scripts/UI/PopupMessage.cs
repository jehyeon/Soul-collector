using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PopupMessage : MonoBehaviour
{
    // 메시지 알림
    [SerializeField]
    private TextMeshProUGUI message;

    public void Popup(string messageText, float time = 1f)
    {
        // 잠시 뒤 없어지는 팝업 메시지
        message.text = messageText;
        this.gameObject.SetActive(true);
        Invoke("Close", time);
    }

    private void Close()
    {
        this.gameObject.SetActive(false);
    }
}
