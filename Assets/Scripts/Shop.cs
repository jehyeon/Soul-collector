using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    [SerializeField]
    private GameObject pref_shopItem;

    void Awake()
    {
        List<Dictionary<string, object>> data = CSVReader.Read("Shop");
        Debug.Log(data.Count);
        for (int id = 0; id < data.Count; id++)
        {
            GameObject item = Instantiate(pref_shopItem);
            item.transform.SetParent(this.transform);
            item.GetComponent<ShopItem>().Set((int)data[id]["itemId"], (int)data[id]["price"]);
        }
    }
}
