using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHpBar : MonoBehaviour
{
    [SerializeField]
    private Slider slider;
    private EnemyHpBarSystem parentSystem;
    private Transform enemyTransform;
    private Vector3 offset;

    private void Update()
    {
        if (enemyTransform != null)
        {
            this.transform.position = Camera.main.WorldToScreenPoint(enemyTransform.position + offset);
        }
    }

    public void SetTransform(Transform transform)
    {
        enemyTransform = transform;
    }
    
    public void Set(EnemyHpBarSystem parent, Vector3 _offset)
    {
        offset = _offset;
        parentSystem = parent;
    }

    public void UpdateHpBar(float hpRatio)
    {
        slider.value = hpRatio;
    }

    public void Return()
    {
        parentSystem.Return(this);
    }
}
