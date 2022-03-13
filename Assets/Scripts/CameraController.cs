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
    private bool cameraFix = false;

    // 장애물
    Renderer obstacleRenderer;

    void Update()
    {
        FollowPlayer();
        SetCameraOffset();
        MoveCameraToObstacleNext();
    }

    private void FollowPlayer()
    {
        if (player == null || cameraFix)
        {
            // 플레이어가 없거나 카메라가 벽에 충돌되는 경우 
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

    private void MoveCameraToObstacleNext()
    {
        RaycastHit hit;
        
        if (Physics.Raycast(
            player.transform.position,
            new Vector3(0, cameraOffset, cameraOffset * -1),
            out hit,
            new Vector3(0, cameraOffset, cameraOffset * -1).magnitude)
        )
        {
            // 카메라가 벽에 닿는 경우, 충돌 지점으로 옮김
            cameraFix = true;
            this.transform.position = hit.point;
        }
        else
        {
            cameraFix = false;
        }
    }
}
