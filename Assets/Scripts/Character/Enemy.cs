using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : ACharacter
{
    private Spawner spawner;
    private int enemyId;
    private Vector3 startPos;

    private int findRange = 5;      // 플레이어 감지 범위 (임시)
    private int backRange = 20;     // 스폰 위치로부터의 제한 거리 (임시)

    public Spawner ParentSpawner { get { return spawner; } }

    public void Start()
    {
        // 초기화
        startPos = this.transform.position;         // 임시
        InvokeRepeating("FindNearByPlayer", 0f, 1f);

        SetTarget(GameObject.Find("Player"));       // 임시
    }

    private void Update()
    {
        Move();
    }

    public void SetParentSpawner(Spawner parentSpawner)
    {
        spawner = parentSpawner;
    }

    private void FindNearByPlayer()
    {
        if (state == State.Idle)
        {
            // 되돌아가는 상태가 아닐 때
            if (targetDistance <= Mathf.Pow(findRange, 2))
            {
                destinationPos = target.transform.position;
                state = State.Move;
            }   
        }
    }

    protected override void Die()
    {
        // gameObject.GetComponent<DropItems>().DropItem();
        // gameObject.GetComponent<DropItems>().DropGold();
        spawner.ObjectPool.Return(this);
    }
}
