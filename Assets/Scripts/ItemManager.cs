using System.IO;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemManager: MonoBehaviour
{
    private List<Dictionary<string, object>> data;
    private Item item;
    
    private void Awake()
    {
        data = CSVReader.Read("Item");
        item = gameObject.AddComponent<Item>();
    }

    public Item Get(int itemId)
    {
        item.Set(itemId, data[itemId]);
        return item;
    }
}