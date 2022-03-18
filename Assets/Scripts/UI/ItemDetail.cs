using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemDetail : MonoBehaviour
{
    [SerializeField]
    private Slot slot;
    [SerializeField]
    private TextMeshProUGUI itemName;
    [SerializeField]
    private TextMeshProUGUI itemDes;

    public void Open(Item selectedItem, Vector3 position)
    {
        float width = GetComponent<RectTransform>().rect.width;
        float height = GetComponent<RectTransform>().rect.height;
        // 슬롯 정보 업데이트
        slot.Set(selectedItem);
        itemName.text = selectedItem.ItemName;
        itemName.color = selectedItem.FontColor;
        itemDes.text = selectedItem.ToString();

        // 위치 조정
        position += new Vector3(
            width * 0.5f,
            height * 0.5f * -1f,
            0
        );

        // 아이템 툴팁이 화면 밖으로 나가는 경우
        // 위치를 화면 안으로 밀어줌
        if (position.x + width * 0.5f > Screen.width)
        {
            position.x = Screen.width - width * 0.5f;
        }
        if (position.y + height * -.5f < 0)
        {
            position.y = height * 0.5f;
        }

        this.transform.position = position;
        
        this.gameObject.SetActive(true);
    }

    public void Close()
    {
        this.gameObject.SetActive(false);
        Clear();
    }

    private void Clear()
    {
        slot.ClearSlot();
        itemName.text = "";
        itemName.color = Color.white;
        itemDes.text = "";
    }
}
