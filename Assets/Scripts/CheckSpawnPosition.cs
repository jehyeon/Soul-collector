using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckSpawnPosition : MonoBehaviour
{
    private float halfRoomWidth;

    private CapsuleCollider areaCollider;

    public int spawnCall;
    public bool canSpawn;

    private Spawner spawner;

    public bool CanSpawn { get { return canSpawn; } }
    public int SpawnCallCount { get { return spawnCall; } }

    public List<Collider> colliders;
    

    private void Awake()
    {
        areaCollider = GetComponent<CapsuleCollider>();
        canSpawn = false;
        spawnCall = 0;
    }

    // -------------------------------------------------------------
    // 스폰 요청 및 중지
    // -------------------------------------------------------------
    public void SetRange(Spawner parentSpawner, float roomWidth)
    {
        // room width 범위 내에 스폰 위치 선정
        // Spawn position checker 활성화
        spawner = parentSpawner;
        spawnCall = 0;
        this.gameObject.SetActive(true);
        this.transform.position = spawner.transform.position;
        halfRoomWidth = roomWidth / 2.2f;   // 벽에 스폰되는 경우 방지하고자 2f가 아닌 2.2f로 수정

        FindSpawnPosition();    // 처음 시작 시 랜덤 위치로 조정
    }

    private void Stop()
    {
        this.gameObject.SetActive(false);
    }

    public void SpawnCall()
    {
        // parent spawner에서 spawn 요청
        spawnCall += 1;
        this.gameObject.SetActive(true);    // 스폰 위치 검색

        // CheckSpawnCall 재시작
        CancelInvoke("CheckSpawnCall");
        InvokeRepeating("CheckSpawnCall", 0f, 1f);
    }

    private void CheckSpawnCall()
    {
        if (spawnCall == 0)
        {
            // 남은 스폰 요청이 없는 경우
            CancelInvoke("CheckSpawnCall");     // check spawn call 종료
            Stop();
            return;
        }

        if (canSpawn)
        {
            // 현재 스폰 가능한 상태
            spawner.Spawn();    // 스폰
            spawnCall -= 1;
        }
    }

    // -------------------------------------------------------------
    // 스폰 위치 검색
    // -------------------------------------------------------------
    private void FindSpawnPosition()
    {
        canSpawn = false;
        this.transform.position = new Vector3(
            spawner.transform.position.x + Random.Range(-1 * halfRoomWidth, halfRoomWidth + 1), 
            spawner.transform.position.y,
            spawner.transform.position.z + Random.Range(-1 * halfRoomWidth, halfRoomWidth + 1)
        );

        CheckNoColliders();     // 위치 조정 후 스폰 가능한 지 확인
    }

    private void CheckNoColliders()
    {
        if (colliders.Count == 0)
        {
            canSpawn = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    { 
        canSpawn = false;
        colliders.Add(other);
    }

    private void OnTriggerStay()
    {
        canSpawn = false;

        FindSpawnPosition();    // 충돌체가 있으면 위치 다시 검색
    }

    private void OnTriggerExit(Collider other)
    {
        colliders.Remove(other);

        CheckNoColliders();     // 충돌체가 없어지면 colliders가 비었는 지 확인
    }
}
