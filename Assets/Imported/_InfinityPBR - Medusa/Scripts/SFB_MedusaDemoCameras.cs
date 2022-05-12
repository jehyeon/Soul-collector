using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFB_MedusaDemoCameras : MonoBehaviour
{
    public SFB_DemoControl_v2 demoControl;
    public GameObject thisCamera;

    void OnEnable()
    {
        demoControl.cameraObject = thisCamera;
    }
}
