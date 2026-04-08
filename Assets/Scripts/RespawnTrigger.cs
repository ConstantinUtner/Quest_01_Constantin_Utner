using UnityEngine;

public class RespawnTrigger : MonoBehaviour
{
    // The target destination assigned in the Inspector
    [SerializeField]
    private Transform respawnPoint;

    private void OnTriggerEnter(Collider other)
    {
        // Try to get the player's controller
        CharacterController playerController = other.gameObject.GetComponent<CharacterController>();

        // Proceed only if the colliding object is the player
        if (playerController != null)
        {
            Respawn(playerController);
        }
    }

    private void Respawn(CharacterController controller)
    {
        // Disable the controller to allow manual transform changes
        controller.enabled = false;

        // Teleport to the respawn point
        controller.transform.position = respawnPoint.position;

        // Re-enable the controller
        controller.enabled = true;
    }
}
