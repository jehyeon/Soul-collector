using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;
    public float cameraSpeed = 0.05f;

    private float cameraOffset = 5f;
    public float offsetMax = 10f;
    public float offsetMin = 3f;

    void Update()
    {
        FollowPlayer();
        SetCameraOffset();
        
    }

    private void FollowPlayer()
    {
        if (player == null)
        {
            return;
        }

        Vector3 willGoPos = player.transform.position + new Vector3(0, cameraOffset, cameraOffset * -1);
        this.transform.position = Vector3.Lerp(this.transform.position, willGoPos, cameraSpeed);
    }

    private void SetCameraOffset()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            cameraOffset -= 1f;
            if (cameraOffset < offsetMin)
            {
                cameraOffset = offsetMin;
            }
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            cameraOffset += 1f;
            if (cameraOffset > offsetMax)
            {
                cameraOffset = offsetMax;
            }
        }
    }
}
