using UnityEngine;

public class MineSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer backgroundSprite;
    private const string MINE_TAG = "Mine";

    [Header("Spawn Settings")]
    public float minSpawnTime = 3f;
    public float maxSpawnTime = 8f;

    [Header("Mine Movement")]
    public float minFallSpeed = 1.5f;
    public float maxFallSpeed = 3f;
    public float minOscillation = 0.3f;
    public float maxOscillation = 0.8f;

    [Header("Debug")]
    [SerializeField] private bool showSpawnArea = true;
    [SerializeField] private Color gizmoColor = Color.red;

    private float spawnHeight;
    private float minX;
    private float maxX;
    private float nextSpawnTime;
    private bool canSpawn = true;

    void Start()
    {
        if (backgroundSprite == null)
        {
            Debug.LogError("Background Sprite is not assigned to MineSpawner!");
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

        minX = bgPosition.x - (bgSize.x / 2f);
        maxX = bgPosition.x + (bgSize.x / 2f);
        spawnHeight = bgPosition.y + (bgSize.y / 2f) + 1f;

        float margin = 1f;
        minX += margin;
        maxX -= margin;
    }

    void Update()
    {
        if (Time.time >= nextSpawnTime && canSpawn)
        {
            SpawnMine();
            SetNextSpawnTime();
        }
    }

    void SetNextSpawnTime()
    {
        nextSpawnTime = Time.time + Random.Range(minSpawnTime, maxSpawnTime);
    }

    void SpawnMine()
    {
        Vector3 spawnPosition = new Vector3(
            Random.Range(minX, maxX),
            spawnHeight,
            0
        );

        GameObject mineObject = ObjectPoolManager.Instance.SpawnFromPool(MINE_TAG, spawnPosition, Quaternion.identity);
        if (mineObject != null)
        {
            Mine mine = mineObject.GetComponent<Mine>();
            if (mine != null)
            {
                mine.Initialize(
                    Random.Range(minFallSpeed, maxFallSpeed),
                    Random.Range(minOscillation, maxOscillation),
                    Random.Range(1.5f, 2.5f)
                );
            }
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
            Vector3 center = new Vector3((maxX + minX) / 2, spawnHeight, 0);
            Vector3 size = new Vector3(maxX - minX, 0.5f, 0);
            Gizmos.DrawWireCube(center, size);
        }
    }

    void OnValidate()
    {
        if (minSpawnTime > maxSpawnTime)
        {
            maxSpawnTime = minSpawnTime;
        }
    }
}

