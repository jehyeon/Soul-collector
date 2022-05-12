using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFB_TrollDemo : MonoBehaviour {

	public void Locomotion(float newValue)
    {
        GetComponent<Animator>().SetFloat("Locomotion", newValue);
    }
}
