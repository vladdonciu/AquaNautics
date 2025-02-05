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


    [Header("Input Settings")]
    [SerializeField] private FloatingJoystick joystick; // Referință la joystick
    [SerializeField] private bool useMobileControls = false;

    private SubmarineHealth health;
    private UIManager uiManager;
    private bool isGameActive = true;

    void Start()
    {
        health = GetComponent<SubmarineHealth>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = submarineSprite.GetComponent<SpriteRenderer>();
        animator = submarineSprite.GetComponent<Animator>();
        uiManager = FindObjectOfType<UIManager>();

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

        // Verifică dacă panoul de win sau game over este activ
        if (!isGameActive || (uiManager != null && (uiManager.IsGameOver() || uiManager.IsGameWon())))
        {
            rb.linearVelocity = Vector2.zero;
            animator.enabled = false;
            if (bubbleParticles.isPlaying)
            {
                bubbleParticles.Stop();
            }
            return;
        }

        if (health != null && health.currentHealth <= 0)
        {
            rb.linearVelocity = Vector2.zero;
            animator.enabled = false;
            if (bubbleParticles.isPlaying)
            {
                bubbleParticles.Stop();
            }
            return;
        }

        Vector2 movement;

        if (useMobileControls && joystick != null)
        {
            // Folosește input-ul de la joystick
            movement = new Vector2(joystick.Horizontal, joystick.Vertical);
        }
        else
        {
            // Folosește input-ul de la tastatură
            movement = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        }

        if (movement.magnitude > 0.1f)
        {
            rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, movement * moveSpeed, 0.1f);
            animator.enabled = true;

            if (!bubbleParticles.isPlaying)
            {
                float tinyStep = 0.000001f;
                bubbleParticles.Simulate(tinyStep, true, true, false);
                bubbleParticles.Play();
            }

            var emission = bubbleParticles.emission;
            emission.rateOverTime = movement.magnitude * 10f;

            // Verifică direcția pentru flip
            if (movement.x > 0.1f && !isFacingRight)
            {
                Flip();
            }
            else if (movement.x < -0.1f && isFacingRight)
            {
                Flip();
            }
        }
        else
        {
            rb.linearVelocity *= friction;
            animator.enabled = false;

            if (bubbleParticles.isPlaying)
            {
                bubbleParticles.Stop();
            }
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

