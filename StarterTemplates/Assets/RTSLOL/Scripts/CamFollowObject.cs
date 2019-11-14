using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CameraBehavior
{
    public class CamFollowObject : MonoBehaviour
    {
        public float Speed = 5.0f;
        public float Height = 12.0f;
        public float OriginOffset = -13.0f;
        public float CameraOrentationSpeed = 1.2f;
        public float MinTiltOffset = -0.5f;

        [SerializeField]
        private Vector3 mainCameraPosition;
        private bool obscurred = false;
        private float camHeight;
        private float camOffset;
        private CameraBehavior.CamComponent mainCameraComponent;

        private void Start()
        {
            mainCameraComponent = this.GetComponent<CameraBehavior.CamComponent>();
            camOffset = OriginOffset;
            camHeight = Height;
        }

        private void HandlePlayerObscurring()
        {
            RaycastHit rayInfo;
            if (Physics.Linecast(this.transform.position, mainCameraComponent.CameraTarget.transform.position, out rayInfo))
            {
                if (rayInfo.collider.name != "Player" && rayInfo.collider.gameObject.layer == 9)
                {
                    if (camOffset <= MinTiltOffset)
                        camOffset += Time.deltaTime * CameraOrentationSpeed;
                }
                else
                {
                    if (camOffset >= OriginOffset)
                        camOffset -= Time.deltaTime * CameraOrentationSpeed;
                }
            }
        }

        void FixedUpdate()
        {
            //HandlePlayerObscurring();

            if (mainCameraComponent.CameraTarget)
            {
                mainCameraPosition = mainCameraComponent.CameraTarget.transform.position;
                mainCameraPosition.y += camHeight;
                mainCameraPosition.z += OriginOffset;

                Vector3 followPosition = mainCameraComponent.CameraTarget.transform.position;
                followPosition.y += camHeight;
                followPosition.z += camOffset;

                this.transform.position = Vector3.Lerp(this.transform.position, followPosition, Time.deltaTime * Speed);
            }
            else
            {
                Debug.LogWarning("Please set a target for the camera to follow");
            }
        }
    }
}
