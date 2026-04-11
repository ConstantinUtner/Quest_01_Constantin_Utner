using UnityEngine;
using UnityEngine.InputSystem;

public class Character : MonoBehaviour
{
    private bool isJumping = false;
    private float jumpCooldownTimer;
    private CharacterController controller;
    private InputAction moveAction;
    private InputAction jumpAction;

    [SerializeField]
    private float jumpCooldown;

    [SerializeField]
    private float gravity;

    [SerializeField]
    private float characterSpeed;

    [SerializeField]
    private float jumpSpeed;

    [SerializeField]
    private float dampening;

    [SerializeField]
    private Transform cameraTransform;

    [SerializeField]
    private LayerMask platformLayer;

    private Vector3 characterMovement;
    private Vector3 jumpVelocity;
    private Vector3 characterGravity;
    private Vector3 platformVelocity;
    private Vector2 currentInputMovement;

    void Start()
    {
        this.controller = this.GetComponent<CharacterController>();
        this.moveAction = InputSystem.actions.FindAction("Move");
        this.jumpAction = InputSystem.actions.FindAction("Jump");
        this.jumpCooldownTimer = 0.0f;
    }

    void HandleJumping()
    {
        if (this.controller.isGrounded && this.isJumping && this.jumpCooldownTimer <= 0.0f)
        {
            this.jumpVelocity = Vector3.zero;
            this.isJumping = false;
        }

        if (!this.isJumping && this.jumpAction.WasPressedThisFrame())
        {
            this.characterGravity = Vector3.zero;
            this.jumpVelocity = Vector3.zero;
            this.jumpVelocity.y = this.jumpSpeed;
            this.jumpCooldownTimer = this.jumpCooldown;
            this.isJumping = true;
        }

        if (this.jumpVelocity.y > 0.0f)
        {
            this.jumpVelocity.y -= Time.deltaTime;
        }
        else
        {
            this.jumpVelocity = Vector3.zero;
        }

        this.jumpCooldownTimer -= Time.deltaTime;
    }

    private void GetPlatformVelocity()
    {
        this.platformVelocity = Vector3.zero;
        if (Physics.Raycast(this.transform.position, Vector3.down, out RaycastHit hitInfo, 2.0f, this.platformLayer))
        {
            MovingPlatform platform = hitInfo.collider.GetComponent<MovingPlatform>();
            if (platform != null)
            {
                this.platformVelocity = platform.GetVelocity();
            }
        }
    }

    void FixedUpdate()
    {
        this.GetPlatformVelocity();

        var inputRightDirection = this.cameraTransform.right;
        var inputForwardDirection = this.cameraTransform.forward;

        inputRightDirection.y = 0.0f;
        inputForwardDirection.y = 0.0f;
        inputRightDirection.Normalize();
        inputForwardDirection.Normalize();

        if (this.controller.isGrounded)
        {
            this.characterGravity.y = 0.0f;
        }

        this.characterGravity.y += this.gravity * Time.deltaTime;
        this.characterMovement += this.characterGravity * Time.deltaTime;
        this.characterMovement += this.jumpVelocity * Time.deltaTime;
        this.characterMovement +=
            inputRightDirection * this.currentInputMovement.x * this.characterSpeed * Time.deltaTime;
        this.characterMovement +=
            inputForwardDirection * this.currentInputMovement.y * this.characterSpeed * Time.deltaTime;
        this.characterMovement *= (1 - this.dampening);

        Vector3 targetDirection = (inputRightDirection * this.currentInputMovement.x) + (inputForwardDirection * this.currentInputMovement.y);


        if (targetDirection.sqrMagnitude > 0.001f)
        {
            this.transform.forward = targetDirection.normalized;
        }

        Vector3 appliedPlatformVelocity = Vector3.zero;
        if (!this.isJumping)
        {
            appliedPlatformVelocity = this.platformVelocity;
        }

        var combinedMovement = this.characterMovement + appliedPlatformVelocity * Time.fixedDeltaTime;
        this.controller.Move(combinedMovement);
    }

    void Update()
    {
        this.HandleJumping();
        this.currentInputMovement = this.moveAction.ReadValue<Vector2>();
    }
}
