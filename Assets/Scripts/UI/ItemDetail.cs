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
        // 슬롯 정보 업데이트
        slot.Set(selectedItem);
        itemName.text = selectedItem.ItemName;
        itemName.color = selectedItem.FontColor;
        itemDes.text = selectedItem.ToString();

        // 위치 조정
        position += new Vector3(
            GetComponent<RectTransform>().rect.width * 0.5f,
            GetComponent<RectTransform>().rect.height * 0.5f * -1f,
            0
        );
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
