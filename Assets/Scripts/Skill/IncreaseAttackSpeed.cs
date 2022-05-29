using UnityEngine;

public class IncreaseAttackSpeed : PassiveSkill
{
    protected override void Activate()
    {
        id = 6;
        type = SkillType.Passive;
        rank = SkillRank.Blue;
        passiveStat.SumForCollect(2, 5);
        player.gameManager.AddPassiveSkillBuff(id, passiveStat);
    }
}
