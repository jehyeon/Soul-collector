using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickSlotSystem : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // -------------------------------------------------------------
    // UI gameObject (use on UIController.cs)
    // -------------------------------------------------------------
    public void Open()
    {
        this.gameObject.SetActive(true);
    }

    public void Close()
    {
        this.gameObject.SetActive(false);
    }  
}
