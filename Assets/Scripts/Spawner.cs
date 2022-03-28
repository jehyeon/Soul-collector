using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField]
    private GameManager gameManager;

    // 스폰 범위 (spawner 위치 기준)
    [SerializeField]
    private float halfRow;
    [SerializeField]
    private float halfColumn;

    [SerializeField]
    private EnemyObjectPool enemyObjectPool;

    [SerializeField]
    private CheckSpawnPosition spawnPosition;

    [SerializeField]
    private float spawnCycle;

    private int remainEnemyCount;       // 남은 enemy 숫자

    public EnemyObjectPool ObjectPool { get { return enemyObjectPool; } }

    public GameManager GameManager { get { return gameManager; } }
    
    // Start is called before the first frame update
    private void Start()
    {
        remainEnemyCount = 0;

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
            Enemy enemy = enemyObjectPool.Get();
            enemy.transform.position = spawnPosition.transform.position;

            if (enemy.Target == null || enemy.ParentSpawner == null)
            {
                enemy.SetParentSpawner(this);
                // enemy는 player를 항상 타겟으로 지정
                enemy.SetTarget(gameManager.Player.gameObject);
            }

            remainEnemyCount += 1;
        }
    }

    public void Die()
    {
        remainEnemyCount -= 1;
    }
}
