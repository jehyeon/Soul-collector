using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerController : MonoBehaviour
{
    // 이동, 조작 관련 / 목적지 및 타겟 지정
    private Player player;

    // Target Marker
    [SerializeField]
    private GameObject targetParnet;
    [SerializeField]
    private GameObject moveTargetPref;
    [SerializeField]
    private GameObject attackTargetPref;
    private GameObject moveTarget;
    private GameObject attackTarget;
    
    [SerializeField]
    private List<Collider> enemies;

    private float getItemRange = 2f;
    public float GetItemRange { get { return getItemRange; } }
    private float findEnemyRange = 15f;
    private int maxTargetLength = 5;

    private void Start()
    {
        player = GetComponent<Player>();
        InvokeRepeating("FindNearbyEnemy", 0f, 1f);     // 1초 마다 주변 enemy 갱신
    }

    private void Update()
    {
        TouchAction();
        KeyBoardAction();
    }

    // -------------------------------------------------------------
    // 플레이어 조작
    // -------------------------------------------------------------
    private void TouchAction()
    {
        if (Input.GetMouseButtonDown(0) && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject ())
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit raycastHit))
            {
                // 땅 클릭하면 그 위치로 이동
                if (raycastHit.collider.CompareTag("Ground"))
                {
                    player.Move(raycastHit.point);
                    ActivateMoveCursor(raycastHit.point);
                }

                // 적 클릭하면 target 설정
                if (raycastHit.collider.CompareTag("Enemy"))
                {
                    player.AttackTarget(raycastHit.collider.gameObject);
                    
                    // 커서 추가
                    ActivateAttackCursor(raycastHit.collider.gameObject.transform);
                    ClearMoveCursor();
                }

                if (raycastHit.collider.CompareTag("Item"))
                {
                    player.MoveToItem(raycastHit.collider.gameObject);
                    ActivateMoveCursor(raycastHit.collider.transform.position); // 커서 추가
                }
            }
        }
    }

    private void KeyBoardAction()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        
        if (moveX != 0 || moveZ != 0)
        {
            // 키보드 입력이 있는 경우
            player.Move(this.transform.position + new Vector3(moveX, 0, moveZ).normalized, true);
            ClearMoveCursor();  // 키보드 이동 시 이동 타겟 클리어
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            // 타겟 방향으로 이동
            player.AttackTarget();
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            // 타겟 전환
            ChangeTarget();
        }
    }

    // -------------------------------------------------------------
    // 타겟
    // -------------------------------------------------------------
    private void ActivateMoveCursor(Vector3 point)
    {
        if (moveTarget == null)
        {
            moveTarget = Instantiate(moveTargetPref);
            moveTarget.transform.parent = targetParnet.transform;
        }

        moveTarget.SetActive(true);
        moveTarget.transform.position = point;
    }

    private void ActivateAttackCursor(Transform transform)
    {
        if (attackTarget == null)
        {
            attackTarget = Instantiate(attackTargetPref);
        }
        ClearMoveCursor();
        attackTarget.SetActive(true);
        attackTarget.transform.parent = transform;
        attackTarget.transform.localPosition = Vector3.zero;
    }

    public void ClearMoveCursor()
    {
        if (moveTarget == null)
        {
            moveTarget = Instantiate(moveTargetPref);
            moveTarget.transform.parent = targetParnet.transform;
        }
        moveTarget.SetActive(false);
    }

    public void ClearAttackCursor()
    {
        if (attackTarget == null)
        {
            attackTarget = Instantiate(attackTargetPref);
        }
        attackTarget.transform.parent = targetParnet.transform;
        attackTarget.SetActive(false);
    }

    private void FindNearbyEnemy()
    {
        // findEnemyRage 내의 enemy layer object collider를 
        // enemies array에 거리순으로 정렬하여 저장
        var query = from enemy in Physics.OverlapSphere(transform.position, findEnemyRange, 1 << 7)
                    orderby (this.transform.position - enemy.transform.position).sqrMagnitude
                    select enemy;
        enemies = query.ToList();

        if (enemies.Count == 0)
        {
            // 주변에 enemy가 없는 경우 기존 타겟 해제
            // player.SetTarget(null);
            ClearAttackCursor();
        }
        else if (enemies.Count > maxTargetLength)
        {
            // 타겟 list 크기 한정
            enemies = enemies.GetRange(0, maxTargetLength);
        }
    }

    private void ChangeTarget()
    {
        // target이 없는 경우 가장 가까운 enemy를 target 지정
        // 거리 순으로 다음 타겟 전환
        if (enemies.Count == 0)
        {
            // 주변에 enemy가 없는 경우
            return;
        }

        if (player.Target == null)
        {
            // 이미 지정한 타겟이 없는 경우
            if (enemies.Count > 0)
            {
                // 근처 enemy가 있을 때 가장 가까운 enemy를 target으로 설정
                player.SetTarget(enemies[0].gameObject);
                ActivateAttackCursor(enemies[0].transform);
            }
        }
        else
        {
            if (enemies.Count == 1)
            {
                if (System.Object.ReferenceEquals(enemies[0].gameObject, player.Target.gameObject))
                {
                    // 주변 Enemy가 Target 밖에 없는 경우
                    player.SetTarget(null);
                    ClearAttackCursor();
                }
                else
                {
                    player.SetTarget(enemies[0].gameObject);
                    ActivateAttackCursor(enemies[0].transform);
                }
            }
            else if (enemies.Count > 1)
            {
                // 현재 target이 몇 번째로 가까운 지 확인
                int index = enemies.FindIndex(enemy => System.Object.ReferenceEquals(player.Target, enemy.gameObject));
                if (index + 1 >= enemies.Count)
                {
                    // 마지막 index의 enemy가 타겟인 경우
                    player.SetTarget(enemies[0].gameObject);
                    ActivateAttackCursor(enemies[0].transform);
                }
                else
                {
                    // 다음 index 타겟으로 설정
                    player.SetTarget(enemies[index + 1].gameObject);
                    ActivateAttackCursor(enemies[index + 1].transform);
                }
            }
        }
    }
}