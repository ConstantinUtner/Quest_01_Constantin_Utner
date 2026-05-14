using UnityEngine;

public class RespawnTrigger : MonoBehaviour
{
    [SerializeField]
    private Transform respawnPoint;

    private void OnTriggerEnter(Collider other)
    {
        Character character = other.gameObject.GetComponent<Character>();

        if (character != null)
        {
            // Zieht dem Spieler maximales Leben ab, was ihn sofort tötet (statt Respawn)
            character.InflictDamage(character.GetMaxHealth());
        }
    }

    private void Respawn(CharacterController controller)
    {
        controller.enabled = false;

        controller.transform.position = respawnPoint.position;

        controller.enabled = true;
    }
}
