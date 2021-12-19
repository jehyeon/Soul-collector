using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float playerSpeed = 10f;
    public float hp = 100f;
    
    private bool isMove;
    private Vector3 destinationPos;
    
    private bool isTarget;
    private GameObject targetEnemy;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
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

        if (isTarget)
        {
            Attack();
        }
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
            targetEnemy.GetComponent<EnemyController>().Hit(Time.deltaTime);
        }
    }

    public void Kill()
    {
        isTarget = false;
        targetEnemy = null;
    }

    public void Hit(float damage)
    {
        hp -= damage;
    }
}
