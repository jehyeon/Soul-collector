using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyEffectSound : MonoBehaviour
{
    [SerializeField]
    private AudioSource attackedSound;

    public void PlayAttackedSound()
    {
        attackedSound.Play();
    }
}
