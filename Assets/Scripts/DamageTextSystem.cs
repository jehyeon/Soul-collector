using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTextSystem : MonoBehaviour
{
    private GameObject damageTextParent;
    [SerializeField]
    private ObjectPool damageTextOP;

    [SerializeField]
    private Vector3 dir;
    [SerializeField]
    private Vector3 offset;
    [SerializeField]
    private float alphaSpeed;

    public void FloatDamageText(int finalDamage, Vector3 pos)
    {
        GameObject damageText = damageTextOP.Get();
        if (damageTextParent == null)
        {
            damageTextParent = GameObject.Find("UI Controller").GetComponent<UIController>().DamageTextParent;
        }
        damageText.transform.SetParent(damageTextParent.transform);
        damageText.GetComponent<DamageText>().Set(damageTextOP, finalDamage, pos + offset, dir, alphaSpeed);
    }
}
