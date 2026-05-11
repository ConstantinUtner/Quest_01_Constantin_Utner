using UnityEngine;
using UnityEngine.Audio;

public class Coin : MonoBehaviour
{
    [SerializeField]
    private float rotationSpeed = 90f;

    [SerializeField]
    private AudioClip collectSound;

    [SerializeField]
    private AudioMixerGroup sfxMixerGroup;

    void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }

    private bool isCollected = false;

    private void OnTriggerEnter(Collider other)
    {
        if (isCollected || !other.CompareTag("Player"))
            return;

        // Mark the coin as collected to prevent multiple triggers
        isCollected = true;
        UIManager.Instance.CollectCoin();

        if (collectSound != null)
        {
            var go = new GameObject("CoinCollectSound");
            go.transform.position = transform.position;
            var source = go.AddComponent<AudioSource>();
            source.clip = collectSound;
            source.outputAudioMixerGroup = sfxMixerGroup;
            source.Play();
            Destroy(go, collectSound.length);
        }

        Destroy(gameObject);
    }
}
