using System.IO;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemManager: MonoBehaviour
{
    public List<Dictionary<string, object>> data;

    private void Awake()
    {
        data = CSVReader.Read("Item");
    }

    public Item Get(int itemId)
    {
        Item item = gameObject.AddComponent<Item>();
        item.Set(itemId, data[itemId]);

        return item;
    }
}