using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubaLagi : MonoBehaviour

{
    [Header("Movement")]
    [SerializeField] private float rotationSpeed = 500f;
    [SerializeField] private float jumpSpeed = 7f;
    [SerializeField] private float jumpButtonGracePeriod = 0.2f;
    [SerializeField] private float jumpHorizontalSpeed = 5f;

    [Header("Climbing")]
    public float climbSpeed = 3f;
    [SerializeField] private float pushOffForce = 4f;

    private Animator animator;
    private CharacterController characterController;

    private float ySpeed;
    private float originalStepOffset;
    private float? lastGroundedTime;
    private float? jumpButtonPressedTime;
    private bool isJumping;
    private bool isGrounded;
    private Vector3 movementDirection;
    private float speed;
    private bool isClimbing;
    private bool canClimb;
    private int climbLayer;
    private Vector3 wallNormal;

    [SerializeField] private float moveSpeed = 3f;

    void Start()
    {
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        originalStepOffset = characterController.stepOffset;

        climbLayer = LayerMask.NameToLayer("ClimbLayer");
        climbLayer = ~climbLayer;
    }
    

    void Update()
    {
        // Remove the local declarations, just assign:
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        movementDirection = new Vector3(horizontalInput, 0, verticalInput);
        speed = Mathf.Clamp01(movementDirection.magnitude);

        // FIX: use magnitude directly as "Speed" (0–1 walk, 0.5 with shift)

        if (movementDirection == Vector3.zero)
        {
            animator.SetFloat("Speed", 0);
        }

        else if (!Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift))
        {
            animator.SetFloat("Speed", 0.5f);
        }

        else
        {
            animator.SetFloat("Speed", 1);
            speed *= 2f;
        }

        animator.SetFloat("Speed", speed, 0.05f, Time.deltaTime);

        // Accumulate gravity every frame
        ySpeed += Physics.gravity.y * Time.deltaTime;

        // Record last time we were grounded
        if (characterController.isGrounded)
            lastGroundedTime = Time.time;

        // Record when jump was pressed
        if (Input.GetButtonDown("Jump"))
            jumpButtonPressedTime = Time.time;

        if (Time.time - lastGroundedTime <= jumpButtonGracePeriod)
        {
            // ---- GROUNDED BRANCH ----
            characterController.stepOffset = originalStepOffset;
            ySpeed = -0.5f;                          // small downward force keeps CC grounded

            animator.SetBool("IsGrounded", true);
            isGrounded = true;
            animator.SetBool("IsJumping", false);
            isJumping = false;
            animator.SetBool("IsFalling", false);    // FIX: clear falling on land

            // Jump within grace window
            if (jumpButtonPressedTime.HasValue &&
                Time.time - jumpButtonPressedTime.Value <= jumpButtonGracePeriod)
            {
                ySpeed = jumpSpeed;
                animator.SetBool("IsJumping", true);
                isJumping = true;
                jumpButtonPressedTime = null;
                lastGroundedTime = null;
            }
        }
        else
        {
            // ---- IN-AIR BRANCH ----
            characterController.stepOffset = 0;
            animator.SetBool("IsGrounded", false);
            isGrounded = false;

            if ((isJumping && ySpeed < 0) || ySpeed < -2f)
                animator.SetBool("IsFalling", true);

            // FIX: move the character while airborne
            Vector3 velocity = movementDirection * speed * jumpHorizontalSpeed;
            velocity.y = ySpeed;
            characterController.Move(velocity * Time.deltaTime);
        }

        // Rotation
        if (movementDirection != Vector3.zero)
        {
            animator.SetBool("IsMoving", true);
            Quaternion toRotation = Quaternion.LookRotation(movementDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }
        else
        {
            animator.SetBool("IsMoving", false);
        }
 
       
        Debug.Log($"isClimbing={isClimbing}, canClimb={canClimb}");

        if (canClimb && !isClimbing && Input.GetKeyDown(KeyCode.E))
        {
            animator.SetBool("IsClimbing", true);
            isClimbing = true;
        }

        if (isClimbing)
        {
            HandleClimbing();
            HandleEmote();
            return; // skip normal movement/gravity/jump logic entirely while climbing
        }

        HandleEmote();
    }

    // OnAnimatorMove drives grounded movement via root motion
    private void OnAnimatorMove()
    {
        if (isGrounded)
        {
            // Remove the animator.deltaPosition line and drive it manually
            Vector3 velocity = movementDirection * speed * moveSpeed;
            velocity.y = ySpeed;
            characterController.Move(velocity * Time.deltaTime);
        }
    }

    private void HandleEmote()
    {
        if (Input.GetKeyDown(KeyCode.G))
            animator.SetTrigger("Greet");
    }

    private void OnApplicationFocus(bool focus)
    {
        Cursor.lockState = focus ? CursorLockMode.Locked : CursorLockMode.None;
    }

    private void HandleClimbing()
    {
        // Exit by jumping off
        if (Input.GetButtonDown("Jump"))
        {
            ExitClimb(pushOff: true);
            return;
        }

        // Vertical input drives climb up/down
        float climbInput = Input.GetAxis("Vertical");
        Vector3 climbMove = Vector3.up * climbInput * climbSpeed;
        characterController.Move(climbMove * Time.deltaTime);

        // Drive blend tree parameter
        animator.SetFloat("ClimbDirection", climbInput, 0.05f, Time.deltaTime);

        animator.SetFloat("ClimbSpeed", climbInput, 0.05f, Time.deltaTime);

        // Reset any residual ySpeed so it doesn't carry into falling once you exit
        ySpeed = 0f;
    }

    private void ExitClimb(bool pushOff)
    {
        animator.SetBool("IsClimbing", false);
        isClimbing = false;

        if (pushOff)
        {
            // Push backward away from the wall and give a small upward hop
            ySpeed = jumpSpeed * 0.5f;
            Vector3 push = -wallNormal * pushOffForce;
            characterController.Move(push * Time.deltaTime);
        }
        else
        {
            ySpeed = -0.5f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wall"))
        {
            canClimb = true;
            // Estimate wall normal as the direction from the wall to the player
            wallNormal = (transform.position - other.transform.position).normalized;
            wallNormal.y = 0;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Wall"))
        {
            canClimb = false;
            if (isClimbing)
                ExitClimb(pushOff: false);
        }
    }
}