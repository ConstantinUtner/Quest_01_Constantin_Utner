using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Lever : MonoBehaviour
{
    private bool on = false;

    public bool IsOn
    {
        get { return this.on; }
    }

    private bool interpolating = false;
    private float currentInterpolationTime = 0.0f;
    private InputAction interactAction;
    private bool isPlayerInRange = false;
    private bool wasInteractPressed = false;

    [SerializeField]
    private float switchTime;

    [SerializeField]
    private Transform onPosition;

    [SerializeField]
    private Transform offPosition;

    [SerializeField]
    private GameObject leverHandle;

    void Start()
    {
        this.interactAction = InputSystem.actions.FindAction("Interact");
    }

    void Update()
    {
        if (this.interactAction.WasPressedThisFrame())
        {
            this.wasInteractPressed = true;
        }
    }

    void FixedUpdate()
    {
        if (this.wasInteractPressed)
        {
            if (this.isPlayerInRange && !this.interpolating)
            {
                this.ToggleLever();
            }
            
            this.wasInteractPressed = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Character"))
        {
            this.isPlayerInRange = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Character"))
        {
            this.isPlayerInRange = false;
        }
    }

    void ToggleLever()
    {
        this.on = !this.on;
        this.StartCoroutine(this.InterpolateLeverCoroutine());
    }

    IEnumerator InterpolateLeverCoroutine()
    {
        this.interpolating = true;
        Vector3 startPosition,
            targetPosition;
        Quaternion startRotation,
            targetRotation;

        if (this.on)
        {
            startPosition = this.offPosition.position;
            startRotation = this.offPosition.rotation;
            targetPosition = this.onPosition.position;
            targetRotation = this.onPosition.rotation;
        }
        else
        {
            startPosition = this.onPosition.position;
            startRotation = this.onPosition.rotation;
            targetPosition = this.offPosition.position;
            targetRotation = this.offPosition.rotation;
        }

        this.currentInterpolationTime = 0.0f;

        while (this.currentInterpolationTime < this.switchTime)
        {
            float percentage = this.currentInterpolationTime / this.switchTime;
            var currentPosition = Vector3.Lerp(startPosition, targetPosition, percentage);
            var currentRotation = Quaternion.Slerp(startRotation, targetRotation, percentage);

            this.leverHandle.transform.SetPositionAndRotation(currentPosition, currentRotation);

            yield return null;
            this.currentInterpolationTime += Time.deltaTime;
        }

        this.leverHandle.transform.SetPositionAndRotation(targetPosition, targetRotation);
        this.interpolating = false;
    }
}
