using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItems : MonoBehaviour
{
    public GameObject dropItem;

    public void DropItem()
    {
        // if (Random.value < 0.5f)
        // {
            dropItem.GetComponent<Item>().SetId(Random.Range(0,4));

            Instantiate(dropItem, this.transform.position, this.transform.rotation);
        // }
    }
}
