using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    private bool disableMode;
    private GameManager gameManager;

    [SerializeField]
    private int portalId;

    // private Portal anotherPortal;

    public int Id { get { return portalId; } }

    // public void SetAnotherPortal(Portal another)
    // {
    //     anotherPortal = another;
    //     disableMode = false;
    // }

    // public void Disable()
    // {
    //     disableMode = true;
    // }
    public void Set(int id, bool disable = false)
    {
        portalId = id;
        disableMode = disable;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (disableMode)
        {
            // disableMode 일 때는 동작 안함
            return;
        }

        if (other.CompareTag("Player"))
        {
            if (gameManager == null)
            {
                gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
            }
            if (portalId == 0)
            {
                // viliage에서 포탈에 들어가면
                gameManager.PopupAsk("GoDungeon", "던전으로 이동하시겠습니까?", "아니요", "네");
            }
            else if (portalId == 1)
            {
                gameManager.PopupAsk("GoViliage", "마을로 이동하시겠습니까?", "아니요", "네");
            }
            else
            {
                gameManager.PopupAsk("GoNextFloor", "다음 층으로 이동하시겠습니까?", "아니요", "네");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 플레이어가 포탈에서 나가면 활성화
            disableMode = false;
        }    
    }
}
