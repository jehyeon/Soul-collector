using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFB_MinotaurDemo : MonoBehaviour {

    public Animator animator;
	
    public void SetLocomotion(float value)
    {
        animator.SetFloat("Locomotion", value);
    }
}
