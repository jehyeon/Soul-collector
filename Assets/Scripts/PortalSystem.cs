using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalSystem : MonoBehaviour
{
    [SerializeField]
    private GameObject goldPortalOpenPref;
    [SerializeField]
    private GameObject goldPortalPref;
    [SerializeField]
    private GameObject redPortalOpenPref;
    [SerializeField]
    private GameObject redPortalPref;
    [SerializeField]
    private GameObject greenPortalOpenPref;
    [SerializeField]
    private GameObject greenPortalPref;
    [SerializeField]
    private GameObject purplePortalOpenPref;
    [SerializeField]
    private GameObject purplePortalPref;
    [SerializeField]
    private GameObject bluePortalOpenPref;
    [SerializeField]
    private GameObject bluePortalPref;

    [SerializeField]
    private GameObject portalPref;

    private static PortalSystem instance = null;
    public static PortalSystem Instance { get { return instance; } }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void CreatePortal(Vector3 position, PortalType type)
    {
        Portal portal = Instantiate(portalPref).GetComponent<Portal>();
        portal.transform.position = position;
        if (type == PortalType.GoViliage)
        {
            portal.Set(
                Instantiate(greenPortalOpenPref),
                Instantiate(greenPortalPref),
                type
            );
            return;
        }

        if (type == PortalType.GoDungeon || type == PortalType.NextFloor)
        {
            portal.Set(
                Instantiate(bluePortalOpenPref),
                Instantiate(bluePortalPref),
                type
            );
            return;
        }

        if (type == PortalType.Red)
        {
            portal.Set(
                Instantiate(redPortalOpenPref),
                Instantiate(redPortalPref),
                type
            );
            return;
        }

        if (type == PortalType.Gold)
        {
            portal.Set(
                Instantiate(goldPortalOpenPref),
                Instantiate(goldPortalPref),
                type
            );
            return;
        }

        if (type == PortalType.Boss)
        {
            portal.Set(
                Instantiate(purplePortalOpenPref),
                Instantiate(purplePortalPref),
                type
            );
            return;
        }
    }
}
