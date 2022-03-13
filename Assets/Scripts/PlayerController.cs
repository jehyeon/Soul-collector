using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // 이동, 조작 관련 / 목적지 및 타겟 지정
    private Player player;
    
    private void Start()
    {
        player = GetComponent<Player>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject ())
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit raycastHit))
            {
                // 땅 클릭하면 그 위치로 이동
                if (raycastHit.collider.CompareTag("Ground"))
                {
                    player.SetDestination(raycastHit.point + new Vector3(0, 1, 0));
                }

                // 적 클릭하면 target 설정
                if (raycastHit.collider.CompareTag("Enemy"))
                {
                    player.SetTarget(raycastHit.collider.gameObject);
                }

                if (raycastHit.collider.CompareTag("Item"))
                {
                    player.gameManager.GetItem(raycastHit.collider.gameObject);
                }
            }
        }
    }
}