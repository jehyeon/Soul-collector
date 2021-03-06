using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIEffectSound : MonoBehaviour
{
    [SerializeField]
    private AudioSource openInventorySound;
    [SerializeField]
    private AudioSource clickButtonSound;
    [SerializeField]
    private AudioSource selectItemSound;
    [SerializeField]
    private AudioSource potionSound;
    [SerializeField]
    private AudioSource getRewardSound;
    [SerializeField]
    private AudioSource coinSound;
    [SerializeField]
    private AudioSource createSound;
    [SerializeField]
    private AudioSource equipSwordSound;
    [SerializeField]
    private AudioSource equipBowSound;
    [SerializeField]
    private AudioSource equipLeatherSound;
    [SerializeField]
    private AudioSource equipMetalSound;
    [SerializeField]
    private AudioSource equipJewelSound;
    [SerializeField]
    private AudioSource getItemSound;

    public void PlayOpenInventorySound()
    {
        openInventorySound.Play();
    }

    public void PlaySelectItemSound()
    {
        selectItemSound.Play();
    }

    public void PlayEquipSwordSound()
    {
        equipSwordSound.Play();
    }

    public void PlayEquipMetalArmorSound()
    {
        equipMetalSound.Play();
    }

    public void PlayBuySound()
    {
        coinSound.Play();
    }
    public void PlayGetItemSound()
    {
        getItemSound.Play();
    }

    public void PlayGetRewardSound()
    {
        getRewardSound.Play();
    }

    public void PlayPotionSound()
    {
        potionSound.Play();
    }
}
