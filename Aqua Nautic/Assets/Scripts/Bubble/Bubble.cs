using UnityEngine;

public class Bubble : MonoBehaviour, IPooledObject
{
    private const string BUBBLE_TAG = "Bubble";

    [Header("Movement Settings")]
    public float timeBonus = 5f;
    public float floatSpeed = 1f;
    public float oscillationSpeed = 2f;
    public float oscillationAmount = 0.5f;

    [Header("Components")]
    private Animator animator;
    private Collider2D bubbleCollider;

    [Header("Audio")]
    [SerializeField] private AudioClip popSound;
    private AudioSource audioSource;

    private Vector3 startPosition;
    private float randomOffset;
    private Camera mainCamera;
    private float topBound;
    private bool hasExploded = false;

    void Awake()
    {
        // Initialize animator and collider
        animator = GetComponent<Animator>();
        bubbleCollider = GetComponent<Collider2D>();

        // Initialize audio source
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.volume = 0.5f; // Adjust default volume as needed

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            topBound = mainCamera.orthographicSize + 1f;
        }
    }

    public void OnObjectSpawn()
    {
        InitializeBubble();
    }

    void InitializeBubble()
    {
        // Reset bubble properties
        hasExploded = false;
        startPosition = transform.position;
        randomOffset = Random.Range(0f, 2f * Mathf.PI);

        if (bubbleCollider != null)
            bubbleCollider.enabled = true;

        if (animator != null)
        {
            animator.Play("Idle", 0, 0f); // Play the idle animation
        }
    }

    void Update()
    {
        if (!hasExploded)
        {
            // Vertical movement
            transform.position += Vector3.up * floatSpeed * Time.deltaTime;

            // Oscillating horizontal movement
            float xOffset = Mathf.Sin((Time.time + randomOffset) * oscillationSpeed) * oscillationAmount;
            transform.position = new Vector3(startPosition.x + xOffset, transform.position.y, transform.position.z);

            // Disable bubble if it goes off-screen
            if (transform.position.y > topBound)
            {
                gameObject.SetActive(false);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (hasExploded) return;

        SubmarineHealth submarine = null;

        // Check if the collider belongs to a submarine
        if (other.CompareTag("Submarine"))
        {
            submarine = other.GetComponent<SubmarineHealth>();
        }
        else if (other.gameObject.transform.parent != null && other.gameObject.transform.parent.CompareTag("Submarine"))
        {
            submarine = other.gameObject.transform.parent.GetComponent<SubmarineHealth>();
        }

        // Explode bubble and grant bonus time
        if (submarine != null)
        {
            ExplodeBubble(submarine);
        }
    }

    void ExplodeBubble(SubmarineHealth submarine)
    {
        if (hasExploded) return;

        hasExploded = true;
        bubbleCollider.enabled = false;
        submarine.AddTime(timeBonus);

        // Play pop sound
        if (popSound != null && audioSource != null)
        {
            audioSource.clip = popSound;
            audioSource.Play();
        }

        if (animator != null)
        {
            animator.Play("BubbleExplode", 0, 0f);
            float explosionLength = GetAnimationClipLength("BubbleExplode");

            // Use the longer of animation length or audio length
            float delay = popSound != null ?
                Mathf.Max(explosionLength, popSound.length) :
                explosionLength;

            Invoke(nameof(DisableBubble), delay);
        }
        else
        {
            Debug.LogWarning("Animator is null! Disabling bubble immediately.");
            DisableBubble();
        }
    }

    float GetAnimationClipLength(string clipName)
    {
        if (animator == null || animator.runtimeAnimatorController == null) return 0.5f;

        foreach (var clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == clipName)
            {
                return clip.length;
            }
        }

        Debug.LogWarning($"Animation clip '{clipName}' not found!");
        return 0.5f; // Default length if clip is not found
    }

    void DisableBubble()
    {
        gameObject.SetActive(false);
    }

    void OnDisable()
    {
        // Return the bubble to the object pool
        if (ObjectPoolManager.Instance != null)
        {
            ObjectPoolManager.Instance.ReturnToPool(BUBBLE_TAG, gameObject);
        }
    }

    // Optional: Method to adjust sound volume
    public void SetSoundVolume(float volume)
    {
        if (audioSource != null)
        {
            audioSource.volume = Mathf.Clamp01(volume);
        }
    }
}
