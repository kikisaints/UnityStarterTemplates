using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [System.Serializable]
    public enum MOVEMENTSTATE
    {
        WALKING = 0,
        DASHING,
        SKATING,
        FLYING
    };

    [System.Serializable]
    public class MovementType
    {
        public float MovementSpeed = 0f;
        public float RotationSpeed = 0f;
    }    

    public Rigidbody Rigidbody;
    public CapsuleCollider Collider;
    public MOVEMENTSTATE CurrentMoveState;
    public List<MovementType> MovementTypes;
    
    public float FallMultiplier = 5.0f;
    public LayerMask GroundLayer;
    public float MaxGroundAngle = 120.0f;
    public bool debug;

    private float movementSpeedMultiplier = 100.0f;    
    private float groundAngle;
    private float groundHeightOffset;
    private float groundHeightError;

    private Vector3 forward;
    private Camera cam = null;
    private RaycastHit hitInfo;
    private bool grounded;

    private void Start()
    {
        groundHeightOffset = Collider.height / 2;
        groundHeightError = groundHeightOffset * 0.1f;
        cam = Camera.main;
    }

    private void Update()
    {
        CalculateForward();
        CalculateGroundAngle();
        CheckGround();

        MultiplyGravity();
        AimChracter();

        //Debug stuff
        DrawDebugLines();
    }

    private void FixedUpdate()
    {
        MoveCharacter();
    }

    private void MoveCharacter()
    {
        if (groundAngle >= MaxGroundAngle) return;

        float translation = Input.GetAxis("Vertical") * GetMovementSpeed();

        translation *= Time.deltaTime;
        Rigidbody.velocity = forward * translation;
    }

    private void AimChracter()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Quaternion targetRotation = Quaternion.LookRotation(ray.direction);
        Quaternion currentRotation = transform.rotation;
        float angularDifference = Quaternion.Angle(currentRotation, targetRotation);

        if (angularDifference > 0)
        {
            Quaternion finalRotation = Quaternion.Slerp(currentRotation, targetRotation, (GetRotationSpeed() * 180 * Time.deltaTime) / angularDifference);
            finalRotation.eulerAngles = new Vector3(0, finalRotation.eulerAngles.y, 0);
            transform.rotation = finalRotation;
        }
        else
        {
            transform.rotation = targetRotation;
        }
    }

    private float GetMovementSpeed()
    {
        int index = (int)CurrentMoveState;
        return MovementTypes[index].MovementSpeed * movementSpeedMultiplier;
    }

    private float GetRotationSpeed()
    {
        int index = (int)CurrentMoveState;
        return MovementTypes[index].RotationSpeed;
    }

    private void CalculateForward()
    {
        if(!grounded)
        {
            forward = this.transform.forward;
            return;
        }

        forward = Vector3.Cross(this.transform.right, hitInfo.normal);
    }

    private void CalculateGroundAngle()
    {
        if (!grounded)
        {
            groundAngle = 90;
            return;
        }

        groundAngle = Vector3.Angle(hitInfo.normal, this.transform.forward);
    }

    private void CheckGround()
    {
        if(Physics.Raycast(this.transform.position, -Vector3.up, out hitInfo, groundHeightOffset + groundHeightError, GroundLayer))
        {
            grounded = true;
        }
        else
        {
            grounded = false;
        }
    }

    private void MultiplyGravity()
    {
        if(!grounded)
        {
            Rigidbody.AddForce(-Vector3.up * (FallMultiplier * movementSpeedMultiplier), ForceMode.Acceleration);
        }
    }

    private void DrawDebugLines()
    {
        if (!debug) return;

        Debug.DrawLine(this.transform.position, this.transform.position + forward * groundHeightOffset * 2, Color.blue);
    }
}
