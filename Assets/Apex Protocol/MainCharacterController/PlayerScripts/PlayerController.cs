using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.Rendering;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Camera playerCamera;
    [SerializeField] public Animator animator;

    public float runAcceleration = 0.25f;
    public float runSpeed = 6f;
    public float sprintSpeed = 9f;
    public float drag = 0.27f;

    public Vector3 jumpVelocity;
    private float sprintBoost;
    bool isSprinting = false;
    bool isRolling = false;
    float rollTimer;
    bool beginRoll;

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
    private void Awake()
    {
        playerinput = GetComponent<PlayerKeyboard>();
        rollTimer = 1.2f;
        
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


        //characterController.Move(newMovement * Time.deltaTime);

        // jump

        bool isGrounded = characterController.isGrounded;
        //float testF = 1f;
        if (isGrounded && jumpVelocity.y < 0)
        {
            jumpVelocity.y = -1f; // small downward force to stick to ground
            //testF = 1f;
        }
        if (Input.GetKeyDown(KeyCode.Space)&& isGrounded)
        {
            isGrounded = false;
            animator.SetBool("isJumping", true);
            jumpVelocity.y = 4f; // set jump velocity
       //     testF = 4f;
        }


        //sprint 
        isSprinting = false;
        if (Input.GetKey(KeyCode.LeftShift) && sprintConstraints(horizontalAxis,verticalAxis)&&isGrounded)
        {
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

        //gravity
        jumpVelocity.y += gravityConst * Time.deltaTime;

        Vector3 finalMovement = newMovement + new Vector3(0f, jumpVelocity.y, 0f);
        //finalMovement = finalMovement * testF;
        characterController.Move((finalMovement * Time.deltaTime));
        if (isGrounded)
        {
            animator.SetBool("isJumping",false);
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
    }

    public bool sprintConstraints(float x, float y)
    {
        bool allowSprint = true;
        if (y > 0.1 || (y==0 && x!=0))
        {
            //Debug.Log("Sprint Allowed with x:" + x +" and y:" +y);
            return allowSprint;
        }

            //Debug.Log("Sprint not allowed with x:" + x +" and y:" +y);
            return !allowSprint;
        
    }
    public bool rollConstraints(float x, float y)
    {
        bool allowRoll = true;
        if (y > 0.1 || (y==0 && x!=0)|| (y<0&& x==0))
        {
            //Debug.Log("Roll Allowed with x:" + x +" and y:" +y);
            return allowRoll;
        }
            //Debug.Log("Roll not allowed with x:" + x +" and y:" +y);
        return !allowRoll;

        
    }


}
