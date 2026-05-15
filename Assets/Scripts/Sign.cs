using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization;
using UnityEngine.UIElements;

public class Sign : MonoBehaviour
{
    [SerializeField]
    private GameObject dialogBox;
    private bool canInteract;
    private InputAction interactAction;

    [SerializeField]
    private LocalizedString dialogText;

    private void Start()
    {
        this.interactAction = InputSystem.actions.FindAction("Attack");

        // Steuerung aktivieren, da Tastendrücke sonst ignoriert werden (Bugfix Input wurde nach Sign Activation ignoriert)
        if (this.interactAction != null && this.interactAction.actionMap != null)
        {
            this.interactAction.actionMap.Enable();
            this.interactAction.performed += ToggleDialogBox;
        }

        this.dialogBox.SetActive(false);
        this.canInteract = false;
    }

    private void ToggleDialogBox(InputAction.CallbackContext cxt)
    {
        if (this.canInteract)
        {
            if (this.dialogBox.activeInHierarchy)
            {
                this.dialogBox.SetActive(false);
            }
            else
            {
                this.dialogBox.SetActive(true);

                var uiDocument = this.dialogBox.GetComponent<UIDocument>();
                var label = uiDocument.rootVisualElement.Q<Label>();
                label.text = this.dialogText.GetLocalizedString();
            }
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
