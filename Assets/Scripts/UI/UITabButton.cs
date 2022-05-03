using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UITabButton : MonoBehaviour
{
    [SerializeField]
    private GameObject focus;
    [SerializeField]
    private TextMeshProUGUI tabName;


    public void Select()
    {
        focus.SetActive(true);
        Color selectedFontColor;
        ColorUtility.TryParseHtmlString("#F6E19CFF", out selectedFontColor);
        tabName.color = selectedFontColor;
    }

    public void UnSelect()
    {
        focus.SetActive(false);
        Color unSelectedFontColor;
        ColorUtility.TryParseHtmlString("#FFFFFFFF", out unSelectedFontColor);
        tabName.color = unSelectedFontColor;
    }
}
