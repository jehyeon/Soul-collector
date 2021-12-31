using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stat : MonoBehaviour
{
    // 공격
    protected int _criticalPercent;     // 치명타 확률
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

    public int CriticalPercent { get { return _criticalPercent; }}
    public float AttackSpeed { get { return _attackSpeed; }}
    public int MaxDamage { get { return _maxDamage; }}
    public int MinDamage { get { return _minDamage; }}
    public int DefaultDamage { get { return _defaultDamage; }}
    public int Accuracy { get { return _accuracy; }}
    public int AbsoluteAccuracy { get { return _absoluteAccuracy; }}
    public int MaxHp { get { return _maxHp; }}
    public int Hp { get { return _hp; }}
    public int HpRecovery { get { return _hpRecovery; }}
    public int DamageReduction { get { return _damageReduction; }}
    public int EvasionPercent { get { return _evasionPercent; }}
    public float MoveSpeed { get { return _moveSpeed; }}
    private void Awake()
    {
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
        _moveSpeed = 10f;
    }

    public void Attacked(int damage)
    {
        _hp -= damage;
    }

    public void LoadFromCSV(int id, string fileName)
    {
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
    }

    // 방어
    public void RecoveryHp()
    {
        _hp += _hpRecovery;
    }

    public override string ToString()
    {
        return 
            "치명타 확률: " + _criticalPercent.ToString() + "\n"
            + "공격속도: " + _attackSpeed.ToString() + "\n"
            + "최대 가변 데미지: " + _maxDamage.ToString() + "\n"
            + "최소 가변 데미지: " + _minDamage.ToString() + "\n"
            + "기본 데미지: " + _defaultDamage.ToString() + "\n"
            + "공격 정확도: " + _accuracy.ToString() + "\n"
            + "체력: " + _hp.ToString() + "\n"
            + "Hp 자동 회복: " + _hpRecovery.ToString() + "\n"
            + "데미지 감소: " + _damageReduction.ToString() + "\n"
            + "회피율: " + _evasionPercent.ToString() + "\n"
            + "이동속도: " + _moveSpeed.ToString() + "\n";
    }

    public void Equip(Item item)
    {
        if (item.ItemType == 1 || item.ItemType == 2 || item.ItemType == 3)
        {
            // 무기
            _maxDamage += item.MaxDamage;
            _minDamage += item.MinDamage;
            _defaultDamage += item.DefaultDamage;
            _attackSpeed = (_attackSpeed * 100f - item.AttackSpeed) / 100f;
        }
    }

    public void UnEquip(Item item)
    {
        if (item.ItemType == 1 || item.ItemType == 2 || item.ItemType == 3)
        {
            // 무기
            _maxDamage -= item.MaxDamage;
            _minDamage -= item.MinDamage;
            _defaultDamage -= item.DefaultDamage;
            _attackSpeed = (_attackSpeed * 100f + item.AttackSpeed) / 100f;
        }
    }
}