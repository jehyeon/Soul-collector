using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckSpawnPosition : MonoBehaviour
{
    private float halfRow;
    private float halfColumn;

    private CapsuleCollider areaCollider;

    private bool spawnCall;
    private bool canSpawn;

    private Spawner spawner;

    public bool CanSpawn { get { return canSpawn; } }

    public List<Collider> colliders;
    

    void Start()
    {
        spawnCall = true;
        areaCollider = GetComponent<CapsuleCollider>();
        // this.gameObject.SetActive(false);
    }

    public void FindSpawnPosition()
    {
        spawnCall = true;
        this.transform.position = new Vector3(
            spawner.transform.position.x + Random.Range(-1 * halfRow, halfRow + 1), 
            spawner.transform.position.y,
            spawner.transform.position.z + Random.Range(-1 * halfColumn, halfColumn + 1)
        );
        this.gameObject.SetActive(true);

        CheckNoColliders();
    }

    public void Stop()
    {
        spawnCall = false;
        canSpawn = false;
        this.gameObject.SetActive(false);
    }

    public void SetRange(Spawner parentSpawner, float _halfRow, float _halfColumn)
    {
        spawner = parentSpawner;
        this.transform.position = spawner.transform.position;
        halfRow = _halfRow;
        halfColumn = _halfColumn;
    }

    private void CheckNoColliders()
    {
        if (colliders.Count == 0)
        {
            canSpawn = true;
        }
        else
        {
            canSpawn = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        colliders.Add(other);
    }

    private void OnTriggerStay()
    {
        canSpawn = false;
        if (spawnCall)
        {
            FindSpawnPosition();    // 충돌체가 있으면 위치 다시 검색
        }
    }

    // private void OnTriggerStay(Collider other)
    // {
    //     canSpawn = false;
    //     colliders.Add(other);
    //     if (spawnCall)
    //     {
    //         FindSpawnPosition();    // 충돌체가 있으면 위치 다시 검색
    //     }
    // }

    private void OnTriggerExit(Collider other)
    {
        colliders.Remove(other);

        CheckNoColliders();     // 충돌체가 없어지면 colliders가 비었는 지 확인
    }

}
