using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalSystem : MonoBehaviour
{
    // 서로 연결된 포탈은 같은 index를 가짐
    private Portal[] portalPairOne;
    private Portal[] portalPairTwo;

    // 포탈 시스템 하위 오브젝트는 모두 Portal Component가 있어야 함
    void Start()
    {
        portalPairOne = new Portal[10];
        portalPairTwo = new Portal[10];
        
        // 포탈 초기화
        foreach (Transform child in transform)
        {
            Portal portal = child.GetComponent<Portal>();

            if (portal.Id >= 0)
            {
                Debug.Log(portal.Id);
                Debug.Log(portalPairOne[portal.Id] == null);
                if (portalPairOne[portal.Id] == null)
                {
                    // 한쪽 포탈만 연결됨
                    portalPairOne[portal.Id] = portal;
                }
                else
                {
                    // 양쪽 포탈 모두 등록된 경우 -> 서로 연결
                    portalPairTwo[portal.Id] = portal;
                    portalPairOne[portal.Id].SetAnotherPortal(portalPairTwo[portal.Id]);
                    portalPairTwo[portal.Id].SetAnotherPortal(portalPairOne[portal.Id]);
                }
            }
            else
            {
                Debug.LogError("포탈 초기화: 포탈 ID가 유효하지 않음");
            }
        }
    }

}
