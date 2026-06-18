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
    [SerializeField] private float moveSpeed = 3f;

    void Start()
    {
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        originalStepOffset = characterController.stepOffset;
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

        else if (!Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
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
}