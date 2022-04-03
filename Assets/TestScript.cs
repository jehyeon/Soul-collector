using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    public DamageTextSystem damageTextSystem;

    private void Start()
    {
        InvokeRepeating("DamageRepeat", 0f, 1f);
    }

    private void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     damageTextSystem.FloatDamageText(Random.Range(0,10), new Vector3(0, 0, 0));
        // }
    }

    private void DamageRepeat()
    {
        damageTextSystem.FloatDamageText(Random.Range(0,10), new Vector3(0, 0, 0));
    }
}
