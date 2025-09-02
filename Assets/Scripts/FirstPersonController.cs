using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    [Header("Movement Speeds")]
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float sprintMultiplier = 2f;

    [Header("Jump Parameters")]
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float gravityMultiplier = 1f;

    [Header("Look Parameters")]
    [SerializeField] private float mouseSensitivity = 0.1f;
    [SerializeField] private float upDownLookRange = 80f;

    [Header("References")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private PlayerInputHandler playerInputHandler;

    private Vector3 currentMovement;
    private float verticalRotation;
    private float CurrentSpeed => walkSpeed * (playerInputHandler.SprintTriggered ? sprintMultiplier : 1);

    void Start()
    {
        // lock and hide cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleMovement();
        HandleRotation();
    }

    private void HandleMovement()
    {
        // calculate world direction
        Vector3 worldDirection = CalculateWorldDirection();

        // set horizontal speed
        currentMovement.x = worldDirection.x * CurrentSpeed;
        currentMovement.z = worldDirection.z * CurrentSpeed;

        // handle jumping
        HandleJumping();

        // move the player using character controller Move()
        characterController.Move(currentMovement * Time.deltaTime);
    }

    private Vector3 CalculateWorldDirection()
    {
        // get input values from Input Action
        Vector3 inputDirection = new Vector3(playerInputHandler.MovementInput.x, 0f, playerInputHandler.MovementInput.y);

        // convert input direction into world direction
        Vector3 worldDirection = transform.TransformDirection(inputDirection);

        // return normalized world direction
        return worldDirection.normalized;
    }

    private void HandleJumping()
    {
        // check if the player is grounded
        if (characterController.isGrounded)
        {
            // keep player grounded
            currentMovement.y = -0.5f;
            // check if jump is triggered (space bar is pressed)
            if (playerInputHandler.JumpTriggered)
            {
                // add jump force
                currentMovement.y = jumpForce;
            }
        }
        // if not grounded
        else
        {
            // apply gravity
            currentMovement.y += Physics.gravity.y * gravityMultiplier * Time.deltaTime;
        }
    }

    private void HandleRotation()
    {
        // grab rotation input and apply sensitivity
        float mouseXRotation = playerInputHandler.RotationInput.x * mouseSensitivity;
        float mouseYRotation = playerInputHandler.RotationInput.y * mouseSensitivity;

        // apply rotations
        ApplyHorizontalRotation(mouseXRotation);
        ApplyVerticalRotation(mouseYRotation);
    }

    private void ApplyHorizontalRotation(float rotationAmount)
    {
        // rotate player's y axis
        transform.Rotate(0, rotationAmount, 0);
    }

    private void ApplyVerticalRotation(float rotationAmount)
    {
        // clamp vertical rotation
        verticalRotation = Mathf.Clamp(verticalRotation - rotationAmount, -upDownLookRange, upDownLookRange);

        // rotate camera's x axis
        mainCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }
}
