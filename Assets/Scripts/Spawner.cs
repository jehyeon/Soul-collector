using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField]
    private GameManager gameManager;

    private float roomWidth;
    // 스폰 범위 (spawner 위치 기준)

    [SerializeField]
    private EnemyObjectPool enemyObjectPool;

    [SerializeField]
    private CheckSpawnPosition spawnPosition;

    private float spawnCycle;
    private int maxEnemyCount;

    private int remainEnemyCount;       // 남은 enemy 숫자

    public GameManager GameManager { get { return gameManager; } }
    
    // -------------------------------------------------------------
    // Init
    // -------------------------------------------------------------
    private void Awake()
    {
        remainEnemyCount = 0;
    }

    private void Run()
    {
        // 스폰 위치에 반경 설정 후 위치 찾기 시작
        spawnPosition.SetRange(this, roomWidth);

        InitSpawn();
    }

    public void Set(GameManager gm, float _roomWidth, int maxCount, float cycle)
    {
        gameManager = gm;
        roomWidth = _roomWidth;
        spawnCycle = cycle;
        maxEnemyCount = maxCount;

        Run();
    }

    private void InitSpawn()
    {
        for (int i = 0; i < maxEnemyCount; i++)
        {
            Spawn();
        }
    }

    public void Spawn()
    {
        if (remainEnemyCount >= maxEnemyCount)
        {
            // Enemy 생성 제한
            return;
        }

        if (spawnPosition.CanSpawn)
        {
            // CheckSpawnPosition에서 미리 스폰 위치에 충돌이 있는지 확인 후 호출
            Enemy enemy = enemyObjectPool.Get();
            enemy.transform.position = spawnPosition.transform.position;
            // 랜덤 회전
            enemy.transform.rotation = Quaternion.Euler(new Vector3(0f, Random.Range(0f, 360f), 0f));

            enemy.Set(this, gameManager.Player.gameObject);

            remainEnemyCount += 1;
        }
    }

    public void Die(Enemy enemy)
    {
        gameManager.Drop(enemy.DropId, enemy.transform.position);
        // enemy.Reset();
        enemyObjectPool.Return(enemy);
        remainEnemyCount -= 1;

        Invoke("Spawn", spawnCycle);        // spawnCycle 이후 리스폰
    }
}
