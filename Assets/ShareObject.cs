using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShareObject : MonoBehaviour
{
    public static ShareObject instance = null;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(instance);
    }
}
