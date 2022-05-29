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

    public float DPS { get { return GetDPS(); } }

    public Stat(bool reset = false)
    {
        // 플레이어 초기값
        _criticalPercent = 0;
        _attackSpeed = 1f;
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
        _attackRange = 2f;

        if (reset)
        {
            _attackSpeed = 0;
            _defaultDamage = 0;
            _maxHp = 0;
            _hp = 0;
            _hpRecovery = 0;
            _moveSpeed = 0;
            _attackRange = 0;
        }
    }

    public void SetEnemyStat(
        int minDamage, int maxDamage, int defaultDamage, 
        int maxHp, int damageReduction, 
        float attackSpeed, float moveSpeed, float attackRange)
    {
        // 몹 스탯 Set에 사용
        _minDamage = minDamage;
        _maxDamage = maxDamage;
        _defaultDamage = defaultDamage;
        _maxHp = maxHp;
        _hp = maxHp;    // 현재 체력은 최대 체력과 동일
        _damageReduction = damageReduction;
        _attackSpeed = attackSpeed;
        _moveSpeed = moveSpeed;
        _attackRange = attackRange;
    }

    private float GetDPS()
    {
        float criticalPercent = _criticalPercent * 0.01f;
        float nonCriAvgDamage = (float)((1 - criticalPercent) * (Mathf.FloorToInt((_minDamage + _maxDamage) / 2) + _defaultDamage));
        float criAvgDamage = (float)(criticalPercent * (_maxDamage + _defaultDamage));
        // Debug.Log(nonCriAvgDamage);
        // Debug.Log(criAvgDamage);
        return Mathf.Floor((nonCriAvgDamage + criAvgDamage) * _attackSpeed * 100f) * 0.01f;
    }

    public void DecreaseHp(int damage)
    {
        _hp -= damage;
        if (_hp < 1)
        {
            _hp = 0;
        }
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

    // 공격 관련 메소드
    public int CalculateAttackDamage()
    {
        // 최소 데미지 ~ 최대 데미지 + 기본 데미지
        if (this._criticalPercent > 0)
        {
            float rand = Random.value;

            if (rand < this._criticalPercent * 0.01f)
            {
                // cri effect 추가해야 함
                return _maxDamage + _defaultDamage;
            }
        }

        return Random.Range(_minDamage, _maxDamage + 1) + _defaultDamage;
    }

    // 방어 관련 메소드
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

    public void Heal(bool maxHeal)
    {
        if (maxHeal)
        {
            _hp = _maxHp;
        }
    }

    public int CalculateTakenDamage(int damage)
    {
        return (damage - this._damageReduction) > 0 ? damage - this._damageReduction : 0;
    }

    public int TakeDamage(int damage)
    {
        int real = CalculateTakenDamage(damage);
        DecreaseHp(real);
        return real;
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

    public string ToStringForTooltip()
    {
        string text = "";
        if (_defaultDamage != 0)
        {
            text += "기본 데미지 +" + _defaultDamage.ToString() + "\n";
        }
        if (_attackSpeed != 0)
        {
            text += "공격속도 +" + _attackSpeed.ToString() + "%\n";
        }
        if (_criticalPercent != 0)
        {
            text += "치명타 확률 +" + _criticalPercent.ToString() + "%\n";
        }
        if (_maxHp != 0)
        {
            text += "최대 체력 +" + _maxHp.ToString() + "\n";
        }
        if (_hpRecovery != 0)
        {
            text += "Hp 자동 회복 +" + _hpRecovery.ToString() + "\n";
        }
        if (_damageReduction != 0)
        {
            text += "데미지 감소 +" + _damageReduction.ToString() + "\n";
        }

        return text;
    }

    public string GetAtkStatDes()
    {
        return 
            "기본 데미지: " + _defaultDamage.ToString() + "\n"
            + "최소 데미지: " + _minDamage.ToString() + "\n"
            + "최대 데미지: " + _maxDamage.ToString() + "\n"
            + "공격속도: " + _attackSpeed.ToString() + "\n"
            + "공격 정확도: " + _accuracy.ToString() + "\n"
            + "명중률: " + _absoluteAccuracy.ToString() + "\n"
            + "치명타 확률: " + _criticalPercent.ToString() + "\n";
    }

    public string GetDefStatDes()
    {
        return 
            "체력: " + _maxHp.ToString() + "\n"
            + "Hp 자동 회복: " + _hpRecovery.ToString() + "\n"
            + "데미지 감소: " + _damageReduction.ToString() + "\n"
            + "회피율: " + _evasionPercent.ToString() + "\n"
            + "이동속도: " + _moveSpeed.ToString() + "\n";
    }
    // -------------------------------------------------------------
    // for Collection
    // -------------------------------------------------------------
    public void SumForCollect(int collectionIndex, int amount)
    {
        // !!! not good
        switch (collectionIndex)
        {
            case 0:
                _defaultDamage += amount;
                break;
            case 1:
                _criticalPercent += (float)amount;
                break;
            case 2:
                _attackSpeed += (float)amount;
                break;
            case 3:
                _damageReduction += amount;
                break;
            case 4:
                _hpRecovery += amount;
                break;
            case 5:
                _maxHp += amount;
                break;
            case 6:
                _moveSpeed += amount;
                break;
        }
    }
    // -------------------------------------------------------------
    // 버프로 인한 스탯 변화 (Item, Skill)
    // 1. 스탯이 바뀌는 패시브 스킬
    // -------------------------------------------------------------
    public void ActivateBuff(Buff buff)
    {
        _defaultDamage += buff.Stat.DefaultDamage;
        _attackSpeed += buff.Stat.AttackSpeed * 0.01f;
        _criticalPercent += buff.Stat.CriticalPercent;
        _maxHp += buff.Stat.MaxHp;
        _damageReduction += buff.Stat.DamageReduction;
        _hpRecovery += buff.Stat.HpRecovery;
        _moveSpeed += buff.Stat.MoveSpeed;

        if (_hp > _maxHp)
        {
            _hp = _maxHp;
        }
    }

    public void DeActivateBuff(Buff buff, bool hpContinue = false)
    {
        _defaultDamage -= buff.Stat.DefaultDamage;
        _attackSpeed -= buff.Stat.AttackSpeed * 0.01f;
        _criticalPercent -= buff.Stat.CriticalPercent;
        _maxHp -= buff.Stat.MaxHp;
        _hpRecovery -= buff.Stat.HpRecovery;
        _moveSpeed -= buff.Stat.MoveSpeed;

        if (_hp > _maxHp && !hpContinue)
        {
            _hp = _maxHp;
        }
    }

    // -------------------------------------------------------------
    // 장착으로 인한 스탯 변화
    // -------------------------------------------------------------
    public void Equip(Item item)
    {
        if (item.ItemType != ItemType.Weapon && item.ItemType != ItemType.Armor)
        {
            // 장착 아이템이 아닌 경우 스탯 변화 없음
            return;
        }

        _maxDamage += item.MaxDamage;
        _minDamage += item.MinDamage;
        _defaultDamage += item.DefaultDamage;
        _attackSpeed += item.AttackSpeed * .01f;
        _damageReduction += item.DamageReduction;
        _maxHp += item.MaxHp;
        _hpRecovery += item.HpRecovery;
        _evasionPercent += item.EvasionPercent;
        _criticalPercent += item.CriticalPercent;
        _moveSpeed += item.MoveSpeed * .05f;

        if (_hp > _maxHp)
        {
            _hp = _maxHp;
        }
    }

    public void UnEquip(Item item)
    {
        if (item.ItemType != ItemType.Weapon && item.ItemType != ItemType.Armor)
        {
            // 장착 아이템이 아닌 경우 스탯 변화 없음
            return;
        }

        // 무기
        _maxDamage -= item.MaxDamage;
        _minDamage -= item.MinDamage;
        _defaultDamage -= item.DefaultDamage;
        _attackSpeed -= item.AttackSpeed * .01f;
        // 방어구
        _damageReduction -= item.DamageReduction;
        _maxHp -= item.MaxHp;
        _hpRecovery -= item.HpRecovery;
        _evasionPercent -= item.EvasionPercent;
        _criticalPercent -= item.CriticalPercent;
        _moveSpeed -= item.MoveSpeed * 0.05f;

        Heal(0);    // 장비 착용 해제 후 hp가 maxHp 넘는 경우 maxHp가 되도록 고정
    }
}