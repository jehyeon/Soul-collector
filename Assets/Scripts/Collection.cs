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
    private GameObject selectedMark;
    [SerializeField]
    private TextMeshProUGUI textObject;

    private int index;
    private string statText;
    private CollectionType type;

    private bool activated = false;

    public string StatText { get { return statText; } }
    public bool Activated { get { return activated; } }
    public int Index { get { return index; } }
    public CollectionType Type { get { return type; } }

    public void Set(Collect collect, CollectionType _type, int _index)
    {
        parent = collect;
        type = _type;
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
        selectedMark.SetActive(true);
    }

    public void UnSelect()
    {
        selectedMark.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Select();
        }
    }
}
