using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // 이동, 조작 관련 / 목적지 및 타겟 지정
    Player player;

    private void Start()
    {
        player = gameObject.GetComponent<Player>();
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
                    SetDestination(raycastHit.point + new Vector3(0, 1, 0));
                    player.targetEnemy = null;     // 타겟 삭제
                }

                // 적 클릭하면 target 설정
                if (raycastHit.collider.CompareTag("Enemy"))
                {
                    SetTarget(raycastHit.collider.gameObject);
                }

                if (raycastHit.collider.CompareTag("Item"))
                {
                    GetItem(raycastHit.collider.gameObject);
                }
            }
        }
    }

    private void SetDestination(Vector3 pos)
    {
        // 목적지 지정 state를 Move로 변경
        player.destinationPos = pos;
        player.state = ACharacter.State.Move;
    }

    public void SetTarget(GameObject targetObject)
    {
        // 타겟 설정 및 목적지 지정
        player.targetEnemy = targetObject;
        SetDestination(targetObject.transform.position);
    }

    public void GetItem(GameObject targetObject)
    {
        bool emptyInventory = player.cv.GetComponent<Inventory>().AcquireItem(targetObject.GetComponent<Item>()._id);

        if (emptyInventory)
        {
            Destroy(targetObject);
        }
        else
        {
            Debug.Log("인벤토리 꽉 참");
        }
    }    
}
