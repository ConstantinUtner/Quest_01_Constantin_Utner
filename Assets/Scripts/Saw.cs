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

    [Header("Particles")]
    [SerializeField]
    private ParticleSystem cuttingParticles;

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
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            SetState(true);
    }

    private void OnTriggerExit(Collider other)
    {
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
