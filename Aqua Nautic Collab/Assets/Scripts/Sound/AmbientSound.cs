using UnityEngine;
using UnityEngine.Audio;

public class AmbientSound : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] private AudioClip ambientClip;
    [SerializeField] [Range(0f, 1f)] private float volume = 0.5f;
    [SerializeField] private bool playOnAwake = true;

    private AudioSource audioSource;

    void Awake()
    {
        // Create and configure AudioSource
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = ambientClip;
        audioSource.loop = true;
        audioSource.volume = volume;
        audioSource.playOnAwake = playOnAwake;

        if (playOnAwake)
        {
            audioSource.Play();
        }
    }

    public void PlayAmbient()
    {
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    public void StopAmbient()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    public void SetVolume(float newVolume)
    {
        volume = Mathf.Clamp01(newVolume);
        if (audioSource != null)
        {
            audioSource.volume = volume;
        }
    }
}
