using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager
{
    public List<Dictionary<string, object>> data;

    public ShopManager()
    {
        data = CSVReader.Read("Shop");
    }
}
