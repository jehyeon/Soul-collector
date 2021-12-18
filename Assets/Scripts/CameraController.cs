using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;

    void Update()
    {
        this.transform.position = player.transform.position + new Vector3(0, 10, -10);
    }
}
