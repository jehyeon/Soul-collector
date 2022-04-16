using UnityEngine;
using System.Collections.Generic;

public class SkillSystem : MonoBehaviour
{
    [SerializeField]
    private Player player;
    [SerializeField]
    private UIController uiController;

    // 스킬 view
    [SerializeField]
    private GameObject publicSkillParent;
    [SerializeField]
    private GameObject oneHandSkillParent;
    [SerializeField]
    private GameObject twoHandSkillParent;
    [SerializeField]
    private GameObject bowSkillParent;

    private SkillSlot[] slots;

    // 버프 시스템

    public void InitSkillSystem()
    {
        // 각 skill parent child에서 스킬 정보 로드
        slots = publicSkillParent.GetComponentsInChildren<SkillSlot>();
    }

    public void Activate(List<int> skillIds)
    {
        // save 정보를 전달 받아 활성화
        foreach (int id in skillIds)
        {
            foreach (SkillSlot skillSlot in slots)
            {
                if (skillSlot.Id == id)
                {
                    skillSlot.Activate();       // view
                    AddSkillComponent(id);
                }
            }
        }
    }

    public void Activate(int id)
    {
        foreach (SkillSlot skillSlot in slots)
        {
            if (skillSlot.Id == id)
            {
                skillSlot.Activate();       // view
                AddSkillComponent(id);
            }
        }
    }

    private void AddSkillComponent(int skillId)
    {
        switch (skillId)
        {
            case 0:
                player.gameObject.AddComponent<IncreaseArmor>();
                break;
        }

    }
}
