using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempAutoHunt : MonoBehaviour
{
    // public float findEnemyCoolTime = 2f;
    // private float findEnemyTime;

    // public bool isAutoHunt;

    // private List<GameObject> FoundEnemies;

    // void Start()
    // {
    //     isAutoHunt = false;
    //     findEnemyTime = 0;
    //     FoundEnemies = new List<GameObject>(GameObject.FindGameObjectsWithTag("Enemy"));
    // }

    // // Update is called once per frame
    // void Update()
    // {
    //     if (isAutoHunt)
    //     {
    //         FindNearbyEnemy();
    //         CheckRemoveTarget();
    //     }

    //     if (Input.GetKeyDown(KeyCode.G))
    //     {
    //         // G 키로 자동사냥 활성화
    //         isAutoHunt = !isAutoHunt;
    //     }
    // }

    // private void FindNearbyEnemy()
    // {
    //     // PlayerController를 가져와서 target을 선정함
    //     // findEnemyTime += Time.deltaTime;

    //     if (!this.GetComponent<Player>().isTarget)
    //     {
    //         Debug.Log(FoundEnemies);
    //         // findEnemyTime = 0;
    //         FoundEnemies = new List<GameObject>(GameObject.FindGameObjectsWithTag("Enemy"));

    //         // 우선 첫번째 대상을 선택
    //         this.GetComponent<Player>().SetTarget(FoundEnemies[0]);
    //     }
    // }

    // private void CheckRemoveTarget()
    // {
    //     // isTarget이 없어지면 임시로 적 리스트를 초기화 시킴
    //     FoundEnemies.Clear();
    // }
}
