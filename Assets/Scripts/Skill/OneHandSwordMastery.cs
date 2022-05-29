using UnityEngine;

public class OneHandSwordMastery : PassiveSkill
{
    protected override void Activate()
    {
        id = 23;
        type = SkillType.Passive;
        rank = SkillRank.Red;
        passiveStat.SumForCollect(0, 10);
        passiveStat.SumForCollect(2, 10);
        passiveStat.SumForCollect(1, 10);
        player.gameManager.AddPassiveSkillBuff(id, passiveStat);
    }
}
