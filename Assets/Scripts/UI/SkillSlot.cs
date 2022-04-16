using UnityEngine;
using UnityEngine.UI;

public class SkillSlot : MonoBehaviour
{
    [SerializeField]
    private Image image;
    [SerializeField]
    private int skillId;
    [SerializeField]
    private GameObject deactive;

    private bool activated = false;

    public int Id { get { return skillId; } }

    public void Activate()
    {
        activated = true;
        deactive.SetActive(false);
    }
}