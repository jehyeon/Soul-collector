using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySystem : MonoBehaviour
{
    // Troll Pref
    [SerializeField]
    public GameObject defaultTrollPref;
    [SerializeField]
    private GameObject whiteTrollPref;
    [SerializeField]
    private GameObject darkTrollPref;

    // Minotaur Pref
    [SerializeField]
    private GameObject defaultMinotaurPref;
    [SerializeField]
    private GameObject whiteMinotaurPref;
    [SerializeField]
    private GameObject blueMinotaurPref;

    // Medusa Pref
    [SerializeField]
    private GameObject defaultMedusaPref;
    [SerializeField]
    private GameObject greenMedusaPref;

    private GameObject[][] enemyList;

    private void Awake()
    {
        enemyList = new GameObject[16][]
        {
            new GameObject[] {},
            new GameObject[] { defaultTrollPref },
            new GameObject[] { defaultTrollPref, defaultMinotaurPref },
            new GameObject[] { defaultMinotaurPref },
            new GameObject[] { defaultMinotaurPref, whiteTrollPref },
            new GameObject[] { whiteTrollPref },
            new GameObject[] { whiteTrollPref, whiteMinotaurPref },
            new GameObject[] { whiteMinotaurPref },
            new GameObject[] { whiteMinotaurPref, defaultMedusaPref },
            new GameObject[] { defaultMedusaPref },
            new GameObject[] { defaultMedusaPref, darkTrollPref },
            new GameObject[] { darkTrollPref },
            new GameObject[] { darkTrollPref, blueMinotaurPref },
            new GameObject[] { blueMinotaurPref },
            new GameObject[] { blueMinotaurPref, greenMedusaPref },
            new GameObject[] { greenMedusaPref }
        };
    }

    public GameObject[] GetEnemyObjects(int floor)
    {
        return enemyList[floor];
    }
}
