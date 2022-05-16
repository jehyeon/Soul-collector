using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Village : MonoBehaviour
{
    void Start()
    {
        // 마을에서 던전 가는 포탈 재 생성
        PortalSystem.Instance.CreatePortal(new Vector3(10f, 2f, 0), PortalType.GoDungeon);
        
        // Player reset
        Player player = GameObject.Find("Player").GetComponent<Player>();
        NavMeshAgent agent = player.GetComponent<NavMeshAgent>();
        player.IdleMode();
        agent.enabled = false;
        player.transform.position = Vector3.zero;
        agent.enabled = true;
        player.Heal(true);     // 마을 복귀 시 최대 체력

        // 마을 복귀시 몬스터 체력바 삭제
        GameObject EnemyBarSystem = GameObject.Find("Enemy Hp bar");
        if (EnemyBarSystem.transform.childCount > 0)
        {
            EnemyHpBar[] childList = EnemyBarSystem.GetComponentsInChildren<EnemyHpBar>();
            for (int i = 0; i < childList.Length; i++)
            {
                Destroy(childList[i].gameObject);
            }
        }
    }
}
