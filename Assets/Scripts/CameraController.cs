using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;
    public float cameraSpeed = 0.05f;

    private Vector3 offset = Vector3.zero;  // camera position offset

    void Start()
    {
        offset = new Vector3(0, 10, -10);
    }

    void Update()
    {
        Vector3 willGoPos = player.transform.position + offset;
        this.transform.position = Vector3.Lerp(this.transform.position, willGoPos, cameraSpeed);
    }
}
