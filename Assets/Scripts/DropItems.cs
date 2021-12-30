using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItems : MonoBehaviour
{
    public GameObject dropItem;

    public void DropItem()
    {
        dropItem.GetComponent<Item>().SetId(1);
        Instantiate(dropItem, this.transform.position, this.transform.rotation);
    }
}
