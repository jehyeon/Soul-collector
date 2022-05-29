using UnityEngine;

public class ArmorMastery : PassiveSkill
{
    protected override void Activate()
    {
        id = 2;
        type = SkillType.Passive;
        rank = SkillRank.Green;
        passiveStat.SumForCollect(3, 2);
        player.gameManager.AddPassiveSkillBuff(id, passiveStat);
    }
}
