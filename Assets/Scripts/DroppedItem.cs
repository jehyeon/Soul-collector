using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppedItem : MonoBehaviour
{
    private int id;
    public int Id { get { return id; } }

    public void SetDrop(int itemId)
    {
        id = itemId;
    }
}
