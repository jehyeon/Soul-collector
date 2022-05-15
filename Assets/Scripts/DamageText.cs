using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI text;

    private float alphaSpeed;
    private Vector3 dir;

    private Color alpha;
    private ObjectPool objectPool;

    private void Start()
    {
        alpha = text.color;
    }

    private void Update()
    {
        this.transform.Translate(dir * Time.deltaTime);
        alpha.a = Mathf.Lerp(alpha.a, 0, Time.deltaTime * alphaSpeed);
        text.color = alpha;
    }

    public void Set(ObjectPool obp, int damage, Vector3 position, Vector3 textDir, float transparencySpeed)
    {
        // 초기화
        objectPool = obp;
        text.text = damage == 0
            ? "Block"
            : string.Format("{0}", damage);
        this.transform.position = Camera.main.WorldToScreenPoint(position);
        alpha.a = 1f;
        dir = textDir;
        alphaSpeed = transparencySpeed;
        // Invoke("ReturnObject", 1f);
    }

    private void ReturnObject()
    {
        objectPool.Return(this.gameObject);
    }
}
