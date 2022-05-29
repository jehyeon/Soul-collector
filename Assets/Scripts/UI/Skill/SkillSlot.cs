using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SkillSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private UIController uiController;
    
    [SerializeField]
    private Image image;
    [SerializeField]
    private Image frame;
    [SerializeField]
    private GameObject deactive;
    [SerializeField]
    private int skillId;
    [SerializeField]
    private SkillRank rank;
    private bool activated = false;

    private Color frameColor;
    private Color fontColor;

    public int Id { get { return skillId; } }

    private void Start()
    {
        // 이미지 로드
        image.sprite = Resources.Load<Sprite>(string.Format("Skill/{0}", skillId));

        if (rank == SkillRank.Green)
        {
            ColorUtility.TryParseHtmlString("#8BFF8BFF", out frameColor);
            ColorUtility.TryParseHtmlString("#28B71FFF", out fontColor);
        }
        else if (rank == SkillRank.Blue)
        {
            ColorUtility.TryParseHtmlString("#2F66FFFF", out frameColor);
            ColorUtility.TryParseHtmlString("#3275F8FF", out fontColor);
        }
        else
        {
            // Red
            ColorUtility.TryParseHtmlString("#FF4040FF", out frameColor);
            ColorUtility.TryParseHtmlString("#B71B1BFF", out fontColor);
        }
        frame.color = frameColor;
    }

    public void Activate()
    {
        // view
        activated = true;
        deactive.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    { 
        SkillDes info = SkillManager.Instance.Get(skillId);
        // Vector3 pos, string skillName, Color skillNameColor, string des, string additionalDes = ""
        // 마우스 오버
        uiController.OpenSkillDetail(transform.position, info.Name, info.Color, info.Description);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // 마우스 오버 아웃
        uiController.CloseSkillDetail();
    }
}