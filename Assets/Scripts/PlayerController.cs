using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class PlayerController : MonoBehaviour
{
    // 이동, 조작 관련 / 목적지 및 타겟 지정
    private Player player;
    private AutoHunt auto;
    // Auto hunt mark
    [SerializeField]
    private Animator autoHuntMarkAnimator;
    [SerializeField]
    private TextMeshProUGUI autoHuntMarkText;
    [SerializeField]
    private GameObject autoHuntForBackground;

    // Target Marker
    [SerializeField]
    private GameObject targetParnet;
    [SerializeField]
    private GameObject attackTargetPref;
    private GameObject attackTarget;
    
    [SerializeField]
    private List<Collider> enemies;

    public List<Collider> CloseEnemies;

    private float getItemRange = 2f;
    public float GetItemRange { get { return getItemRange; } }
    private float findEnemyRange = 15f;
    private int maxTargetLength = 5;

    // 자동 사냥 모드
    private bool isAuto;

    private void Start()
    {
        player = GetComponent<Player>();
        isAuto = false;
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
            if (isAuto)
            {
                // 자동 사냥 중이면 자동 사냥 종료
                StopAutoHunt();
            }
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit raycastHit))
            {
                // 땅 클릭하면 그 위치로 이동
                if (raycastHit.collider.CompareTag("Ground"))
                {
                    player.Move(raycastHit.point);
                    // ActivateMoveCursor(raycastHit.point);
                    MovePoint.Instance.Activate(raycastHit.point);
                    return;
                }

                // 적 클릭하면 target 설정
                if (raycastHit.collider.CompareTag("Enemy"))
                {
                    player.AttackTarget(raycastHit.collider.gameObject);
                    
                    // 커서 추가
                    ActivateAttackCursor(raycastHit.collider.gameObject.transform);
                    return;
                }

                if (raycastHit.collider.CompareTag("Item"))
                {
                    player.MoveToItem(raycastHit.collider.gameObject);
                    MovePoint.Instance.Activate(raycastHit.collider.transform.position);
                    return;
                }

                if (raycastHit.collider.CompareTag("Portal"))
                {
                    raycastHit.collider.gameObject.GetComponent<Portal>().Enter();
                    return;
                }
                
                // if (raycastHit.collider.CompareTag("NPC"))
                // {
                //     raycastHit.collider.gameObject.GetComponent<NPC>().Talk();
                //     return;
                // }
            }
        }
    }

    private void KeyBoardAction()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            player.gameManager.GoViliage();
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            player.gameManager.GoDungeon(5);
        }
        
        // 자동 사냥 모드
        if (Input.GetKeyDown(KeyCode.G))
        {
            if (isAuto)
            {
                StopAutoHunt();
            }
            else
            {
                StartAutoHunt();
            }

            return;
        }

        // if (isAuto && Input.anyKeyDown)
        // {
        //     // 자동 사냥 중일 때 키보드 입력이 생기면 자동 사냥 종료
        //     StopAutoHunt();

        //     return;
        // }

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        
        if (moveX != 0 || moveZ != 0)
        {
            if (isAuto)
            {
                StopAutoHunt();
            }
            // 키보드 입력이 있는 경우
            player.Move(this.transform.position + new Vector3(moveX, 0, moveZ).normalized, true);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isAuto)
            {
                StopAutoHunt();
            }
            // 타겟 방향으로 이동
            player.AttackTarget();
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (isAuto)
            {
                StopAutoHunt();
            }
            // 타겟 전환
            ChangeTarget();
        }
    }

    // -------------------------------------------------------------
    // 타겟
    // -------------------------------------------------------------
    public void ActivateAttackCursor(Transform transform)
    {
        if (attackTarget == null)
        {
            attackTarget = Instantiate(attackTargetPref);
        }
        attackTarget.SetActive(true);
        attackTarget.transform.parent = transform;
        attackTarget.transform.localPosition = Vector3.zero;
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

    // -------------------------------------------------------------
    // 자동 사냥 모드 지원
    // -------------------------------------------------------------
    private void StartAutoHunt()
    {
        if (player.gameManager.Floor == 0)
        {
            // 마을에서는 자동 사냥이 안됨
            return;
        }
        // 이동 및 타겟, 커서 초기화
        ClearAttackCursor();
        player.SetTarget(null);
        player.StopMove();

        // 적 타겟 검색 종료 AutoHunt에서 처리함
        CancelInvoke("FindNearbyEnemy");
    
        // AutoHunt 추가
        isAuto = true;
        this.gameObject.AddComponent<AutoHunt>();   
        auto = GetComponent<AutoHunt>();

        // player.gameManager.PopupMessage("자동사냥 중입니다.\n아무 키를 누르면 종료 됩니다.", float.PositiveInfinity);
        autoHuntMarkText.text = "Auto";
        autoHuntMarkText.color = new Color(1f, 1f, 1f, 1f);
        autoHuntMarkAnimator.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
        // autoHuntMarkAnimator.Play("AutoHuntAnimation");
        autoHuntMarkAnimator.enabled = true;
        autoHuntForBackground.SetActive(true);
    }

    private void StopAutoHunt()
    {
        // 이동 및 타겟, 커서 초기화
        ClearAttackCursor();
        player.SetTarget(null);
        player.StopMove();

        // AutoHunt 삭제
        isAuto = false;
        Destroy(GetComponent<AutoHunt>());
        auto = null;

        // 주변 적 타겟 검색 재 시작
        InvokeRepeating("FindNearbyEnemy", 0f, 1f);

        // player.gameManager.PopupMessageClose();
        autoHuntMarkText.text = "Stop";
        autoHuntMarkText.color = new Color(1f, 1f, 1f, 0.5f);
        autoHuntMarkAnimator.GetComponent<Image>().color = new Color(1f, 1f, 1f, .5f);
        autoHuntMarkAnimator.enabled = false;
        autoHuntForBackground.SetActive(false);
    }
}