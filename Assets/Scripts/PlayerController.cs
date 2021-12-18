using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Vector3 clickedPos = Vector3.zero;
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit raycastHit))
            {
                if (raycastHit.collider.CompareTag("Ground"))
                {
                    clickedPos = raycastHit.point + new Vector3(0, 1, 0);
                }
                else
                {
                    clickedPos = this.transform.position;
                }
            }
            this.transform.position = clickedPos;
        }
    }
}
