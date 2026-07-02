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
    [SerializeField] private float moveSpeed = 3f;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource footstepAudioSource;
    [SerializeField] private float walkPitch = 0.85f;  // Slower footsteps for walking
    [SerializeField] private float runPitch = 1.2f;    // Faster footsteps for running

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

    [SerializeField] private Transform cameraTransform;

    void Start()
    {
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        originalStepOffset = characterController.stepOffset;

        // Automatically try to get the AudioSource if it wasn't dragged into the Inspector slot
        if (footstepAudioSource == null)
        {
            footstepAudioSource = GetComponent<AudioSource>();
        }

        // Configure audio source defaults via code to prevent mistakes
        if (footstepAudioSource != null)
        {
            footstepAudioSource.playOnAwake = false;
            footstepAudioSource.loop = true;
        }
    }

    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Camera directions
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        // Ignore camera tilt
        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        // Camera-relative movement
        movementDirection = forward * verticalInput + right * horizontalInput;

        speed = Mathf.Clamp01(movementDirection.magnitude);

        if (movementDirection != Vector3.zero)
            movementDirection.Normalize();

        bool isRunning = false;

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
            isRunning = true; // Player is sprinting
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
            ySpeed = -0.5f;

            animator.SetBool("IsGrounded", true);
            isGrounded = true;
            animator.SetBool("IsJumping", false);
            isJumping = false;
            animator.SetBool("IsFalling", false);

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

        // Handle footstep audio triggers based on movement state
        HandleFootstepAudio(isRunning);

        HandleEmote();
    }

    // OnAnimatorMove drives grounded movement via root motion
    private void OnAnimatorMove()
    {
        if (isGrounded)
        {
            Vector3 velocity = movementDirection * speed * moveSpeed;
            velocity.y = ySpeed;
            characterController.Move(velocity * Time.deltaTime);
        }
    }

    private void HandleFootstepAudio(bool playerIsRunning)
    {
        if (footstepAudioSource == null) return;

        // Player should only make sound if moving AND touching the ground
        bool shouldPlaySound = (movementDirection != Vector3.zero) && isGrounded;

        if (shouldPlaySound)
        {
            // Dynamic footstep speed adjustment based on walking vs running
            footstepAudioSource.pitch = playerIsRunning ? runPitch : walkPitch;

            if (!footstepAudioSource.isPlaying)
            {
                footstepAudioSource.Play();
            }
        }
        else
        {
            // Stop sound immediately if player stands still or jumps into the air
            if (footstepAudioSource.isPlaying)
            {
                footstepAudioSource.Stop();
            }
        }
    }

    private void HandleEmote()
    {
        if (Input.GetKeyDown(KeyCode.G))
            animator.SetTrigger("Greet");

        if (Input.GetKeyDown(KeyCode.Q))
            animator.SetTrigger("Ask");
    }

    private void OnApplicationFocus(bool focus)
    {
        Cursor.lockState = focus ? CursorLockMode.Locked : CursorLockMode.None;
    }
}