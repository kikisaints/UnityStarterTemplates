using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharController : MonoBehaviour
{
    #region Variables

    //Components
    public Rigidbody rb;
    public Animator animator;
    public Camera SceneCamera;

    //movement variables
    public float walkSpeed = 26f;
    public float gravityMultiplier = 2.0f;

    //Slope movement variables
    public float height = 1.0f;
    public float heightPadding = 0.05f;
    public LayerMask slopeMask;
    public float maxGroundAngle = 120;
    public bool debug;
    
    //private variables
    private float currentMovementSpeed;
    private float rotationSpeed = 40f;
    private Vector3 inputVec;
    private Vector3 newVelocity;
    private bool canMove = true;
    private float groundAngle;
    private RaycastHit hitInfo;
    private bool onSlope;
    private bool grounded;
    private Vector3 upVector;
    private Vector3 forward;    
    private bool movingCheck = false;
    private Vector3 movePlace = Vector3.zero;

    //Inputs
    private float inputHorizontal = 0;
    private float inputVertical = 0;

    #endregion

    #region Initialization

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        currentMovementSpeed = walkSpeed;
    }

    #endregion

    #region UpdateAndInput

    public void SetMovePlace(Vector3 p) { movePlace = p; }

    public void StopMoving()
    {
        StopMovement();
        currentMovementSpeed = 0;
        canMove = false;

        rb.constraints = RigidbodyConstraints.FreezeAll;
    }

    public void AllowMovement()
    {
        canMove = true;
        currentMovementSpeed = walkSpeed;

        rb.constraints = RigidbodyConstraints.None;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    void Inputs()
    {
        if (canMove)
        {
            inputHorizontal = Input.GetAxis("Horizontal");
            inputVertical = Input.GetAxis("Vertical");

            //If all horizontal and vertical input is approx. 0, then we'll register that as a cease movement
            if (Input.GetAxisRaw("Horizontal") > 0 || Input.GetAxisRaw("Horizontal") < 0 || Input.GetAxisRaw("Vertical") > 0 || Input.GetAxisRaw("Vertical") < 0)
            {
                movingCheck = true;

                rb.constraints = RigidbodyConstraints.None;
                rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

                if(grounded)
                    animator.SetBool("Moving", true);
            }
            else
            {
                movingCheck = false;
                currentMovementSpeed = walkSpeed;

                rb.constraints = RigidbodyConstraints.None;
                rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionX |
                RigidbodyConstraints.FreezePositionZ;

                if(grounded)
                    animator.SetBool("Moving", false);
            }
        }        
    }

    void Update()
    {
        if (canMove)
        {
            //slope functions
            CalculateGroundAngle();
            CheckGround();
            ApplyGravity();
            DrawDebugLines();
            //end slope stuff

            CameraRelativeInput();            
        }
        else {
            inputVec = new Vector3(0, 0, 0);
        }

        animator.SetBool("Grounded", grounded);
    }

    #endregion

    #region Fixed/Late Updates

    void FixedUpdate()
    {
        float currentMovementVelocity = UpdateMovement();

        if (canMove)
        {
            Inputs();            
        }
    }

    #endregion

    #region UpdateMovement

    //Moves the character
    float UpdateMovement()
    {
        Vector3 motion = inputVec;
        //reduce input for diagonal movement
        if (motion.magnitude > 1)
        {
            motion.Normalize();
        }

        if (canMove)
        {
            newVelocity = motion * currentMovementSpeed;
            RotateTowardsMovementDir();
        }

        if (!onSlope)
        {
            //if we are falling use momentum
            newVelocity.y = -9.8f * (gravityMultiplier * Time.deltaTime);

            rb.velocity = newVelocity;
        }
        else {
            if (movingCheck)
                newVelocity.y = -9.8f * (gravityMultiplier * Time.deltaTime);
            else
                newVelocity.y = rb.velocity.y * Time.deltaTime;

            rb.velocity = newVelocity;
        }
        //return a movement value for the animator
        return inputVec.magnitude;
    }

    void StopMovement()
    {
        inputVec = Vector3.zero;
        inputHorizontal = 0;
        inputVertical = 0;
    }

    //rotate character towards direction moved
    void RotateTowardsMovementDir()
    {
        if (inputVec != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(inputVec), Time.deltaTime * rotationSpeed); ;
        }
    }

    //All movement is based off camera facing
    void CameraRelativeInput()
    {
        //Camera relative movement
        Transform cameraTransform = SceneCamera.transform;

        //Forward vector relative to the camera along the x-z plane   
        forward = cameraTransform.TransformDirection(Vector3.forward);
        forward.y = 0;
        forward = forward.normalized;
        Vector3 right = new Vector3(forward.z, 0, -forward.x);
        inputVec = (inputHorizontal * (right)) + (inputVertical * forward);
    }

    #endregion

    #region SlopeMovement

    void CalculateGroundAngle()
    {
        groundAngle = Vector3.Angle(hitInfo.normal, transform.forward);
    }

    void CheckGround()
    {
        if (Physics.Raycast(transform.position, -Vector3.up, out hitInfo, height, slopeMask))
        {
            onSlope = true;
        }
        else
        {
            onSlope = false;
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if(other.collider.tag == "Ground")
            grounded = false;
    }

    private void OnCollisionStay(Collision other)
    {
        if(other.collider.tag == "Ground")
            grounded = true;
    }

    void ApplyGravity()
    {
        if (onSlope)
        {
            Vector3 slopeVector = Vector3.Cross(hitInfo.normal, -transform.TransformDirection(Vector3.right));
            float slopeAngle = Mathf.Atan2(slopeVector.y, slopeVector.x);

            if (slopeAngle > -1.5f)
            {
                gravityMultiplier = (slopeAngle * -1) * 1.5f;
            }
            else
            {
                gravityMultiplier = 80;
            }
        }
        else
        {
            gravityMultiplier = 80;
        }
    }

    void DrawDebugLines()
    {
        Vector3 fwd = transform.TransformDirection(Vector3.forward);
        Vector3 slopeVector = Vector3.Cross(hitInfo.normal, -transform.TransformDirection(Vector3.right));

        if (debug)
        {
            Debug.DrawRay(transform.position, -Vector3.up * height, Color.green);
            Debug.DrawRay(new Vector3(transform.position.x, transform.position.y, transform.position.z), fwd * height, Color.red);
            Debug.DrawRay(transform.position, slopeVector * height, Color.yellow);
        }
    }

    #endregion
}