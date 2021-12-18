using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float playerSpeed = 10f;
    
    private bool isMove;
    private Vector3 destinationPos;
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit raycastHit))
            {
                if (raycastHit.collider.CompareTag("Ground"))
                {
                    SetDestination(raycastHit.point + new Vector3(0, 1, 0));
                }
            }
        }

        Move();
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
}
