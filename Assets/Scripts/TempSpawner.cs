using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempSpawner : MonoBehaviour
{
    public int enemyId = 0;
    public GameObject enemyPref;
    public float fieldX = 50;
    public float fieldY = 50;

    private float temp = 5f;
    private float tempNow = 0f;

    void Start()
    {
        Spawn();
    }

    // Update is called once per frame
    void Update()
    {
        tempNow += Time.deltaTime;

        if (tempNow > temp)
        {
            tempNow = 0;
            Spawn();
        }
    }

    void Spawn()
    {
        int spwanX = Random.Range((int)(this.transform.position.x - fieldX), (int)(this.transform.position.x + fieldX));
        int spwanZ = Random.Range((int)(this.transform.position.y - fieldY), (int)(this.transform.position.y + fieldY));

        GameObject instance = Instantiate(enemyPref, new Vector3(spwanX, 1, spwanZ), this.transform.rotation);
        instance.GetComponent<Enemy>()._stat.LoadFromCSV(enemyId, "Enemy");
    }
}
