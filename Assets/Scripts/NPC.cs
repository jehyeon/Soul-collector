using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NPCType
{
    Craft,
    Shop,
    Auction
}

public class NPC : MonoBehaviour
{
    private UIController uiController;

    [SerializeField]
    private NPCType type;

    private void Start()
    {
        uiController = GameObject.Find("UI Controller").GetComponent<UIController>();
    }

    public void Talk()
    {
        if (this.type == NPCType.Shop)
        {
            uiController.CloseUI();
            uiController.OpenShopUI();
            return;
        }

        if (this.type == NPCType.Craft)
        {
            uiController.CloseUI();
            uiController.OpenCraftUI();
            return;
        }

        if (this.type == NPCType.Auction)
        {
            uiController.CloseUI();
            uiController.OpenAuctionUI();
            return;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            this.Talk();
        }
    }
}
