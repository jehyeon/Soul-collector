using UnityEngine;
using System.Collections.Generic;

public class SkillSystem : MonoBehaviour
{
    [SerializeField]
    private GameManager gameManager;

    // 스킬 view
    [SerializeField]
    private GameObject publicSkillParent;
    [SerializeField]
    private GameObject oneHandSkillParent;
    // [SerializeField]
    // private GameObject twoHandSkillParent;
    // [SerializeField]
    // private GameObject bowSkillParent;

    public List<SkillSlot> slots;

    // 버프 시스템

    public void InitSkillSystem()
    {
        // 각 skill parent child에서 스킬 정보 로드
        slots = new List<SkillSlot>();
        slots.AddRange(publicSkillParent.GetComponentsInChildren<SkillSlot>());
        slots.AddRange(oneHandSkillParent.GetComponentsInChildren<SkillSlot>());
    }

    // -------------------------------------------------------------
    // 스킬 활성화
    // -------------------------------------------------------------
    public void Activate(List<int> skillIds)
    {
        foreach (int id in skillIds)
        {
            Activate(id);
        }
    }

    public void Activate(int id)
    {
        foreach (SkillSlot skillSlot in slots)
        {
            if (skillSlot.Id == id)
            {
                // view
                skillSlot.Activate();
                break;
            }
        }
        AddSkillToPlayer(id);
    }

    public void AddSkillToPlayer(int skillId)
    {
        switch (skillId)
        {
            case 1:
                gameManager.Player.gameObject.AddComponent<IncreaseHealth>();
                break;
            case 2:
                gameManager.Player.gameObject.AddComponent<ArmorMastery>();
                break;
            case 3:
                gameManager.Player.gameObject.AddComponent<Relax>();
                break;
            case 4:
                gameManager.Player.gameObject.AddComponent<Haste>();
                break;
            case 5:
                gameManager.Player.gameObject.AddComponent<WeaponMastery>();
                break;
            case 6:
                gameManager.Player.gameObject.AddComponent<IncreaseAttackSpeed>();
                break;
            case 20:
                gameManager.Player.gameObject.AddComponent<Smite>();
                break;
            case 21:
                gameManager.Player.gameObject.AddComponent<SolidBlock>();
                break;
            case 22:
                gameManager.Player.gameObject.AddComponent<Shield>();
                break;
            case 23:
                gameManager.Player.gameObject.AddComponent<OneHandSwordMastery>();
                break;
            case 24:
                gameManager.Player.gameObject.AddComponent<Heal>();
                break;
            case 25:
                gameManager.Player.gameObject.AddComponent<ShieldReflect>();
                break;
        }
    }

    // GameObject UI
    public void Open()
    {
        this.gameObject.SetActive(true);
    }

    public void Close()
    {
        this.gameObject.SetActive(false);
    }
}
