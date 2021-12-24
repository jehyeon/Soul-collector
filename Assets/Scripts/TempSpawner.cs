using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempSpawner : MonoBehaviour
{
    public GameObject enemyPref;
    public float fieldX = 50;
    public float fieldY = 50;

    private float temp = 5f;
    private float tempNow = 0f;

    void Start()
    {
        Instantiate(enemyPref, this.transform.position, this.transform.rotation);
    }

    // Update is called once per frame
    void Update()
    {
        tempNow += Time.deltaTime;

        if (tempNow > temp)
        {
            tempNow = 0;
            Instantiate(enemyPref, this.transform.position + new Vector3(0, 1, 0), this.transform.rotation);
        }
    }
}
