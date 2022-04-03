using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppedItem : MonoBehaviour
{
    private DropSystem parentDropSystem;
    private int id;
    public int Id { get { return id; } }

    public void Set(DropSystem system, int itemId)
    {
        parentDropSystem = system;
        id = itemId;
    }

    public void Return()
    {
        if (id < 1600)
        {
            // !!! sword 프리팹으로 고정
            parentDropSystem.SwordOP.Return(this.gameObject);
        }
        else
        {
            // !!! box 프리팹으로 고정
            parentDropSystem.BoxOP.Return(this.gameObject);
        }
    }
}
