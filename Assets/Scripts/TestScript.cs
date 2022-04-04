using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    public DungeonGenerator generator;
    // public DamageTextSystem damageTextSystem;

    private void Start()
    {
        generator = GameObject.Find("Dungeon Generator").GetComponent<DungeonGenerator>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            ClearDungeon();
            GenerateDungeon();
        }
    }

    // private void DamageRepeat()
    // {
    //     damageTextSystem.FloatDamageText(Random.Range(0,10), new Vector3(0, 0, 0));
    // }
    public void GenerateDungeon()
    {
        generator.Generate();
    }

    public void ClearDungeon()
    {
        generator.Clear();
    }
}
