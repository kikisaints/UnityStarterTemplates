using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CameraBehavior
{
    public class CamLookAt : MonoBehaviour
    {
        private CamComponent mainCameraComponent;
        public float LookAtSpeed;

        private void Start()
        {
            mainCameraComponent = this.GetComponent<CamComponent>();
        }

        void LateUpdate()
        {
            Vector3 lookAtLerp = Vector3.Lerp(transform.position, mainCameraComponent.CameraTarget.position, Time.deltaTime * LookAtSpeed);
            this.transform.LookAt(lookAtLerp);
        }
    }
}
