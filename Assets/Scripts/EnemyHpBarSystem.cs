using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHpBarSystem : MonoBehaviour
{
    private GameObject hpBarParent;
    [SerializeField]
    private ObjectPool EnemyHpBarOP;

    private Vector3 offset = new Vector3(0, 2.5f, 0);

    public EnemyHpBar InitHpBar(Vector3 initPos)
    {
        EnemyHpBar hpBar = EnemyHpBarOP.Get().GetComponent<EnemyHpBar>();

        // parent system(this) 및 object(enemy) 설정
        hpBar.Set(this, offset);
        hpBar.transform.position = Camera.main.WorldToScreenPoint(initPos + offset);    // 초기 위치 설정
        if (hpBarParent == null)
        {
            hpBarParent = GameObject.Find("UI Controller").GetComponent<UIController>().EnemyHpBarParent;
        }
        hpBar.transform.SetParent(hpBarParent.transform);
        return hpBar;
    }

    public void Return(EnemyHpBar hpBar)
    {
        hpBar.UpdateHpBar(1f);       // 초기화
        EnemyHpBarOP.Return(hpBar.gameObject);
    }
}
