using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Camera playerCamera;
    [SerializeField] public Animator animator;

    public float runAcceleration = 0.25f;
    public float runSpeed = 2f;
    public float drag = 0.27f;

    public Vector3 jumpVelocity;

    public float gravityConst = -9.81F;

    public float lookSenseX = 0.27f;
    public float lookSenseY = 0.27f;
    public float lookLimit = 88f;

    private Vector2 camerarotation = Vector2.zero;
    private Vector2 playerrotation = Vector2.zero;


    private PlayerKeyboard playerinput;
    private float horizontalAxis;
    private float verticalAxis; 
    private void Awake()
    {
        playerinput = GetComponent<PlayerKeyboard>();
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
        Vector3 newMovement = characterController.velocity + movementDelta;

        Vector3 currentMove = newMovement.normalized * drag * Time.deltaTime;
        newMovement = (newMovement.magnitude > drag * Time.deltaTime) ? newMovement - currentMove : Vector3.zero;
        newMovement = Vector3.ClampMagnitude(newMovement, runSpeed);

        //characterController.Move(newMovement * Time.deltaTime);

        // jump

        bool isGrounded = characterController.isGrounded;
        if (isGrounded && jumpVelocity.y < 0)
        {
            jumpVelocity.y = -1f; // small downward force to stick to ground
        }
        if (Input.GetKeyDown(KeyCode.Space)&& isGrounded)
        {
            isGrounded = false;
            animator.SetBool("isJumping", true);
            jumpVelocity.y = 3.5f; // set jump velocity
        }
        
        //gravity
        jumpVelocity.y += gravityConst * Time.deltaTime;

        Vector3 finalMovement = newMovement + new Vector3(0f, jumpVelocity.y, 0f);
        characterController.Move((finalMovement * Time.deltaTime));
        if (isGrounded)
        {
            animator.SetBool("isJumping",false);
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


}
