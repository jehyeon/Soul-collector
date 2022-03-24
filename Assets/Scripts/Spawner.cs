using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    // 스폰 범위 (spawner 위치 기준)
    [SerializeField]
    private float halfRow;
    [SerializeField]
    private float halfColumn;

    [SerializeField]
    private ObjectPool enemyObjectPool;

    [SerializeField]
    private CheckSpawnPosition spawnPosition;

    [SerializeField]
    private float spawnCycle;
    
    // Start is called before the first frame update
    private void Start()
    {
        // 스폰 위치에 반경 설정 후 위치 찾기 시작
        spawnPosition.SetRange(this, halfRow, halfColumn);
        spawnPosition.FindSpawnPosition();
        InvokeRepeating("Spawn", 0, spawnCycle);
    }

    public void Spawn()
    {
        if (spawnPosition.CanSpawn)
        {
            // CheckSpawnPosition에서 미리 스폰 위치에 충돌이 있는지 확인 후 호출
            GameObject enemy = enemyObjectPool.Get();
            enemy.transform.position = spawnPosition.transform.position;
        }
    }
}
