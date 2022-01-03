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
    private int _rank;

    // Weapon
    private int _maxDamage;
    private int _minDamage;
    private int _defaultDamage;
    private int _attackSpeed;

    // Armor
    private int _damageReduction;
    private int _maxHp;
    private int _hpRecovery;
    private int _evasionPercent;
    private int _moveSpeed;
    private int _criticalPercent;

    public int Id { get { return _id; }}
    public string ItemName { get { return _itemName; }}
    public int ImageId { get { return _imageId; }}
    public int ItemType { get { return _itemType; }}
    public int Rank { get { return _rank; }}

    public int MaxDamage { get { return _maxDamage; }}
    public int MinDamage { get { return _minDamage; }}
    public int DefaultDamage { get { return _defaultDamage; }}
    public int AttackSpeed { get { return _attackSpeed; }}

    public int DamageReduction { get { return _damageReduction; }}
    public int MaxHp { get { return _maxHp; }}
    public int HpRecovery { get { return _hpRecovery; }}
    public int EvasionPercent { get { return _evasionPercent; }}
    public int MoveSpeed { get { return _moveSpeed; }}
    public int CriticalPercent { get { return _criticalPercent; }}

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
        _damageReduction = (int)data[id]["damageReduction"];
        _evasionPercent = (int)data[id]["evasionPercent"];
        _maxHp = (int)data[id]["maxHp"];
        _hpRecovery = (int)data[id]["hpRecovery"];
        _moveSpeed = (int)data[id]["moveSpeed"];
        _criticalPercent = (int)data[id]["criticalPercent"];
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
                description += "공격속도 " + _attackSpeed.ToString() + "%\n";
            }
            if (_defaultDamage != 0)
            {
                description += "기본 데미지 " + _defaultDamage.ToString() + "\n";
            }
            if (_minDamage != 0 && _maxDamage != 0)
            {
                description += "무기 데미지 " + _minDamage.ToString() + " ~ " + _maxDamage.ToString() + "\n";
            }

            return description;
        }
        else if (_itemType >= 2 || _itemType <=9)
        {
            // 방어구 description
            string description = "";
            if (_damageReduction != 0)
            {
                description += "데미지 감소 " + _damageReduction.ToString() + "\n";
            }
            if (_maxHp != 0)
            {
                description += "최대 HP " + _maxHp.ToString() + "\n";
            }
            if (_hpRecovery != 0)
            {
                description += "HP 자동 회복 " + _hpRecovery.ToString() + "\n";
            }
            if (_evasionPercent != 0)
            {
                description += "회피율 " + _evasionPercent.ToString() + "%\n";
            }
            if (_criticalPercent != 0)
            {
                description += "치명타 확률 " + _criticalPercent.ToString() + "%\n";
            }
            if (_moveSpeed != 0)
            {
                description += "이동 속도 " + _moveSpeed.ToString() + "%\n";
            }

            return description;
        }
        else
        {
            return "";
        }
    }    
}
