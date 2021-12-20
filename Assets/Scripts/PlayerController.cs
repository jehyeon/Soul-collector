using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float playerSpeed = 10f;
    public float maxHp = 100f;
    public float nowHp;
    public float attackSpeed = 2f;
    public float damage = 5f;
    public float timeAfterAttack;

    private bool isMove;
    private Vector3 destinationPos;
    
    private bool isTarget;
    private GameObject targetEnemy;

    public Slider hpBar;
    
    void Start()
    {
        hpBar.maxValue = maxHp;
        nowHp = maxHp;
        timeAfterAttack = attackSpeed;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject ())
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit raycastHit))
            {
                // 땅 클릭하면 그 위치로 이동
                if (raycastHit.collider.CompareTag("Ground"))
                {
                    SetDestination(raycastHit.point + new Vector3(0, 1, 0));
                }

                // 적 클릭하면 target 설정
                if (raycastHit.collider.CompareTag("Enemy"))
                {
                    targetEnemy = raycastHit.collider.gameObject;
                    isTarget = true;
                }
            }
        }

        Move();

        timeAfterAttack += Time.deltaTime;

        if (isTarget)
        {
            if (timeAfterAttack > attackSpeed)
            {
                Attack();
                timeAfterAttack = 0f;
            }
        }

        UpdateHpBar();
    }

    private void Move()
    {
        if (isMove)
        {
            if (Vector3.Distance(destinationPos, this.transform.position) <= 0.1f)
            {
                // 목적지에 플레이어가 도착하면
                isMove = false;
                return;
            }
            
            Vector3 dir = destinationPos - this.transform.position;

            this.transform.position += dir.normalized * Time.deltaTime * playerSpeed;
        }
    }

    private void SetDestination(Vector3 pos)
    {
        destinationPos = pos;
        isMove = true;
    }

    private void Attack()
    {
        if (Vector3.Distance(targetEnemy.transform.position, this.transform.position) <= 1.1f)
        {
            targetEnemy.GetComponent<EnemyController>().Hit(damage);
        }
    }

    public void Kill()
    {
        isTarget = false;
        targetEnemy = null;
    }

    public void Hit(float damage)
    {
        nowHp -= damage;
    }

    private void UpdateHpBar()
    {
        hpBar.value = nowHp;
    }
}
