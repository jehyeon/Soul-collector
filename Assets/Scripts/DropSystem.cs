using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropSystem : MonoBehaviour
{
    [SerializeField]
    private ObjectPool swordOP;
    [SerializeField]
    private ObjectPool boxOP;

    public ObjectPool SwordOP { get { return swordOP; } }
    public ObjectPool BoxOP { get { return boxOP; } }

    public void DropItem(int itemId, Vector3 position)
    {
        // 장비 아이템인 경우 아이템 타입에 맞게 외형 변경
        GameObject itemObject;
        if (itemId < 1600)
        {
            // !!! sword 프리팹으로 고정
            itemObject = swordOP.Get();
        }
        else
        {
            // !!! box 프리팹으로 고정
            itemObject = boxOP.Get();
        }

        itemObject.GetComponent<DroppedItem>().Set(this, itemId);
        itemObject.transform.position = position;
        // 랜덤 회전
        itemObject.transform.rotation = Quaternion.Euler(new Vector3(0f, Random.Range(0f, 360f), 0f));
    }
}
