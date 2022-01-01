using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public int _id;
    private string _itemName;
    private int _imageId;
    private int _itemType;      // itemp type 1 ~ 13 (장비), 14 (리소스 itemCount가 존재)

    // Weapon
    private int _maxDamage;
    private int _minDamage;
    private int _defaultDamage;
    private int _attackSpeed;
    private int _rank;

    public int Id { get { return _id; }}
    public string ItemName { get { return _itemName; }}
    public int ItemType { get { return _itemType; }}
    public int MaxDamage { get { return _maxDamage; }}
    public int MinDamage { get { return _minDamage; }}
    public int DefaultDamage { get { return _defaultDamage; }}
    public int AttackSpeed { get { return _attackSpeed; }}
    public int Rank { get { return _rank; }}

    public int ImageId { get { return _imageId; }}

    public void LoadFromCSV(int id, string fileName)
    {
        SetId(id);
        List<Dictionary<string, object>> data = CSVReader.Read(fileName);
        _imageId = (int)data[id]["imageId"];
        _itemType = (int)data[id]["itemType"];
        _rank = (int)data[id]["rank"];
        _itemName = data[id]["itemName"].ToString();
        _defaultDamage = (int)data[id]["defaultDamage"];
        _maxDamage = (int)data[id]["maxDamage"];
        _minDamage = (int)data[id]["minDamage"];
        _attackSpeed = (int)data[id]["attackSpeed"];
    }

    public void SetId(int id)
    {
        _id = id;
    }

    public override string ToString()
    {
        if (_itemType == 0)
        {
            // 무기 description
            string description = "";

            if (_attackSpeed != 0)
            {
                description += "공격속도 " + _attackSpeed.ToString() + "\n";
            }
            if (_defaultDamage != 0)
            {
                description += "기본 데미지 " + _defaultDamage.ToString() + "\n";
            }
            return description
                + "최소 데미지 " + _minDamage.ToString() + "\n"
                + "최대 데미지 " + _maxDamage.ToString() + "\n";
        }
        else
        {
            // 임시
            return "";
        }
    }    
}
