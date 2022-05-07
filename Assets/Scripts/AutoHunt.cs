using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum AutoHuntMode
{
    GetItem,
    HuntEnemy,
    Find
}

public class AutoHunt : MonoBehaviour
{
    private Player player;
    private PlayerController playerController;

    public AutoHuntMode autoMode;
    private float findEnemyRange = 100f;
    private float findItemRange = 15f;
    private List<Collider> enemies;
    private List<Collider> items;

    private void Start()
    {
        player = GetComponent<Player>();
        playerController = GetComponent<PlayerController>();
        
        autoMode = AutoHuntMode.Find;
    }

    private void Update()
    {
        if (autoMode != AutoHuntMode.Find)
        {
            // 행동이 정해지면 주변 object 검색을 안함
            if (autoMode == AutoHuntMode.GetItem)
            {
                if (items[0].gameObject.activeSelf)
                {
                    return;
                }

                // 아이템을 먹으면
                autoMode = AutoHuntMode.Find;
            }
            else if (autoMode == AutoHuntMode.HuntEnemy)
            {
                if (enemies[0].gameObject.activeSelf)
                {
                    // !!! enemy die animation이 추가되면 die 상태인지 확인해야함
                    return;
                }

                autoMode = AutoHuntMode.Find;
            }
            
            // 자동 사냥 모드를 끄려면 AutoHunt Component를 제거
            return;
        }

        FindNearbyObject();

        if (items.Count > 0 && !player.gameManager.Inventory.isFullInventory())
        {
            // 주변에 아이템이 있는지 확인, 인벤토리에 빈 공간이 있는지 확인
            GetItem();
            return;
        }
        
        if (enemies.Count > 0)
        {
            HuntEnemy();
        }
    }

    private void FindNearbyObject()
    {
        enemies = (from enemy in Physics.OverlapSphere(transform.position, findEnemyRange, 1 << 7)
                    orderby (this.transform.position - enemy.transform.position).sqrMagnitude
                    select enemy).ToList();
        items = (from item in Physics.OverlapSphere(transform.position, findItemRange, 1 << 8)
                    orderby (this.transform.position - item.transform.position).sqrMagnitude
                    select item).ToList();
    }

    private void GetItem()
    {
        player.MoveToItem(items[0].gameObject);
        playerController.ActivateMoveCursor(items[0].transform.position);
        autoMode = AutoHuntMode.GetItem;
    }

    private void HuntEnemy()
    {
        player.AttackTarget(enemies[0].gameObject);
                    
        // 커서 추가
        playerController.ActivateAttackCursor(enemies[0].gameObject.transform);
        playerController.ClearMoveCursor();
        autoMode = AutoHuntMode.HuntEnemy;
    }

    public void SetHuntTarget(Collider enemyCollider)
    {
        enemies[0] = enemyCollider;
        autoMode = AutoHuntMode.HuntEnemy;
    }
}
