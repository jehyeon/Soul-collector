using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonSystem : MonoBehaviour
{
    [SerializeField]
    DungeonGenerator generator;

    private int floor;

    private void Awake()
    {
        floor = 1;
    }

    private void Start()
    {
        CreateDungeon();
    }

    // -------------------------------------------------------------
    // 던전 생성
    // -------------------------------------------------------------
    private void CreateDungeon()
    {
        generator.Generate();
        generator.GenerateWalls();
    }

    private void ClearDungeon()
    {
        generator.Clear();
        generator.ClearWalls();
    }
}
