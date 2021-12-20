using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float enemySpeed = 8f;
    public float hp = 100f;
    public float attackSpeed = 2f;
    public float timeAfterAttack;
    public float damage = 5f;
    
    private GameObject player;
    private Vector3 startPos;
    private bool isMove;
    private bool isBack;
    private bool isAttack;

    void Start()
    {
        player = GameObject.FindWithTag("Player");
        
        startPos = this.transform.position;

        isMove = false;
        isBack = false;

        timeAfterAttack = attackSpeed;
    }

    void Update()
    {
        FindNearbyPlayer();
        Move();

        timeAfterAttack += Time.deltaTime;
        if (isAttack)
        {
            if (timeAfterAttack > attackSpeed)
            {
                Attack();
                timeAfterAttack = 0f;
            }
        }
    }

    private void Move()
    {
        if (isMove)
        {
            if (Vector3.Distance(player.transform.position, this.transform.position) <= 1.1f)
            {
                // 목적지에 플레이어가 도착하면
                isMove = false;
                isAttack = true;
                return;
            } else
            {
                isAttack = false;
            }
            
            Vector3 dir = player.transform.position - this.transform.position;

            this.transform.position += dir.normalized * Time.deltaTime * enemySpeed;
            
            CheckFarComeFromStartPos();
        }
        if (isBack)
        {
            if (Vector3.Distance(startPos, this.transform.position) <= .1f)
            {
                // 목적지에 플레이어가 도착하면
                isBack = false;
                return;
            }

            Vector3 dir = startPos - this.transform.position;

            this.transform.position += dir.normalized * Time.deltaTime * (enemySpeed * 1.5f);
        }
    }

    private void FindNearbyPlayer()
    {
        if (!isBack)
        {
            if (Vector3.Distance(player.transform.position, this.transform.position) <= 5f)
            {
                isMove = true;
                return;
            }   
        }
    }

    private void CheckFarComeFromStartPos()
    {
        if (Vector3.Distance(startPos, this.transform.position) > 20f)
        {
            isBack = true;
            isMove = false;
        }
    }

    public void Hit(float damage)
    {
        hp -= damage;

        if (hp < 0f)
        {
            Destroy(gameObject);
            player.GetComponent<PlayerController>().Kill();
        }
    }

    private void Attack()
    {
        player.GetComponent<PlayerController>().Hit(damage);
    }
}
