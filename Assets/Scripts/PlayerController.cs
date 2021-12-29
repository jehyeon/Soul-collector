using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float maxHp = 100f;
    public float nowHp;
    public float attackSpeed = 1f;
    public float damage = 5f;
    public float attackCoolTime;
    PlayerStat _stat;

    private bool isMove;
    private Vector3 destinationPos;
    
    public bool isTarget;
    private GameObject targetEnemy;

    public Slider hpBar;
    
    [SerializeField]
    private Canvas cv;

    void Start()
    {
        _stat = gameObject.GetComponent<PlayerStat>();

        hpBar.maxValue = maxHp;
        nowHp = maxHp;
        attackCoolTime = attackSpeed;
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
                    SetTarget(raycastHit.collider.gameObject);
                }

                if (raycastHit.collider.CompareTag("Item"))
                {
                    GetItem(raycastHit.collider.gameObject);
                }
            }
        }

        Move();

        attackCoolTime += Time.deltaTime;

        if (isTarget)
        {
            if (CheckEnemyInAttackRange())
            {
                if (attackCoolTime > attackSpeed)
                {
                    Attack();
                    attackCoolTime = 0f;
                }
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

            this.transform.position += dir.normalized * Time.deltaTime * _stat.MoveSpeed;
        }
    }

    private void SetDestination(Vector3 pos)
    {
        destinationPos = pos;
        isMove = true;
    }

    public void SetTarget(GameObject targetObject)
    {
        targetEnemy = targetObject;
        SetDestination(targetObject.transform.position);
        isTarget = true;
    }

    private bool CheckEnemyInAttackRange()
    {
        if (Vector3.Distance(targetEnemy.transform.position, this.transform.position) <= 1.1f)
        {
            isMove = false;
            return true;
        }
        
        return false;
    }

    private void Attack()
    {
        isMove = false;
        targetEnemy.GetComponent<EnemyController>().Hit(damage);
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

    public void GetItem(GameObject _targetObject)
    {
        bool emptyInventory = cv.GetComponent<Inventory>().AcquireItem(_targetObject);

        if (emptyInventory)
        {
            Destroy(_targetObject);
        }
        else
        {
            Debug.Log("인벤토리 꽉 참");
        }
    }
}
