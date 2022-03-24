using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyObjectPool : MonoBehaviour
{
    // public static ObjectPool Instance;
    [SerializeField]
    private GameObject poolingObjectPref;
    [SerializeField]
    private int _count = 0;
    [SerializeField]
    private int _additionalCount = 0;

    Queue<Enemy> poolingObjectQueue = new Queue<Enemy>();

    private void Awake()
    {
        // Instance = this;

        Init(_count);
    }

    private void Init(int count)
    {
        for (int i = 0; i < count; i++)
        {
            poolingObjectQueue.Enqueue(CreateObject());
        }
    }

    private Enemy CreateObject()
    {
        GameObject enemyObject = Instantiate(poolingObjectPref);
        enemyObject.SetActive(false);
        enemyObject.transform.SetParent(this.transform);

        return enemyObject.GetComponent<Enemy>();
    }

    public Enemy Get()
    {
        if (this.poolingObjectQueue.Count < 1)
        {
            Init(_additionalCount);
        }

        Enemy enemy = this.poolingObjectQueue.Dequeue();
        enemy.transform.SetParent(null);
        enemy.gameObject.SetActive(true);

        return enemy;
    }

    public void Return(Enemy enemy)
    {
        enemy.gameObject.SetActive(false);
        enemy.transform.SetParent(this.transform);
        this.poolingObjectQueue.Enqueue(enemy);
    }
}