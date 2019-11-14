using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtObject : MonoBehaviour
{
    private MainCamera mainCameraComponent;

    private void Start()
    {
        mainCameraComponent = this.GetComponent<MainCamera>();
    }

    void FixedUpdate()
    {
        this.transform.LookAt(mainCameraComponent.CameraTarget);
    }
}
