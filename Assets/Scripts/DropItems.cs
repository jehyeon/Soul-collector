using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItems : MonoBehaviour
{
    // 드랍 테이블로 수정
    [SerializeField]
    private GameObject dropWeapon;
    [SerializeField]
    public GameObject dropBox;

    public void DropItem()
    {
        dropWeapon.GetComponent<Item>().SetId(Random.Range(0,3));
        Instantiate(dropWeapon, this.transform.position + new Vector3(0, 1, 0), this.transform.rotation);
    }
    public void DropGold()
    {
        gameObject.GetComponent<Enemy>().player.GetComponent<Player>().GetGold(100);
    }
}
