using UnityEngine;

public class IncreaseHealth : PassiveSkill
{
    protected override void Activate()
    {
        id = 1;
        type = SkillType.Passive;
        rank = SkillRank.Green;
        passiveStat.SumForCollect(5, 25);
        player.gameManager.AddPassiveSkillBuff(id, passiveStat);
    }
}
