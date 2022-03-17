using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftManager
{
    public List<Dictionary<string, object>> data;

    public CraftManager()
    {
        data = CSVReader.Read("Craft");
    }
}
