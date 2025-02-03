using UnityEngine;

public class BubbleSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer backgroundSprite;
    private const string BUBBLE_TAG = "Bubble";

    [Header("Spawn Settings")]
    [SerializeField] private float minSpawnInterval = 3f;
    [SerializeField] private float maxSpawnInterval = 8f;

    [Header("Debug")]
    [SerializeField] private bool showSpawnArea = true;
    [SerializeField] private Color gizmoColor = Color.cyan;

    private float minX;
    private float maxX;
    private float spawnY;
    private float nextSpawnTime;
    private bool canSpawn = true;

    void Start()
    {
        if (backgroundSprite == null)
        {
            Debug.LogError("Background Sprite is not assigned to BubbleSpawner!");
            enabled = false;
            return;
        }

        if (ObjectPoolManager.Instance == null)
        {
            Debug.LogError("ObjectPoolManager not found in scene!");
            enabled = false;
            return;
        }

        CalculateSpawnBounds();
        SetNextSpawnTime();
    }

    void CalculateSpawnBounds()
    {
        Vector2 bgSize = backgroundSprite.bounds.size;
        Vector3 bgPosition = backgroundSprite.transform.position;

        // Calculate spawn boundaries
        minX = bgPosition.x - (bgSize.x / 2f);
        maxX = bgPosition.x + (bgSize.x / 2f);
        spawnY = bgPosition.y - (bgSize.y / 2f) + 1f;

        // Add margin to avoid spawning at exact edges
        float margin = 1f;
        minX += margin;
        maxX -= margin;
    }

    void Update()
    {
        if (Time.time >= nextSpawnTime && canSpawn)
        {
            SpawnBubble();
            SetNextSpawnTime();
        }
    }

    void SetNextSpawnTime()
    {
        nextSpawnTime = Time.time + Random.Range(minSpawnInterval, maxSpawnInterval);
    }

    void SpawnBubble()
    {
        Vector3 spawnPosition = new Vector3(
            Random.Range(minX, maxX),
            spawnY,
            0
        );

        GameObject bubble = ObjectPoolManager.Instance.SpawnFromPool(BUBBLE_TAG, spawnPosition, Quaternion.identity);
        if (bubble != null)
        {
            bubble.transform.SetParent(transform, true);
        }
        else
        {
            Debug.LogWarning("Failed to spawn bubble from pool!");
        }
    }

    public void SetSpawning(bool enabled)
    {
        canSpawn = enabled;
    }

    void OnDrawGizmos()
    {
        if (showSpawnArea && backgroundSprite != null)
        {
            Gizmos.color = gizmoColor;
            Vector3 center = new Vector3((maxX + minX) / 2, spawnY, 0);
            Vector3 size = new Vector3(maxX - minX, 0.5f, 0);
            Gizmos.DrawWireCube(center, size);
        }
    }

    void OnValidate()
    {
        // Ensure minimum spawn interval is not greater than maximum
        if (minSpawnInterval > maxSpawnInterval)
        {
            maxSpawnInterval = minSpawnInterval;
        }
    }
}


