using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CameraBehavior
{
    public class CamComponent : MonoBehaviour
    {
        public Transform CameraTarget;
        public CamFollowObject FollowObjectComponent;
        public CamLookAt LookAtObjectComponent;
    }
}
