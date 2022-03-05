using System.IO;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DropManager
{
    public List<Dictionary<string, object>> data;

    public DropManager()
    {
        data = CSVReader.Read("Drop");
    }

    public int RandomItem(int dropItemId)
    {
        string itemListString = data[dropItemId]["drop"].ToString();
        
        float percent = 0f;
        float rand = Random.value;

        // data[dropItemId]["drop"]: 확률 오름차순 정렬
        string[] itemList = itemListString.Split('/');
        for (int i = 0; i < itemList.Length; i++)
        {
            string[] item = itemList[i].Split('|');

            percent += (float)System.Convert.ToDouble(item[1]);
            if (rand < percent)
            {
                return int.Parse(item[0]);
            }
        }

        return -1; // 마지막 percent가 1 이상이므로 -1 리턴 시 버그
    }
}