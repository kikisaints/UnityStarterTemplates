using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObject : MonoBehaviour
{
    public float Speed = 5.0f;
    public float Height = 12.0f;
    public float OriginOffset = -13.0f;

    private MainCamera mainCameraComponent;
    private void Start()
    {
        mainCameraComponent = this.GetComponent<MainCamera>();
    }

    void FixedUpdate()
    {
        Vector3 followPosition = mainCameraComponent.CameraTarget.transform.position;
        followPosition.y += Height;
        followPosition.z += OriginOffset;

        this.transform.position = Vector3.Lerp(this.transform.position, followPosition, Time.deltaTime * Speed);
    }
}
