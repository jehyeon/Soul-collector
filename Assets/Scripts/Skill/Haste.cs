using UnityEngine;

public class Haste : PassiveSkill
{
    protected override void Activate()
    {
        id = 4;
        type = SkillType.Passive;
        rank = SkillRank.Green;
        passiveStat.SumForCollect(6, 5);
        player.gameManager.AddPassiveSkillBuff(id, passiveStat);
    }
}
