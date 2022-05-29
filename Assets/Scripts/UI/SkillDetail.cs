using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillDetail : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI skillName;
    [SerializeField]
    private TextMeshProUGUI skillDescription;
    [SerializeField]
    private GameObject additionalDesObject;
    [SerializeField]
    private TextMeshProUGUI skillAdditionalDescription;
    private RectTransform rectTr;

    private void Awake()
    {
        rectTr = this.GetComponent<RectTransform>();
    }
    public void Open(Vector3 position, string name, Color nameColor, string description, string additionalDes = "")
    {
        skillName.text = name;
        skillName.color = nameColor;
        skillDescription.text = description;

        if (additionalDes == "")
        {
            // 추가 설명이 공란인 경우
            additionalDesObject.SetActive(false);
        }
        else
        {
            skillAdditionalDescription.text = additionalDes;
            additionalDesObject.SetActive(true);
        }

        this.gameObject.SetActive(true);
        // size fitter

        // Canvas.ForceUpdateCanvases();   // enable true only
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTr);
        float width = rectTr.rect.width;
        float height = rectTr.rect.height;
        
        // 위치 조정
        // position += new Vector3(
        //     width * 0.5f,
        //     height * 0.5f * -1f,
        //     0
        // );

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
    }

    public void Close()
    {
        this.gameObject.SetActive(false);
        Clear();
    }

    private void Clear()
    {
        // slot.ClearSlot();
        skillName.text = "";
        skillName.color = Color.white;
        skillDescription.text = "";
    }
}
