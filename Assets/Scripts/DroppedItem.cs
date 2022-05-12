using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppedItem : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem pillar;
    [SerializeField]
    private ParticleSystem area;
    [SerializeField]
    private MeshRenderer mark;

    private DropSystem parentDropSystem;
    private Item item;
    public Item Item { get { return item; } }

    public void Set(DropSystem system, Item _item)
    {
        parentDropSystem = system;
        item = _item;
        
        if (item.Rank == 1 || item.Rank == 2 || item.Rank == 6 || item.Rank == 7)
        {
            // Include rank 1, 6 / 2, 7
            // default, green
            ActiveItemMark(item.FontColor);
        }
        else
        {
            PlayParticle(item.FontColor);
            ActiveItemMark(item.FontColor);
        }
    }

    public void Return()
    {
        StopParticle();
        DeActiveItemMark();

        if (item.Id < 1600)
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

    private void PlayParticle(Color particalColor)
    {
        var pMain = pillar.main;
        var aMain = area.main;
        particalColor.a = 1f / 255f;
        pMain.startColor = particalColor;
        aMain.startColor = particalColor;

        pillar.gameObject.SetActive(true);
        area.gameObject.SetActive(true);
        pillar.Play();
        area.Play();
    }

    private void StopParticle()
    {
        pillar.Stop();
        area.Stop();
        pillar.gameObject.SetActive(false);
        area.gameObject.SetActive(false);
    }

    private void ActiveItemMark(Color markColor)
    {
        mark.material.color = markColor;
        mark.gameObject.SetActive(true);
    }

    private void DeActiveItemMark()
    {
        mark.gameObject.SetActive(false);
    }
}
