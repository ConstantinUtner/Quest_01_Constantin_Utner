using UnityEngine;

public class Jewel : MonoBehaviour
{
    private bool collected = false; // Verhindert mehrfaches Einsammeln

    // Setzt das Juwel für einen neuen Versuch zurück
    public void ResetJewel()
    {
        collected = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!collected && other.TryGetComponent<Character>(out Character player))
        {
            collected = true; // Jewel eingesammelt

            // Sieg im UIManager auslösen
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowVictory();
            }
        }
    }
}
