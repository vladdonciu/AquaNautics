using UnityEngine;

public class SubmarineController : MonoBehaviour
{
    private Rigidbody2D rb;
    public float moveSpeed = 5f;
    public float friction = 0.95f;
    public float maxRotationAngle = 45f;
    private bool isFacingRight = true;

    [Header("References")]
    [SerializeField] private Transform pivotPoint;
    [SerializeField] private Transform submarineSprite;
    private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;
    [SerializeField] private ParticleSystem bubbleParticles;

    private SubmarineHealth health;

    void Start()
    {
        health = GetComponent<SubmarineHealth>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = submarineSprite.GetComponent<SpriteRenderer>();
        animator = submarineSprite.GetComponent<Animator>();

        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    void FixedUpdate()
    {
        HandleMovement();
        HandleRotation();
    }

    void HandleMovement()
    {
        // Check if health is 0 before allowing movement
        if (health != null && health.currentHealth <= 0)
        {
            // Stop all movement and particles
            rb.linearVelocity = Vector2.zero;
            animator.enabled = false;
            if (bubbleParticles.isPlaying)
            {
                bubbleParticles.Stop();
            }
            return;
        }

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector2 movement = new Vector2(horizontalInput, verticalInput).normalized;

        if (movement.magnitude > 0.1f)
        {
            rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, movement * moveSpeed, 0.1f);

            // Start animation
            animator.enabled = true;

            if (!bubbleParticles.isPlaying)
            {
                float tinyStep = 0.000001f;
                bubbleParticles.Simulate(tinyStep, true, true, false);
                bubbleParticles.Play();
            }

            var emission = bubbleParticles.emission;
            emission.rateOverTime = movement.magnitude * 10f;
        }
        else
        {
            rb.linearVelocity *= friction;

            // Stop animation
            animator.enabled = false;

            if (bubbleParticles.isPlaying)
            {
                bubbleParticles.Stop();
            }
        }

        if (horizontalInput > 0.1f && !isFacingRight)
        {
            Flip();
        }
        else if (horizontalInput < -0.1f && isFacingRight)
        {
            Flip();
        }
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;

        float currentTilt = transform.rotation.eulerAngles.z;
        if (currentTilt > 180) currentTilt -= 360;

        if (isFacingRight)
        {
            transform.rotation = Quaternion.Euler(0, 0, currentTilt);
            bubbleParticles.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 180, currentTilt);
            bubbleParticles.transform.localRotation = Quaternion.Euler(0, 180, 0);
        }

        if (Mathf.Abs(rb.linearVelocity.magnitude) > 0.1f)
        {
            bubbleParticles.Stop();
            float tinyStep = 0.000001f;
            bubbleParticles.Simulate(tinyStep, true, true, false);
            bubbleParticles.Play();
        }
    }

    void HandleRotation()
    {
        // Check if health is 0 before allowing rotation
        if (health != null && health.currentHealth <= 0)
        {
            return;
        }

        float tiltAngle = Mathf.Clamp(rb.linearVelocity.y * 3f, -maxRotationAngle, maxRotationAngle);

        if (isFacingRight)
        {
            transform.rotation = Quaternion.Euler(0, 0, tiltAngle);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 180, tiltAngle);
        }
    }
}




    //void UpdateUI()
    //{
    //    if (UIManager.Instance != null)
    //    {
    //        UIManager.Instance.UpdateHealth(currentHealth, maxHealth);
    //        UIManager.Instance.UpdateTime(currentTime, maxTime);
    //    }
    //}

    //void GameOver()
    //{
    //    Debug.Log("Game Over!");
    //}

//    public void TakeDamage(float damage)
//    {
//        currentHealth = Mathf.Max(0, currentHealth - damage);
//        UpdateUI();
//    }

//    public void AddHealth(float healing)
//    {
//        currentHealth = Mathf.Min(maxHealth, currentHealth + healing);
//        UpdateUI();
//    }

//    public void AddTime(float additionalTime)
//    {
//        float oldTime = currentTime;
//        currentTime = Mathf.Min(currentTime + additionalTime, maxTime);
//        float actualTimeAdded = currentTime - oldTime;
//        Debug.Log($"Adding time: {additionalTime}, Old time: {oldTime}, New time: {currentTime}, Actual added: {actualTimeAdded}");
//        UpdateUI();
//    }

