using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stat : MonoBehaviour
{
    // 공격
    protected int _criticalPercent;     // 치명타 확률
    protected float _attackSpeed;       // 공격 속도
    protected int _maxDamage;           // 최대 가변 데미지
    protected int _minDamage;           // 최소 가변 데미지
    protected int _defaultDamage;       // 기본 데미지
    protected int _accuracy;      // 공격 정확도

    // 방어
    protected int _hp;                  // 체력
    protected int _damageReduction;     // 데미지 감소
    protected int _evasionPercent;      // 회피율

    // 기타
    protected float _moveSpeed;        // 이동 속도

    public int CriticalPercent { get { return _criticalPercent; }}
    public float AttackSpeed { get { return _attackSpeed; }}
    public int MaxDamage { get { return _maxDamage; }}
    public int MinDamage { get { return _minDamage; }}
    public int DefaultDamage { get { return _defaultDamage; }}
    public int Accuracy { get { return _accuracy; }}
    public int Hp { get { return _hp; }}
    public int DamageReduction { get { return _damageReduction; }}
    public int EvasionPercent { get { return _evasionPercent; }}
    public float MoveSpeed { get { return _moveSpeed; }}
    private void Start()
    {
        _criticalPercent = 0;
        _attackSpeed = 1;
        _maxDamage = 0;
        _minDamage = 0;
        _defaultDamage = 1;
        _accuracy = 1;
        _hp = 100;
        _damageReduction = 0;
        _evasionPercent = 0;
        _moveSpeed = 10f;
    }

    public void Attacked(int damage)
    {
        _hp -= damage;
    }
}
