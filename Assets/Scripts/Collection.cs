using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public enum CollectionType
{
    Attack,
    Defense
}

public class Collection : MonoBehaviour, IPointerClickHandler
{
    private Collect parent;
    [SerializeField]
    private GameObject activateMark;
    [SerializeField]
    private TextMeshProUGUI textObject;

    private int index;
    private string statText;
    private CollectionType type;

    private bool activated = false;

    public string StatText { get { return statText; } }

    public void Set(Collect collect, int _index)
    {
        parent = collect;
        index = _index;
    }

    public void ActivateView()
    {
        this.activated = true;
        activateMark.SetActive(true);
    }

    public void UpdateText(string text)
    {
        statText = text;
        textObject.text = text;
    }

    public void Select()
    {
        parent.Select(type, index);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Select();
        }
    }
}
