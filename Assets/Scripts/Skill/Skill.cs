using UnityEngine;

public enum SkillType
{
    PercentPassive,
    Passive,
    Active,
}

public class Skill : MonoBehaviour
{
    protected int id;
    protected int rank;
    protected bool activated;
    protected int priorSkillId;
    protected SkillType type;

    public int Id { get { return id; } }
    public SkillType Type { get { return type; } }
}

public class PassiveSkill : Skill
{
    protected Player player;

    protected void Start()
    {
        player = GetComponent<Player>();
        Activate();
    }

    protected virtual void Activate()
    {
        // 하위 클래스에서 할당
    }
}

public abstract class ActiveSkill : Skill
{
    public abstract void Excute();
}

public abstract class PercentPassive : Skill
{

}