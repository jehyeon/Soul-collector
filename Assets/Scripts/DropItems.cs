using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItems : MonoBehaviour
{
    public GameObject dropItem;

    public void DropItem()
    {
        if (Random.value < 0.2f)
        {
            Instantiate(dropItem, this.transform.position, this.transform.rotation);
        }
    }
}
