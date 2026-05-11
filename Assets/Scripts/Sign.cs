using UnityEngine;
using UnityEngine.InputSystem;

public class Sign : MonoBehaviour
{
    [SerializeField]
    private GameObject dialogBox;
    private bool canInteract;
    private InputAction interactAction;

    private void Start()
    {
        this.interactAction = InputSystem.actions.FindAction("Attack");
        this.interactAction.performed += ToggleDialogBox;

        this.dialogBox.SetActive(false);
        this.canInteract = false;
    }

    private void ToggleDialogBox(InputAction.CallbackContext cxt)
    {
        if (this.canInteract)
        {
            bool isOpening = !this.dialogBox.activeInHierarchy;
            this.dialogBox.SetActive(isOpening);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;
        this.canInteract = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;
        this.canInteract = false;
        this.dialogBox.SetActive(false);
    }
}
