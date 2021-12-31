using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItems : MonoBehaviour
{
    // 드랍 테이블로 수정
    public GameObject dropItem;

    public void DropItem()
    {
        dropItem.GetComponent<Item>().SetId(Random.Range(0,3));
        Instantiate(dropItem, this.transform.position, this.transform.rotation);
    }
    public void DropGold()
    {
        gameObject.GetComponent<Enemy>().player.GetComponent<Player>().GetGold(100);
    }
}
