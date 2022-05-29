using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class BuffSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private Image buffImage;
    [SerializeField]
    private GameObject infinityMark;
    [SerializeField]
    private TextMeshProUGUI remainTimeText;

    private BuffSystem buffSystem;
    private Buff buff;
    private float remainTime;
    private int id;
    public int Id { get { return id; } }

    public void Set(BuffSystem parent, int buffId, Sprite image, Stat buffStat, float time)
    {
        buffSystem = parent;
        id = buffId;
        remainTime = time;
        buffImage.sprite = image;
        buff = new Buff();
        buff.SetStat(buffStat);

        if (remainTime == -1f)
        {
            remainTimeText.gameObject.SetActive(false);
            infinityMark.SetActive(true);
        }
        else
        {
            infinityMark.SetActive(false);
            remainTimeText.text = string.Format("{0}", remainTime);
            remainTimeText.gameObject.SetActive(true);
            Invoke("EndBuff", remainTime);
            InvokeRepeating("UpdateRemainTimeText", 0f, 1f);
        }

        StartBuff();
    }

    private void UpdateRemainTimeText()
    {
        remainTime -= 1f;
        remainTimeText.text = string.Format("{0}", remainTime);

        if (remainTime == 0f)
        {
            CancelInvoke("UpdateRemainTimeText");
        }
    }

    private void StartBuff()
    {
        buffSystem.player.Stat.ActivateBuff(buff);
    }

    private void EndBuff(bool hpContinue = false)
    {
        buffSystem.player.Stat.DeActivateBuff(buff, hpContinue);
        buffSystem.RemoveBuff(id, this.gameObject);
    }

    public void EndNow()
    {
        // invoke 종료
        CancelInvoke("EndBuff");
        CancelInvoke("UpdateRemainTimeText");

        EndBuff(true);
    }

    // 마우스 이벤트
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Vector3 pos, string skillName, Color skillNameColor, string des, string additionalDes = ""
        SkillDes info = SkillManager.Instance.Get(id);
        // 마우스 오버
        UIController.Instance.OpenSkillDetail(transform.position, info.Name, info.Color, info.Description, buff.Stat.ToStringForTooltip());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // 마우스 오버 아웃
        UIController.Instance.CloseSkillDetail();
    }
}
