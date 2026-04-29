using System;
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
    private Animator animator;

    [Header("Audio Settings")]
    [SerializeField]
    private AudioSource footstepAudioSource;

    [SerializeField]
    private AudioSource jumpLandAudioSource;

    [SerializeField]
    private AudioClip jumpSound;

    [SerializeField]
    private AudioClip landSound;

    [Header("Particles")]
    [SerializeField]
    private ParticleSystem runDustParticles;

    [Header("Respawn Settings")]
    [SerializeField]
    private Transform respawnPoint;

    void Start()
    {
        this.controller = this.GetComponent<CharacterController>();
        this.moveAction = InputSystem.actions.FindAction("Move");
        this.jumpAction = InputSystem.actions.FindAction("Jump");
        this.jumpCooldownTimer = 0.0f;
        this.animator = this.GetComponent<Animator>();
    }

    void SetAnimationState(Vector2 inputMovement)
    {
        this.animator.SetBool("IsJumping", this.isJumping);

        bool isMoving = inputMovement != Vector2.zero;
        this.animator.SetBool("IsRunning", isMoving);
        this.animator.SetFloat("MovementForward", inputMovement.magnitude);

        // --- FOOTSTEPS & DUST PARTICLES LOGIK ---
        if (this.controller.isGrounded && isMoving)
        {
            // Sound an
            if (this.footstepAudioSource != null && !this.footstepAudioSource.isPlaying)
            {
                this.footstepAudioSource.Play();
            }
            // Partikel an
            if (this.runDustParticles != null && !this.runDustParticles.isPlaying)
            {
                this.runDustParticles.Play();
            }
        }
        else
        {
            // Sound aus
            if (this.footstepAudioSource != null && this.footstepAudioSource.isPlaying)
            {
                this.footstepAudioSource.Pause();
            }
            // Partikel aus
            if (this.runDustParticles != null && this.runDustParticles.isPlaying)
            {
                this.runDustParticles.Stop();
            }
        }
    }

    void HandleJumping()
    {
        if (this.controller.isGrounded && this.isJumping && this.jumpCooldownTimer <= 0.0f)
        {
            this.jumpVelocity = Vector3.zero;
            this.isJumping = false;

            // --- LANDING AUDIO ---
            if (this.landSound != null && this.jumpLandAudioSource != null)
            {
                this.jumpLandAudioSource.PlayOneShot(this.landSound);
            }
        }

        if (!this.isJumping && this.jumpAction.WasPressedThisFrame())
        {
            this.characterGravity = Vector3.zero;
            this.jumpVelocity = Vector3.zero;
            this.jumpVelocity.y = this.jumpSpeed;
            this.jumpCooldownTimer = this.jumpCooldown;
            this.isJumping = true;

            // --- AUDIO LOGIK JUMP ---
            if (this.jumpSound != null)
            {
                this.jumpLandAudioSource.PlayOneShot(this.jumpSound);
            }
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

        if (
            Physics.Raycast(
                this.transform.position,
                Vector3.down,
                out RaycastHit hitInfo,
                2.0f,
                this.platformLayer
            )
        )
        {
            TweenedMovingPlatform tweenPlatform =
                hitInfo.collider.GetComponent<TweenedMovingPlatform>();
            if (tweenPlatform != null)
            {
                this.platformVelocity = tweenPlatform.GetVelocity();
                return;
            }

            MovingPlatform oldPlatform = hitInfo.collider.GetComponent<MovingPlatform>();
            if (oldPlatform != null)
            {
                this.platformVelocity = oldPlatform.GetVelocity();
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
            inputRightDirection
            * this.currentInputMovement.x
            * this.characterSpeed
            * Time.deltaTime;
        this.characterMovement +=
            inputForwardDirection
            * this.currentInputMovement.y
            * this.characterSpeed
            * Time.deltaTime;
        this.characterMovement *= (1 - this.dampening);

        Vector3 targetDirection =
            (inputRightDirection * this.currentInputMovement.x)
            + (inputForwardDirection * this.currentInputMovement.y);

        if (targetDirection.sqrMagnitude > 0.001f)
        {
            this.transform.forward = targetDirection.normalized;
        }

        Vector3 appliedPlatformVelocity = Vector3.zero;
        if (!this.isJumping)
        {
            appliedPlatformVelocity = this.platformVelocity;
        }

        var combinedMovement =
            this.characterMovement + appliedPlatformVelocity * Time.fixedDeltaTime;
        this.controller.Move(combinedMovement);

        this.SetAnimationState(this.currentInputMovement);
    }

    void Update()
    {
        this.HandleJumping();
        this.currentInputMovement = this.moveAction.ReadValue<Vector2>();
    }

    // --- RESPAWN LOGIK ---
    public void Respawn()
    {
        if (this.respawnPoint != null)
        {
            this.controller.enabled = false;
            this.transform.position = this.respawnPoint.position;
            this.controller.enabled = true;
        }
        else
        {
            Debug.LogWarning(
                "Respawn Point is not set on Character! Please assign a respawn point in the inspector."
            );
        }
    }

    public void EnemyBounce()
    {
        this.characterGravity = Vector3.zero;
        this.jumpVelocity = Vector3.zero;
        this.jumpVelocity.y = this.jumpSpeed * 0.8f; // Lässt den Spieler leicht nach oben abprallen
        this.jumpCooldownTimer = this.jumpCooldown;
        this.isJumping = true;
    }
}
