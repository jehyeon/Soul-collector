using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    private bool disableMode;

    [SerializeField]
    private int portalId;

    private Portal anotherPortal;

    public int Id { get { return portalId; } }

    public void SetAnotherPortal(Portal another)
    {
        anotherPortal = another;
        disableMode = false;
    }

    public void Disable()
    {
        disableMode = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !disableMode)
        {
            // Player가 포탈에 들어오면
            // !!! 사운드, effect, 딜레이 추가하기
            anotherPortal.Disable();    // 반대 포탈 disable
            other.transform.position = anotherPortal.transform.position;    // 반대 포탈 위치로 이동
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 플레이어가 포탈에서 나가면 다시 활성화
            disableMode = false;
        }    
    }
}
