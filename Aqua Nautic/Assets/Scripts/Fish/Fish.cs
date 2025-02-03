using UnityEngine;
using System.Collections;

public class Fish : MonoBehaviour
{
    [Header("Movement Settings")]
    public float swimSpeed = 2f;
    public float wanderRadius = 3f;
    public float changeDirectionInterval = 2f;
    public float verticalMovementStrength = 1f;

    [Header("Audio")]
    [SerializeField] private AudioClip collectSound;
    private AudioSource audioSource;

    [Header("References")]
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private float nextDirectionChange;
    private bool isFacingRight = true;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        startPosition = transform.position;
        SetNewTargetPosition();
        InitializeAudio();
    }

    void InitializeAudio()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.volume = 0.5f;
    }

    void Update()
    {
        if (Time.time >= nextDirectionChange)
        {
            SetNewTargetPosition();
        }

        // Calculate distance to target
        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);

        if (distanceToTarget > 0.1f)
        {
            // Move towards target
            Vector3 moveDirection = (targetPosition - transform.position).normalized;
            transform.position += moveDirection * swimSpeed * Time.deltaTime;

            // Add slight vertical movement using sine wave
            transform.position += Vector3.up * Mathf.Sin(Time.time) * verticalMovementStrength * Time.deltaTime;

            // Handle flipping
            if (moveDirection.x > 0 && !isFacingRight)
            {
                Flip();
            }
            else if (moveDirection.x < 0 && isFacingRight)
            {
                Flip();
            }
        }
    }

    void SetNewTargetPosition()
    {
        // Generate random point within radius
        Vector2 randomPoint = Random.insideUnitCircle * wanderRadius;
        targetPosition = startPosition + new Vector3(randomPoint.x, randomPoint.y, 0);

        // Set next direction change time
        nextDirectionChange = Time.time + changeDirectionInterval + Random.Range(-0.5f, 0.5f);
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Submarine"))
        {
            PlayCollectSound();
            UIManager.Instance.IncrementFishCount();
            StartCoroutine(DestroyAfterSound());
        }
    }

    void PlayCollectSound()
    {
        if (collectSound != null && audioSource != null)
        {
            // Create a temporary audio source at this position
            AudioSource.PlayClipAtPoint(collectSound, transform.position, audioSource.volume);
        }
    }

    IEnumerator DestroyAfterSound()
    {
        // Make fish invisible but keep it for sound
        if (spriteRenderer != null)
            spriteRenderer.enabled = false;

        // Wait for sound to finish if there is one
        if (collectSound != null)
            yield return new WaitForSeconds(collectSound.length);

        Destroy(gameObject);
    }

    // Optional: Visualize movement radius in editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, wanderRadius);
    }

    public void SetSoundVolume(float volume)
    {
        if (audioSource != null)
        {
            audioSource.volume = Mathf.Clamp01(volume);
        }
    }
}
