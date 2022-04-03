using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : ACharacter
{
    private Spawner spawner;
    private int enemyId;
    protected int dropId;           // 자식 클래스에서 재 할당
    protected Vector3 startPos;

    private int findRange = 5;      // 플레이어 감지 범위 (default)
    private int backRange = 15;     // 스폰 위치로부터의 제한 거리 (default)

    private bool isWakeUp = false;

    public Spawner ParentSpawner { get { return spawner; } }
    public int DropId { get { return dropId; } }

    // Hp bar
    private EnemyHpBar hpBar;

    public void Set(Spawner parentSpawner, GameObject target)
    {
        stat.Heal(99999);
        spawner = parentSpawner;
        startPos = this.transform.position;
        SetTarget(target);
    }

    protected void FindNearByPlayer()
    {
        if (state == State.Attack || state == State.Back)
        {
            // 공격 중이거나 되돌아가는 상태일 때에는 타겟을 찾지 않음
            return;
        }
        
        if (!isWakeUp && targetDir.sqrMagnitude < Mathf.Pow(findRange, 2))
        {
            // isWakeUp: false 일 때 findRange 안에만 들어오면 움직이기 시작
            isWakeUp = true;
            SetDestination(target.transform.position);
            return;
        }

        if (isWakeUp && targetDir.sqrMagnitude > Mathf.Pow(stat.AttackRange, 2))
        {
            // isWakeUp: true인 경우 공격 사거리 밖일 때만 플레이어에게 이동
            SetDestination(target.transform.position);
        }
    }

    protected void Back()
    {
        // startPos에서 멀어지면 최대 체력이 되고 처음 위치로 돌아감
        Vector3 outDistance = this.transform.position - startPos;

        if (outDistance.sqrMagnitude > Mathf.Pow(backRange, 2) && state != State.Back)
        {
            // 돌아가는 상태가 아닌데 시작 지점에서 멀어진 경우
            SetDestination(startPos, true);     // state 변경 및 시작 위치로 이동 state = Back
            isWakeUp = false;
        }

        if (state == State.Back)
        {
            // 돌아가는 상태
            stat.Heal(1);     // 호출 주기마다 1씩 체력 회복
            UpdateHpBar();

            if (outDistance.sqrMagnitude < 0.05f)
            {
                // 시작 지점으로 돌아오면
                state = State.Idle;
                hpBar.Return();
                hpBar = null;
            }
        }
    }

    protected override void Die()
    {
        hpBar.Return();
        hpBar = null;
        spawner.Die(this);
    }

    public virtual void Reset()
    {
        isWakeUp = false;
        // Debug.LogError("Reset: 자식 클래스에서 오버라이드 되지 않음");
    }

    protected override void UpdateHpBar()
    {
        if (hpBar == null)
        {
            // init
            hpBar = spawner.GameManager.EnemyHpBarSystem.InitHpBar();
            hpBar.SetTransform(this.transform);
        }

        // update
        hpBar.UpdateHpBar((float)stat.Hp / stat.MaxHp);
    }
}
