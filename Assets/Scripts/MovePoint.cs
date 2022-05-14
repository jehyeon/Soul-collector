using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePoint : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem particle;

    private static MovePoint instance = null;
    public static MovePoint Instance { get { return instance; } }
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

    public void Activate(Vector3 point)
    {
        instance.transform.position = point;
        particle.Play();
    }
}
