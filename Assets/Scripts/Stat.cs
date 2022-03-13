using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stat
{
    // 공격
    protected float _criticalPercent;     // 치명타 확률
    protected float _attackSpeed;       // 공격속도
    protected int _maxDamage;           // 최대 가변 데미지
    protected int _minDamage;           // 최소 가변 데미지
    protected int _defaultDamage;       // 기본 데미지
    protected int _accuracy;            // 공격 정확도
    protected int _absoluteAccuracy;    // 명중률

    // 방어
    protected int _maxHp;
    protected int _hp;                  // 체력
    protected int _hpRecovery;          // Hp 자동 회복
    protected int _damageReduction;     // 데미지 감소
    protected int _evasionPercent;      // 회피율

    // 기타
    protected float _moveSpeed;         // 이동속도
    protected float _attackRange;       // 공격 범위

    public float CriticalPercent { get { return _criticalPercent; } }
    public float AttackSpeed { get { return _attackSpeed; } }
    public int MaxDamage { get { return _maxDamage; } }
    public int MinDamage { get { return _minDamage; } }
    public int DefaultDamage { get { return _defaultDamage; } }
    public int Accuracy { get { return _accuracy; } }
    public int AbsoluteAccuracy { get { return _absoluteAccuracy; } }
    public int MaxHp { get { return _maxHp; } }
    public int Hp { get { return _hp; } set { _hp = value; } }
    public int HpRecovery { get { return _hpRecovery; } }
    public int DamageReduction { get { return _damageReduction; } }
    public int EvasionPercent { get { return _evasionPercent;  } }
    public float MoveSpeed { get { return _moveSpeed; } }
    public float AttackRange { get { return _attackRange; } }

    public Stat()
    {
        // 초기값
        _criticalPercent = 0;
        _attackSpeed = 1;
        _maxDamage = 0;
        _minDamage = 0;
        _defaultDamage = 1;
        _accuracy = 0;
        _absoluteAccuracy = 0;
        _maxHp = 100;
        _hp = 100;
        _hpRecovery = 5;
        _damageReduction = 0;
        _evasionPercent = 0;
        _moveSpeed = 5f;
        _attackRange = 1.1f;
    }

    public void DecreaseHp(int damage)
    {
        _hp -= damage;
    }

    public void LoadFromCSV(int id, string fileName)
    {
        // for enemy
        List<Dictionary<string, object>> data = CSVReader.Read(fileName);
        _criticalPercent = (int)data[id]["criticalPercent"];
        _attackSpeed = (float)System.Convert.ToDouble(data[id]["attackSpeed"]);
        _maxDamage = (int)data[id]["maxDamage"];
        _minDamage = (int)data[id]["minDamage"];
        _defaultDamage = (int)data[id]["defaultDamage"];
        _accuracy = (int)data[id]["accuracy"];
        _absoluteAccuracy = (int)data[id]["absoluteAccuracy"];
        _maxHp = (int)data[id]["maxHp"];
        _hp = (int)data[id]["hp"];
        _hpRecovery = (int)data[id]["hpRecovery"];
        _damageReduction = (int)data[id]["damageReduction"];
        _evasionPercent = (int)data[id]["evasionPercent"];
        _moveSpeed = (float)System.Convert.ToDouble(data[id]["moveSpeed"]);
        // !!! 공격 범위 추가하기
    }

    // 방어
    public void RecoverHp()
    {
        Heal(_hpRecovery);
    }

    public void Heal(int amount)
    {
        _hp += amount; 
        if (_hp > _maxHp)
        {
            _hp = _maxHp;
        }
    }
    
    public override string ToString()
    {
        // temp
        return 
            "기본 데미지: " + _defaultDamage.ToString() + "\n"
            + "최소 데미지: " + _minDamage.ToString() + "\n"
            + "최대 데미지: " + _maxDamage.ToString() + "\n"
            + "공격속도: " + _attackSpeed.ToString() + "\n"
            + "공격 정확도: " + _accuracy.ToString() + "\n"
            + "명중률: " + _absoluteAccuracy.ToString() + "\n"
            + "치명타 확률: " + _criticalPercent.ToString() + "\n"
            + "체력: " + _maxHp.ToString() + "\n"
            + "Hp 자동 회복: " + _hpRecovery.ToString() + "\n"
            + "데미지 감소: " + _damageReduction.ToString() + "\n"
            + "회피율: " + _evasionPercent.ToString() + "\n"
            + "이동속도: " + _moveSpeed.ToString() + "\n";
    }

    public void Equip(Item item)
    {
        if (item.ItemType == ItemType.Weapon)
        {
            // 무기
            _maxDamage += item.MaxDamage;
            _minDamage += item.MinDamage;
            _defaultDamage += item.DefaultDamage;
            _attackSpeed = (_attackSpeed * 100f - item.AttackSpeed) / 100f;
        }
        else if (item.ItemType == ItemType.Armor)
        {
            // 방어구
            _damageReduction += item.DamageReduction;
            _maxHp += item.MaxHp;
            _hpRecovery += item.HpRecovery;
            _evasionPercent += item.EvasionPercent;
            _criticalPercent += item.CriticalPercent;
            _moveSpeed += item.MoveSpeed;

            if (_hp > _maxHp)
            {
                _hp = _maxHp;
            }
        }
    }

    public void UnEquip(Item item)
    {
        if (item.ItemType == ItemType.Weapon)
        {
            // 무기
            _maxDamage -= item.MaxDamage;
            _minDamage -= item.MinDamage;
            _defaultDamage -= item.DefaultDamage;
            _attackSpeed = (_attackSpeed * 100f + item.AttackSpeed) / 100f;
        }
        else if (item.ItemType == ItemType.Armor)
        {
            // 방어구
            _damageReduction -= item.DamageReduction;
            _maxHp -= item.MaxHp;
            _hpRecovery -= item.HpRecovery;
            _evasionPercent -= item.EvasionPercent;
            _criticalPercent -= item.CriticalPercent;
            _moveSpeed -= item.MoveSpeed;
        }

        Heal(0);    // 장비 착용 해제 후 hp가 maxHp 넘는 경우 maxHp가 되도록 고정
    }
}