using UnityEngine;

public class Relax : PassiveSkill
{
    protected override void Activate()
    {
        id = 3;
        type = SkillType.Passive;
        rank = SkillRank.Green;
        passiveStat.SumForCollect(4, 3);
        player.gameManager.AddPassiveSkillBuff(id, passiveStat);
    }
}
