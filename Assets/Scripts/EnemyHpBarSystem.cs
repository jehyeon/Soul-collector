using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHpBarSystem : MonoBehaviour
{
    [SerializeField]
    private GameObject hpBarParent;
    [SerializeField]
    private ObjectPool EnemyHpBarOP;

    public EnemyHpBar InitHpBar()
    {
        EnemyHpBar hpBar = EnemyHpBarOP.Get().GetComponent<EnemyHpBar>();
        // parent system(this) 및 object(enemy) 설정
        hpBar.SetParentSystem(this);
        hpBar.transform.SetParent(hpBarParent.transform);
        return hpBar;
    }

    public void Return(EnemyHpBar hpBar)
    {
        hpBar.UpdateHpBar(1f);       // 초기화
        EnemyHpBarOP.Return(hpBar.gameObject);
    }
}
