using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEffectSound : MonoBehaviour
{
    [SerializeField]
    private AudioSource footstepSound;
    [SerializeField]
    private AudioSource attackSound;

    public void PlayFootstepSound()
    {
        footstepSound.Play();
    }

    public void PlayattackSound()
    {
        attackSound.Play();
    }
}