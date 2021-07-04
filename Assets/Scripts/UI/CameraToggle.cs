using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraToggle : MonoBehaviour
{
    Rocket rocket;
    void Start()
    {
        rocket = GameObject.Find("Rocket").GetComponent<Rocket>();
    }

    public void toggle()
    {
        if (rocket.cameraMode == CameraMode.Follow)
            rocket.cameraMode = CameraMode.Tracking;
        else
            rocket.cameraMode = CameraMode.Follow;
    }
}
