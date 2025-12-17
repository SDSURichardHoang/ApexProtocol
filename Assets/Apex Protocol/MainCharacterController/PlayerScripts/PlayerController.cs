using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.Rendering;

public class PlayerController : MonoBehaviour

{
    public static PlayerController Instance;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Camera playerCamera;
    [SerializeField] public Animator animator;
    public Transform spineTransform;
    [HideInInspector] public StaminaController staminaController;


    public float runAcceleration = 0.25f;
    public float runSpeed = 6f;
    public float sprintSpeed = 9f;
    public float drag = 0.27f;

    public Vector3 jumpVelocity;
    float movementMultiplier = 1f; 
    public bool isSprinting = false;
    bool isRolling = false;
    float rollTimer;
    float fallTimer, fallDamageTimer;

    public float gravityConst = -9.81F;

    public float lookSenseX = 0.27f;
    public float lookSenseY = 0.27f;
    public float lookLimit = 88f;

    private Vector2 camerarotation = Vector2.zero;
    private Vector2 playerrotation = Vector2.zero;

    private Vector3 newMovement;

    private PlayerKeyboard playerinput;
    private float horizontalAxis;
    private float verticalAxis;

    public bool isGrappling = false;

    private void Awake()
    {
        Instance = this;
        playerinput = GetComponent<PlayerKeyboard>();
        staminaController = GetComponent<StaminaController>();
        rollTimer = 1.2f;
        fallTimer = .25f;
        fallDamageTimer = 0f;
        
    }


    public void Update()

    {
        // set x and y for animator blend tree
        horizontalAxis = Input.GetAxis("Horizontal");
        verticalAxis = Input.GetAxis("Vertical");
        animator.SetFloat("InputX", horizontalAxis);
        animator.SetFloat("InputY", verticalAxis);

        Vector3 cameraFowardXZ = new Vector3(playerCamera.transform.forward.x, 0f, playerCamera.transform.forward.z).normalized;
        Vector3 cameraRightXZ = new Vector3(playerCamera.transform.right.x, 0f, playerCamera.transform.right.z).normalized;
        Vector3 movementDirection = cameraRightXZ * playerinput.MovementInput.x + cameraFowardXZ * playerinput.MovementInput.y;

        Vector3 movementDelta = movementDirection * runAcceleration * Time.deltaTime;
        newMovement = characterController.velocity + movementDelta;

        Vector3 currentMove = newMovement.normalized * drag * Time.deltaTime;
        newMovement = (newMovement.magnitude > drag * Time.deltaTime) ? newMovement - currentMove : Vector3.zero;
        newMovement = Vector3.ClampMagnitude(newMovement, runSpeed);



        // jump

        bool isGrounded = characterController.isGrounded;
        float stamina = StaminaController.Instance.playerStamina;
        if (isGrounded && jumpVelocity.y < 0)
        {
            jumpVelocity.y = -1f; // small downward force to stick to ground
        }

        if (Input.GetKeyDown(KeyCode.Space)&& isGrounded && stamina > 25)
        {
            StaminaController.Instance.StaminaJump();
            isGrounded = false;
            if (isSprinting)
            {
                //animator 
                jumpVelocity.y = 6f;
                movementMultiplier= 1.75f;
                animator.SetBool("sprintJumping", true);
            }
            else
            {
                animator.SetBool("isJumping", true);
                jumpVelocity.y = 4.5f; // set jump velocity
                movementMultiplier= 1.25f;
            }
        }
        if (isGrounded)
        {
            movementMultiplier= 1f;
        }


        //sprint 
        isSprinting = false;
        if (Input.GetKey(KeyCode.LeftShift) && sprintConstraints(horizontalAxis,verticalAxis)&&isGrounded&& stamina>0.5)

        {
            StaminaController.Instance.Sprinting();
            //newMovement = newMovement * 2f;
            runSpeed = sprintSpeed;
            isSprinting = true;


        }
        
        
        animator.SetBool("isSprinting", isSprinting);

        //roll
        if (isRolling)
        {
            rollTimer -= Time.deltaTime;
            animator.SetBool("isRolling", true);
            runSpeed = 10f;
            if (rollTimer < 0)
            {
                animator.SetBool("isRolling",false);
                isRolling = false;
                rollTimer = 1.1f;
            }
        }
        if (Input.GetKeyDown(KeyCode.LeftControl) && rollConstraints(horizontalAxis,verticalAxis))
        {
            isRolling = true;


        }

        // falling 
        if (!isGrounded)
        {
            fallTimer -= Time.deltaTime;
            if (fallTimer < 0)
            {
                if (!isGrappling)
                {

                animator.SetBool("isFalling", true);
                animator.SetBool("sprintJumping", false);
                }
                fallTimer = .75f;
            }
            fallDamageTimer += Time.deltaTime;

        }
        // fall damage
        if (isGrounded)
        {
            if (fallDamageTimer > 5)
            {
                Debug.Log(fallDamageTimer);
                this.GetComponent<Health>().takeDamage(fallDamageTimer*fallDamageTimer);
                fallDamageTimer = 0;
            }
            else
            {
                fallDamageTimer = 0;
            }
        }

        //grapple
        Vector3 grappleMove = Vector3.zero;
        if (isGrappling)
        {
            
            Vector3 toPoint = GrapplingHook.Instance.grapplePoint - transform.position;
            float distance = toPoint.magnitude;
            Vector3 dir = toPoint.normalized;

            //pull towards grapple point
            grappleMove = dir * GrapplingHook.Instance.grapplePullSpeed;

            //add optional swing
            Vector3 swingDir = Vector3.Cross(dir, Vector3.up).normalized;
            grappleMove += swingDir * GrapplingHook.Instance.grappleSwingStrength;

            //stop grapple if close
            if (distance < GrapplingHook.Instance.grappleStopDistance)
            {
                isGrappling = false;
                if (GrapplingHook.Instance.grappleLine != null) GrapplingHook.Instance.grappleLine.positionCount = 0;
            }

            //update rope visuals
            if (GrapplingHook.Instance.grappleLine != null)
            {
                GrapplingHook.Instance.grappleLine.positionCount = 2;
                GrapplingHook.Instance.grappleLine.SetPosition(0, GrapplingHook.Instance.gunTip.position);
                GrapplingHook.Instance.grappleLine.SetPosition(1, GrapplingHook.Instance.grapplePoint);
            }
        }



        //gravity
        jumpVelocity.y += gravityConst * Time.deltaTime;

        Vector3 finalMovement = newMovement + new Vector3(0f, jumpVelocity.y, 0f) + grappleMove;
        finalMovement.x *= movementMultiplier;
        finalMovement.z *= movementMultiplier;

            characterController.Move((finalMovement * Time.deltaTime));

        if (isGrounded)
        {
            animator.SetBool("isJumping",false);
            animator.SetBool("sprintJumping",false);
            animator.SetBool("isFalling", false);
        }

        if (!isSprinting && !isRolling)
        {
            runSpeed = 6f;
        }

    }

    public void LateUpdate()
    {
        camerarotation.x += lookSenseX * playerinput.LookInput.x;
        camerarotation.y = Mathf.Clamp(camerarotation.y - lookSenseY * playerinput.LookInput.y, -lookLimit, lookLimit);

        playerrotation.x += transform.eulerAngles.x + lookSenseX * playerinput.LookInput.x;
        transform.rotation = Quaternion.Euler(0f, playerrotation.x, 0f);

        playerCamera.transform.rotation = Quaternion.Euler(camerarotation.y, camerarotation.x, 0f);

        if (GunSystem.Instance.isAiming && spineTransform != null)
        {
            float pitchRotation = -camerarotation.y * 0.8f; // Adjust multiplier as needed
            spineTransform.localRotation = Quaternion.Euler(pitchRotation, 0f, 0f);
        }
    }

    public bool sprintConstraints(float x, float y)
    {
        bool allowSprint = true;
        if (y > 0.1 || (y==0 && x!=0))
        {
            return allowSprint;
        }

            return !allowSprint;
        
    }
    public bool rollConstraints(float x, float y)
    {
        bool allowRoll = true;
        if (y > 0.1 || (y==0 && x!=0)|| (y<0&& x==0))
        {
            return allowRoll;
        }
        return !allowRoll;
    }


}
