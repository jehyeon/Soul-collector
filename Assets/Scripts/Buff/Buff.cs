using UnityEngine;

public enum BuffType
{
    Collection,
    Skill,
    Item
}

public class Buff
{
    private bool isDebuff;
    private BuffType type;
    private Stat stat;

    public BuffType Type { get { return type; } }
    public Stat Stat { get { return stat; } }

    public void SetStat(Stat buffStat)
    {
        stat = buffStat;
    }
}