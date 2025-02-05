using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI fishCountText;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Slider timeSlider;

    [Header("Slider Colors")]
    public Color maxColor = Color.green;
    public Color minColor = Color.red;

    [Header("Fish Settings")]
    [SerializeField] private int totalFishNeeded = 20;
    private int currentFishCount = 0;

    [Header("UI Panels")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject warningImage;

    [Header("Audio")]
    [SerializeField] private AudioClip winSound;
    [SerializeField] private AudioClip gameOverSound;
    [SerializeField] private AudioClip warningSound;
    private AudioSource audioSource;
    private AudioSource warningAudioSource;

    [Header("Game Controls")]
    [SerializeField] private FloatingJoystick joystick;
    [SerializeField] private SubmarineController submarine;

    private bool isGameOver = false;
    private bool isGameWon = false;

    public bool IsGameOver() => isGameOver;
    public bool IsGameWon() => isGameWon;

    private Image healthFillImage;
    private Image timeFillImage;

    public static UIManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializeComponents();
            InitializeAudio();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        ResetUI();
    }

    void InitializeAudio()
    {
        // Main audio source for one-shot sounds
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.volume = 0.5f;

        // Warning audio source for looping
        warningAudioSource = gameObject.AddComponent<AudioSource>();
        warningAudioSource.playOnAwake = false;
        warningAudioSource.loop = true;
        warningAudioSource.clip = warningSound;
        warningAudioSource.volume = 0.5f;
    }

    void InitializeComponents()
    {
        if (healthSlider != null && healthSlider.fillRect != null)
        {
            healthFillImage = healthSlider.fillRect.GetComponent<Image>();
        }

        if (timeSlider != null && timeSlider.fillRect != null)
        {
            timeFillImage = timeSlider.fillRect.GetComponent<Image>();
        }

        if (warningImage != null)
        {
            warningImage.SetActive(false);
        }
    }

    public void ResetUI()
    {

        isGameOver = false;
        isGameWon = false;
        currentFishCount = 0;
        UpdateFishCount(0, totalFishNeeded);
        EnableControls();


        if (healthSlider != null)
        {
            healthSlider.value = 1f;
            if (healthFillImage != null)
            {
                healthFillImage.color = maxColor;
            }
        }

        if (timeSlider != null)
        {
            timeSlider.value = 1f;
            if (timeFillImage != null)
            {
                timeFillImage.color = maxColor;
            }
        }

        if (healthText != null) healthText.text = "100%";
        if (timeText != null) timeText.text = "60s";

        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (winPanel != null) winPanel.SetActive(false);
        if (warningImage != null) warningImage.SetActive(false);

        // Stop warning sound if playing
        if (warningAudioSource != null && warningAudioSource.isPlaying)
        {
            warningAudioSource.Stop();
        }
    }

    public void UpdateHealth(float currentHealth, float maxHealth)
    {
        if (currentHealth < 0) currentHealth = 0;
        float normalizedHealth = maxHealth > 0 ? currentHealth / maxHealth : 0;

        if (healthText != null)
        {
            healthText.text = $" {Mathf.CeilToInt(currentHealth)}%";
        }

        if (healthSlider != null)
        {
            healthSlider.value = normalizedHealth;
            if (healthFillImage != null)
            {
                healthFillImage.color = Color.Lerp(minColor, maxColor, normalizedHealth);
            }
        }
    }

    public void UpdateTime(float currentTime, float maxTime)
    {
        if (currentTime < 0) currentTime = 0;
        float normalizedTime = maxTime > 0 ? currentTime / maxTime : 0;

        // Handle warning sound and image
        if (currentTime <= 0)
        {
            if (!warningAudioSource.isPlaying)
            {
                warningAudioSource.Play();
            }
            if (warningImage != null)
            {
                warningImage.SetActive(true);
            }
        }
        else
        {
            if (warningAudioSource.isPlaying)
            {
                warningAudioSource.Stop();
            }
            if (warningImage != null)
            {
                warningImage.SetActive(false);
            }
        }

        if (timeText != null)
        {
            timeText.text = $"{Mathf.CeilToInt(currentTime)}s";
        }

        if (timeSlider != null)
        {
            timeSlider.value = normalizedTime;
            if (timeFillImage != null)
            {
                timeFillImage.color = Color.Lerp(minColor, maxColor, normalizedTime);
            }
        }
    }

    public void UpdateFishCount(int current, int total)
    {
        currentFishCount = current;
        if (fishCountText != null)
        {
            fishCountText.text = $"{current}/{total}";
        }
    }

    public void IncrementFishCount()
    {
        currentFishCount++;
        UpdateFishCount(currentFishCount, totalFishNeeded);

        if (currentFishCount >= totalFishNeeded)
        {
            ShowWinPanel();
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
    }


    public void ShowGameOverPanel()
    {
        if (gameOverPanel != null)
        {
            isGameOver = true;
            gameOverPanel.SetActive(true);
            DisableControls();
            PlaySound(gameOverSound);

            if (warningAudioSource.isPlaying)
            {
                warningAudioSource.Stop();
            }
            if (warningImage != null)
            {
                warningImage.SetActive(false);
            }
        }
    }

    private void ShowWinPanel()
    {
        if (winPanel != null)
        {
            isGameWon = true;
            winPanel.SetActive(true);
            DisableControls();
            PlaySound(winSound);

            if (warningAudioSource.isPlaying)
            {
                warningAudioSource.Stop();
            }
            if (warningImage != null)
            {
                warningImage.SetActive(false);
            }
        }
    }


    public void RestartLevel()
    {
        ResetUI();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void NextLevel()
    {
        ResetUI();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void GoToHomeScene()
    {
        ResetUI();
        SceneManager.LoadScene("Home");
    }

    public void SetSoundVolume(float volume)
    {
        volume = Mathf.Clamp01(volume);
        if (audioSource != null)
        {
            audioSource.volume = volume;
        }
        if (warningAudioSource != null)
        {
            warningAudioSource.volume = volume;
        }
    }

    private void DisableControls()
    {
        if (joystick != null)
        {
            joystick.gameObject.SetActive(false);
        }
    }

    private void EnableControls()
    {
        if (joystick != null)
        {
            joystick.gameObject.SetActive(true);
        }
    }

    public int GetCurrentFishCount() => currentFishCount;
    public int GetTotalFishNeeded() => totalFishNeeded;
}

