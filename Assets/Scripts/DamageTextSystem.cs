using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTextSystem : MonoBehaviour
{
    [SerializeField]
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
        damageText.transform.SetParent(damageTextParent.transform);
        damageText.GetComponent<DamageText>().Set(damageTextOP, finalDamage, pos + offset, dir, alphaSpeed);
    }
}