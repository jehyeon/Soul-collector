using UnityEngine;

public enum SkillType
{
    PercentPassive,
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
    [SerializeField]
    protected int id;
    [SerializeField]
    protected SkillType type;
    [SerializeField]
    protected SkillRank rank;
    protected bool activated;
    protected int priorSkillId;

    protected Player player;

    public int Id { get { return id; } }
    public SkillType Type { get { return type; } }
    public SkillRank Rank { get { return rank; } }
}

public abstract class PassiveSkill : Skill
{
    protected void Start()
    {
        // player = GetComponent<Player>();
    }

    protected void StartSkill()
    {
        Activate();
    }

    protected abstract void Activate();
}

public abstract class ActiveSkill : Skill
{
    public abstract void Excute();
}

public abstract class PercentPassive : Skill
{

}