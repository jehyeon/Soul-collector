using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : ACharacter
{
    private GameObject player;
    private Vector3 startPos;

    private int findRange = 5;      // 플레이어 감지 범위 (임시)
    private int backRange = 20;     // 스폰 위치로부터의 제한 거리 (임시)

    protected override void Awake()
    {
        base.Awake();
    }

    void Start()
    {
        player = GameObject.FindWithTag("Player");
        startPos = this.transform.position;
    }

    void Update()
    {
        if (player == null)
        {
            // 플레이어가 죽으면 동작 안함
            return;
        }

        // 플레이어가 인식 거리에 있는지 확인
        FindNearbyPlayer();

        // state에 따라 목적지 이동
        Move();

        // 공격 쿨타임 및 공격 사거리에 플레이어가 있는 지 확인
        CheckCanAttack();
        CheckPlayerInAttackRange();

        // 스폰 위치로부터 멀리 떨어졌는지 확인
        CheckFarComeFromStartPos();
    }

    private void FindNearbyPlayer()
    {
        if (state == State.Idle)
        {
            // 되돌아가는 상태가 아닐 때
            if (Vector3.Distance(player.transform.position, this.transform.position) <= findRange)
            {
                destinationPos = player.transform.position;
                state = State.Move;
            }   
        }
    }

    private void CheckPlayerInAttackRange()
    {
        if (Vector3.Distance(player.transform.position, this.transform.position) <= 1.1f)
        {
            // 공격 범위는 고정 (나중에 수정)
            state = State.Attack;
            
            if (canAttack)
            {
                Attack(player);     
            }
        }
        else
        {
            if (state == State.Attack)
            {
                // 멀어졌는데 공격 중이면 Back 전 까지 따라감
                state = State.Move;
                destinationPos = player.transform.position;
            }
        }
    }

    private void CheckFarComeFromStartPos()
    {
        if (Vector3.Distance(startPos, this.transform.position) > backRange)
        {
            destinationPos = startPos;
            state = State.Back;
        }
    }
}
