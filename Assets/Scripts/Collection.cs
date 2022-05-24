using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum CollectionType
{
    Attack,
    Defense
}

public class Collection : MonoBehaviour
{
    [SerializeField]
    private GameObject activateMark;
    [SerializeField]
    private TextMeshProUGUI textObject;

    private CollectionType type;
    private bool activated = false;

    public void ActivateView()
    {
        this.activated = true;
        activateMark.SetActive(true);
    }

    public void UpdateText(string text)
    {
        textObject.text = text;
    }
}
