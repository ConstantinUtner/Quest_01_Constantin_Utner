using UnityEngine;
using UnityEngine.Audio;

public class Saw : MonoBehaviour
{
    [Header("Rotation")]
    [SerializeField]
    private float spinSpeed = 400f;

    [SerializeField]
    private Vector3 spinAxis = Vector3.forward;

    [Header("Audio")]
    private AudioSource _audioSource;

    [SerializeField]
    private AudioClip idleSound;

    [SerializeField]
    private AudioClip cuttingSound;

    [SerializeField]
    private AudioMixerGroup sfxMixerGroup;
    private bool _isCutting;

    // Speichert den genauen Zeitpunkt, an dem der Spieler die Säge zuletzt berührt hat
    private float _lastTriggerTime;

    [Header("Particles")]
    [SerializeField]
    private ParticleSystem cuttingParticles;

    [Header("Damage Settings")]
    [SerializeField]
    private float damagePerSecond = 30f;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Aktualisiert die Zeitmessung kontinuierlich, solange der Spieler in der Säge steht
            _lastTriggerTime = Time.time;

            var character = other.GetComponentInChildren<Character>();
            if (character != null)
            {
                character.InflictDamage(this.damagePerSecond * Time.fixedDeltaTime);

                // === Q3: Kontinuierliches Blinken bei Schaden ===
                character.TriggerBlink();
            }
        }
    }

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.outputAudioMixerGroup = sfxMixerGroup;
        _audioSource.loop = true;
        _audioSource.playOnAwake = true;

        _isCutting = false;

        if (cuttingParticles != null)
            cuttingParticles.Stop();
    }

    private void Start()
    {
        SetState(false);
        SetAndPlayClip(idleSound);
    }

    private void Update()
    {
        transform.Rotate(spinAxis, spinSpeed * Time.deltaTime);

        // === BUGFIX: Status zurücksetzen nach einem Respawn ====
        // Problem: Wenn der Spieler in der Säge stirbt und vom Respawn-Skript wegteleportiert wird,
        // registriert Unity kein "OnTriggerExit". Die Säge würde endlos weiter sägen (Sound & Partikel).
        // Lösung: Wenn die Säge aktiv ist (_isCutting) ABER der letzte Kontakt (_lastTriggerTime)
        // länger als 0.1 Sekunden her ist, wissen wir: Der Spieler ist weg. Wir schalten die Säge ab.
        if (_isCutting && Time.time - _lastTriggerTime > 0.1f)
        {
            SetState(false);
        }
        // ========================================================================================
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Startet die Zeitmessung im Moment der ersten Berührung
            _lastTriggerTime = Time.time;
            SetState(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Wird aufgerufen, wenn der Spieler normal aus der Säge herausläuft (nicht bei Teleport/Respawn)
        if (other.CompareTag("Player"))
            SetState(false);
    }

    private void SetState(bool cutting)
    {
        if (_isCutting == cutting)
            return;

        if (cutting)
        {
            _isCutting = true;
            if (cuttingSound != null)
                SetAndPlayClip(cuttingSound);
            if (cuttingParticles != null)
                cuttingParticles.Play();
        }
        else
        {
            _isCutting = false;
            if (idleSound != null)
                SetAndPlayClip(idleSound);
            if (cuttingParticles != null)
                cuttingParticles.Stop();
        }
    }

    private void SetAndPlayClip(AudioClip clip)
    {
        _audioSource.clip = clip;
        _audioSource.Play();
    }
}
