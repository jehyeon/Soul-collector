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
    public int maxEnemyCount;

    public int remainEnemyCount;       // 남은 enemy 숫자

    public GameManager GameManager { get { return gameManager; } }
    
    // -------------------------------------------------------------
    // Init
    // -------------------------------------------------------------
    private void Awake()
    {
        remainEnemyCount = 0;
    }

    public void Set(GameManager gm, float _roomWidth, int maxCount, float cycle)
    {
        gameManager = gm;
        roomWidth = _roomWidth;
        spawnCycle = cycle;
        maxEnemyCount = maxCount;
        enemyObjectPool.Set(gm.GetEnemyObjects());

        spawnPosition.SetRange(this, roomWidth);
        InitSpawn();
    }

    private void InitSpawn()
    {
        for (int i = 0; i < maxEnemyCount; i++)
        {
            SpawnCall();
        }
    }

    public void Spawn()
    {
        // CheckSpawnPosition에서 미리 스폰 위치에 충돌이 있는지 확인 후 호출
        Enemy enemy = enemyObjectPool.Get();
        enemy.Agent.enabled = false;
        enemy.transform.position = spawnPosition.transform.position;
        enemy.Agent.enabled = true;
        enemy.gameObject.name = this.gameObject.name;
        // 랜덤 회전
        enemy.transform.rotation = Quaternion.Euler(new Vector3(0f, Random.Range(0f, 360f), 0f));

        enemy.Set(this, gameManager.Player.gameObject, spawnPosition.transform.position);

        remainEnemyCount += 1;
    }

    private void SpawnCall()
    {
        // checker에게 스폰 요청
        spawnPosition.SpawnCall();
    }

    public void Die(Enemy enemy)
    {
        gameManager.Drop(enemy.DropId, enemy.transform.position);
        // enemy.Reset();
        enemyObjectPool.Return(enemy);
        remainEnemyCount -= 1;

        Invoke("SpawnCall", spawnCycle);
    }
}
