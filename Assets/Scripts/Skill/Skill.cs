using UnityEngine;

public enum SkillType
{
    PercentPassive,
    AttackedPassive,
    Passive,
    Active,
}

public enum SkillRank
{
    Red,
    Blue,
    Green
}

public class Skill : MonoBehaviour
{
    protected int id;
    protected SkillType type;
    protected SkillRank rank;
    protected Player player;
    public int Id { get { return id; } }
    public SkillType Type { get { return type; } }
    public SkillRank Rank { get { return rank; } }
}

public abstract class PassiveSkill : Skill
{
    protected Stat passiveStat;
    public Stat PassiveStat { get { return passiveStat; } }

    protected void Start()
    {
        player = GetComponent<Player>();
        passiveStat = new Stat(true);
        Activate();
    }

    protected abstract void Activate();
}

public abstract class AttackedPassiveSkill : Skill
{
    protected float percent;
    protected float time;
    protected bool activated;
    public float Percent { get { return percent; } }
    public bool Activated { get { return activated; } }

    protected virtual void Start()
    {
        player = GetComponent<Player>();
        player.AddAttackedPassiveSkill(this);
    }

    public abstract void Activate();
}

public abstract class ActiveSkill : Skill
{
    public abstract void Excute();
}

public abstract class PercentPassive : Skill
{

}