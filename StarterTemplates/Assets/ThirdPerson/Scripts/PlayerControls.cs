using UnityEngine;

public class PlayerControls : MonoBehaviour {

    private Vector3 move; //Store WASD Input
    private Animator anim;
    private CharacterController cont;
    private CameraControls cam;

    public float jumpHeight = 10; //How high the player jumps
    public float gravity = 20; //How fast the player falls

    private bool onGround = true;
    private float vSpeed = -10; //Current gravity force

    private float weight; //Weight of IK look
    public bool useIK; //If we want to use IK look

    void Start()
    {
        anim = GetComponent<Animator>();
        cont = GetComponent<CharacterController>();
        cam = GameObject.FindWithTag("MainCamera").GetComponent<CameraControls>();
    }

    void Update()
    {
        onGround = cont.isGrounded; //Character Controller value for if we are on ground
        if (anim.GetBool("Ground") != onGround) //If our animator Ground isn't the same
        {
            anim.SetBool("Ground", onGround);
            if (onGround)
            {
                vSpeed = -10; //if we our grounded, set constant ground force
                anim.SetBool("Land", true); //Play landing animation
                weight = 0.1f; //Set IK weight low for landing animation
            }
            else if (vSpeed == -10) vSpeed = 0; //If we didn't jump but aren't grounded, remove constant ground force
        }
        if (onGround)
        {
            if (Input.GetButtonDown("Jump"))
            {
                anim.SetBool("Jump", true); //Play jumping animation
                vSpeed = jumpHeight; //Set current gravity force to jump height
            }
        }
        else vSpeed -= gravity * Time.deltaTime; //If in air, add gravity force every frame

        //Sets "move" to our current WASD input value (raw = 0 or 1, nothing inbetween)
        move = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

        //Set our animator values to our input with damping, removing time if we are stopping to make it move immidiate
        anim.SetFloat("Horizontal", move.x, 0.1f, (2.5f - move.magnitude) * Time.deltaTime);
        anim.SetFloat("Vertical", move.z, 0.1f, (2.5f - move.magnitude) * Time.deltaTime);
        
        Vector3 translate = move.normalized * 5; //Normalize move so that we don't go faster diagonally
        if (translate.z < 0) translate *= 0.75f; //If we are going backwards, go slower
        translate.y = vSpeed; //Set our movement.y to current gravity force
        cont.Move(transform.rotation * translate * Time.deltaTime);
    }
    
    //Called by CameraControls at the end of it's LateUpdate calculations
    public void SetRotation(float rot)
    {
        if (move.magnitude > 0) //If moving, set player rotation to camera angle
            transform.rotation = Quaternion.Euler(0, rot, 0);
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (useIK)
        {
            //From landing animation, slowly build weight back up to give it smoothing time
            if (weight < 0.75f) weight = Mathf.Clamp(weight + Time.deltaTime, 0, 0.75f);

            //Set our IK look position to where our camera is looking
            anim.SetLookAtWeight(weight, weight / 3, 0.75f, 1, 0.5f);
            anim.SetLookAtPosition(transform.position + new Vector3(0, 1.5f, 0) + cam.transform.forward);
        }
    }
}
