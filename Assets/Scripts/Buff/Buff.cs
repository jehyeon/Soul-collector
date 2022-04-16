using UnityEngine;

public enum BuffType
{
    Skill,
    Item
}

public class Buff : MonoBehaviour
{
    private int id;
    private bool isDebuff;
    private BuffType type;

    public int Id { get { return id; } }

    public void Set(int _id)
    {
        id = _id;
    }
}