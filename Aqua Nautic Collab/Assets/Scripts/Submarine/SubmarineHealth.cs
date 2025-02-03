using UnityEngine;


public class SubmarineHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;
    public float maxTime = 60f;
    public float currentTime;
    private float healthDecreaseInterval = 1f;
    private float healthDecreaseAmount = 5f;
    private float healthDecreaseTimer = 0f;
    private bool isGameOver = false;

    void Start()
    {
        ResetStatus();
    }

    public void ResetStatus()
    {
        currentHealth = maxHealth;
        currentTime = maxTime;
        isGameOver = false;
        healthDecreaseTimer = 0f;
        UpdateUI();
    }

    void Update()
    {
        if (isGameOver) return;

        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            currentTime = Mathf.Max(0, currentTime);
        }

        if (currentTime <= 0)
        {
            healthDecreaseTimer += Time.deltaTime;
            if (healthDecreaseTimer >= healthDecreaseInterval)
            {
                TakeDamage(healthDecreaseAmount);
                healthDecreaseTimer = 0f;
            }
        }

        if (currentHealth <= 0)
        {
            GameOver();
        }

        UpdateUI();
    }

    void GameOver()
    {
        if (!isGameOver)
        {
            isGameOver = true;
            Debug.Log("Game Over!");

            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowGameOverPanel();
            }
        }
    }

    public void AddTime(float timeBonus)
    {
        if (!isGameOver)
        {
            currentTime = Mathf.Min(currentTime + timeBonus, maxTime);
            UpdateUI();
        }
    }

    public void TakeDamage(float damage)
    {
        if (!isGameOver)
        {
            currentHealth -= damage;
            currentHealth = Mathf.Max(currentHealth, 0);

            if (currentHealth <= 0)
            {
                GameOver();
            }
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateHealth(currentHealth, maxHealth);
            UIManager.Instance.UpdateTime(currentTime, maxTime);
        }
    }
}
