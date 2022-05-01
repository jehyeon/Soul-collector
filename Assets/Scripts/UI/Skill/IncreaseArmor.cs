using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreaseArmor : PassiveSkill
{
    protected override void Activate()
    {
        player.Stat.DamageReduction +=  1;
    }
}
