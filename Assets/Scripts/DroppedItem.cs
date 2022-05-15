using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppedItem : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem red;
    [SerializeField]
    private ParticleSystem blue;
    [SerializeField]
    private ParticleSystem green;
    
    [SerializeField]
    private ParticleSystem white;

    private DropSystem parentDropSystem;
    private Item item;
    public Item Item { get { return item; } }

    public void Set(DropSystem system, Item _item)
    {
        parentDropSystem = system;
        item = _item;
        
        if (item.Rank == 1)
        {
            white.gameObject.SetActive(true);
            white.Play();
        }
        else if (item.Rank == 2)
        {
            green.gameObject.SetActive(true);
            green.Play();
        }
        else if (item.Rank == 3)
        {
            blue.gameObject.SetActive(true);
            blue.Play();
        }
        else if (item.Rank == 4)
        {
            red.gameObject.SetActive(true);
            red.Play();
        }
    }

    public void Return()
    {
        white.Stop();
        white.gameObject.SetActive(false);
        green.Stop();
        green.gameObject.SetActive(false);
        blue.Stop();
        blue.gameObject.SetActive(false);
        red.Stop();
        red.gameObject.SetActive(false);

        if (item.Id > 100)
        {
            // !!! sword 프리팹으로 고정
            parentDropSystem.SwordOP.Return(this.gameObject);
        }
        else
        {
            // !!! box 프리팹으로 고정
            parentDropSystem.BoxOP.Return(this.gameObject);
        }
    }
}
