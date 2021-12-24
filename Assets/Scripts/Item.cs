using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public int id;
    public string itemName;
    public enum itemType
    {
        Equipment,
        ETC
    }
    public int itemCount;
    public Sprite itemImage;

    // Weapon
    public float maxDamage;

    public void SetId(int _id)
    {
        // temp
        id = _id;
    }
}
