using UnityEngine;

public class Mine : MonoBehaviour, IPooledObject
{
    private const string MINE_TAG = "Mine";

    [Header("Mine Settings")]
    public float damage = 20f;
    public float fallSpeed = 2f;
    public float oscillationSpeed = 2f;
    public float oscillationAmount = 0.5f;

    [Header("Audio")]
    [SerializeField] private AudioClip explosionSound;
    private AudioSource audioSource;

    private Vector3 startPosition;
    private float randomOffset;
    private Animator animator;
    private bool hasExploded = false;
    private Collider2D mineCollider;

    private SubmarineHealth submarineHealth;

    void Awake()
    {
        animator = GetComponent<Animator>();
        mineCollider = GetComponent<Collider2D>();

        // Initialize audio source
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.volume = 0.5f; // Adjust default volume as needed
    }

    public void OnObjectSpawn()
    {
        InitializeMine();
    }

    public void Initialize(float newFallSpeed, float newOscillation, float newOscillationSpeed)
    {
        fallSpeed = newFallSpeed;
        oscillationAmount = newOscillation;
        oscillationSpeed = newOscillationSpeed;
    }

    void InitializeMine()
    {
        hasExploded = false;
        startPosition = transform.position;
        randomOffset = Random.Range(0f, 2f * Mathf.PI);

        if (mineCollider != null)
        {
            mineCollider.enabled = true;
        }
    }

    void Update()
    {
        if (!hasExploded)
        {
            transform.position += Vector3.down * fallSpeed * Time.deltaTime;

            float xOffset = Mathf.Sin((Time.time + randomOffset) * oscillationSpeed) * oscillationAmount;
            Vector3 newPos = transform.position;
            newPos.x = startPosition.x + xOffset;
            transform.position = newPos;

            startPosition = new Vector3(startPosition.x, transform.position.y, transform.position.z);

            if (transform.position.y < -10f)
            {
                gameObject.SetActive(false);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Submarine") && !hasExploded)
        {
            SubmarineHealth submarine = other.GetComponent<SubmarineHealth>();
            if (submarine != null)
            {
                Explode(submarine);
            }
        }
    }

    void Explode(SubmarineHealth submarine)
    {
        hasExploded = true;
        mineCollider.enabled = false;
        submarine.TakeDamage(damage);

        // Play explosion sound
        if (explosionSound != null && audioSource != null)
        {
            audioSource.clip = explosionSound;
            audioSource.Play();
        }

        if (animator != null)
        {
            animator.Play("Explosion", 0, 0f);
            float explosionLength = GetAnimationClipLength("Explosion");

            // Use the longer of animation length or audio length
            float delay = explosionSound != null ?
                Mathf.Max(explosionLength, explosionSound.length) :
                explosionLength;

            Invoke(nameof(DisableMine), delay);
        }
        else
        {
            DisableMine();
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

    void DisableMine()
    {
        gameObject.SetActive(false);
    }

    void OnDisable()
    {
        if (ObjectPoolManager.Instance != null)
        {
            ObjectPoolManager.Instance.ReturnToPool(MINE_TAG, gameObject);
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
