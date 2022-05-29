using UnityEngine;

public class SolidBlock : PassiveSkill
{
    protected override void Activate()
    {
        id = 21;
        type = SkillType.Passive;
        rank = SkillRank.Blue;
        passiveStat.SumForCollect(3, 5);
        player.gameManager.AddPassiveSkillBuff(id, passiveStat);
    }
}
