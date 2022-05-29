using UnityEngine;

public class WeaponMastery : PassiveSkill
{
    protected override void Activate()
    {
        id = 5;
        type = SkillType.Passive;
        rank = SkillRank.Blue;
        passiveStat.SumForCollect(0, 5);
        player.gameManager.AddPassiveSkillBuff(id, passiveStat);
    }
}
