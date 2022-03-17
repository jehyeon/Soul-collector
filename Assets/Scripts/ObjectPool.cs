using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    // public static ObjectPool Instance;
    [SerializeField]
    private GameObject poolingObjectPref;
    [SerializeField]
    private int _count = 0;
    [SerializeField]
    private int _additionalCount = 0;

    Queue<GameObject> poolingObjectQueue = new Queue<GameObject>();

    private void Awake()
    {
        // Instance = this;

        Init(_count);
    }

    private void Init(int count)
    {
        for (int i = 0; i < count; i++)
        {
            poolingObjectQueue.Enqueue(CreateObject());
        }
    }

    private GameObject CreateObject()
    {
        GameObject obj = Instantiate(poolingObjectPref);
        obj.SetActive(false);
        obj.transform.SetParent(this.transform);

        return obj;
    }

    public GameObject Get()
    {
        if (this.poolingObjectQueue.Count < 1)
        {
            Init(_additionalCount);
        }

        GameObject obj = this.poolingObjectQueue.Dequeue();
        obj.transform.SetParent(null);
        obj.gameObject.SetActive(true);

        return obj;
    }

    public void Return(GameObject obj)
    {
        obj.SetActive(false);
        obj.transform.SetParent(this.transform);
        this.poolingObjectQueue.Enqueue(obj);
    }
}