using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    private static EffectManager instance = null;
    public static EffectManager Instance { get { return instance; } }

    [SerializeField]
    private ParticleSystem shield;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void EffectShield(float time, Transform target)
    {
        StartCoroutine(Shield(time, target));
    }

    IEnumerator Shield(float time, Transform target)
    {
        // shield.transform.localScale = Vector3.zero;
        shield.transform.SetParent(target);
        shield.gameObject.SetActive(true);
        shield.Play();
        shield.transform.localPosition = new Vector3(0f, 1f, 0f);
        yield return new WaitForSeconds(time);
        shield.Stop();
        shield.gameObject.SetActive(false);
        StopEffect(shield);
    }

    private void StopEffect(ParticleSystem effect)
    {
        effect.transform.SetParent(this.transform);
    }
}
