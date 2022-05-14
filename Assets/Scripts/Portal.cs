using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PortalType
{
    NextFloor,
    GoDungeon,
    GoViliage,
    Red,
    Gold,
    Boss
}

public class Portal : MonoBehaviour
{
    private GameManager gameManager;

    private GameObject portalOpen;
    private GameObject portalIdle;
    private PortalType type;

    public void Set(GameObject open, GameObject idle, PortalType portalType)
    {
        portalOpen = open;
        portalOpen.transform.parent = this.transform;
        portalOpen.transform.localPosition = Vector3.zero;
        portalIdle = idle;
        portalIdle.transform.parent = this.transform;
        portalIdle.transform.localPosition = Vector3.zero;
        type = portalType;
        StartCoroutine("CreatePortal");
    }

    IEnumerator CreatePortal()
    {
        portalOpen.SetActive(true);
        yield return new WaitForSeconds(0.8f);
        portalIdle.SetActive(true);
        portalOpen.SetActive(false);
    }

    public void Enter()
    {
        if (gameManager == null)
        {
            gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        }

        if (type == PortalType.GoViliage)
        {
            gameManager.PopupAsk("GoViliage", "마을로 이동하시겠습니까?", "아니요", "네");
            return;
        }

        if (type == PortalType.NextFloor)
        {
            gameManager.PopupAsk("GoNextFloor", "다음 층으로 이동하시겠습니까?", "아니요", "네");
            return;
        }

        if (type == PortalType.GoDungeon)
        {
            gameManager.PopupAsk("GoDungeon", "던전으로 이동하시겠습니까?", "아니요", "네");
            return;
        }
    }
}
