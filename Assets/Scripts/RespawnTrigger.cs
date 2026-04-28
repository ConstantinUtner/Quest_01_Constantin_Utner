using UnityEngine;

public class RespawnTrigger : MonoBehaviour
{
    [SerializeField]
    private Transform respawnPoint;

    private void OnTriggerEnter(Collider other)
    {
        CharacterController playerController = other.gameObject.GetComponent<CharacterController>();

        if (playerController != null)
        {
            Respawn(playerController);
        }
    }

    private void Respawn(CharacterController controller)
    {
        controller.enabled = false;

        controller.transform.position = respawnPoint.position;

        controller.enabled = true;
    }
}
