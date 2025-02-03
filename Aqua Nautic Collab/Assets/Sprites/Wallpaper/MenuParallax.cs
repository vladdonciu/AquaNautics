using UnityEngine;

public class MenuParallax : MonoBehaviour
{
    [System.Serializable]
    public class ParallaxLayer
    {
        public Transform layerTransform;
        public float parallaxIntensity = 20f;
        public Vector2 customOffset = Vector2.one; // Permite offset diferit pe X și Y
    }

    [Header("Parallax Settings")]
    [SerializeField] private ParallaxLayer[] parallaxLayers;
    [SerializeField] private float smoothness = 5f;
    [SerializeField] private float maxOffset = 50f;

    private Vector3[] initialPositions;
    private Vector3[] targetPositions;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;

        // Inițializează arrayurile
        initialPositions = new Vector3[parallaxLayers.Length];
        targetPositions = new Vector3[parallaxLayers.Length];

        // Salvează pozițiile inițiale
        for (int i = 0; i < parallaxLayers.Length; i++)
        {
            if (parallaxLayers[i].layerTransform != null)
            {
                initialPositions[i] = parallaxLayers[i].layerTransform.position;
            }
        }
    }

    void Update()
    {
        // Get mouse position
        Vector2 mousePosition = Input.mousePosition;

        // Create screen boundaries
        Rect screenRect = new Rect(0, 0, Screen.width, Screen.height);

        // Check if mouse is outside the window
        if (!screenRect.Contains(mousePosition))
        {
            // Return layers to their initial positions smoothly
            for (int i = 0; i < parallaxLayers.Length; i++)
            {
                if (parallaxLayers[i].layerTransform != null)
                {
                    parallaxLayers[i].layerTransform.position = Vector3.Lerp(
                        parallaxLayers[i].layerTransform.position,
                        initialPositions[i],
                        Time.deltaTime * smoothness
                    );
                }
            }
            return;
        }

        // Rest of your existing Update code for parallax effect...
        Vector2 viewportPoint = mainCamera.ScreenToViewportPoint(mousePosition);
        float offsetX = (viewportPoint.x - 0.5f);
        float offsetY = (viewportPoint.y - 0.5f);

        // Update each layer
        for (int i = 0; i < parallaxLayers.Length; i++)
        {
            if (parallaxLayers[i].layerTransform == null) continue;

            Vector3 offset = new Vector3(
                -offsetX * parallaxLayers[i].customOffset.x,
                -offsetY * parallaxLayers[i].customOffset.y,
                0
            ) * parallaxLayers[i].parallaxIntensity;

            offset = Vector3.ClampMagnitude(offset, maxOffset);
            targetPositions[i] = initialPositions[i] + offset;

            parallaxLayers[i].layerTransform.position = Vector3.Lerp(
                parallaxLayers[i].layerTransform.position,
                targetPositions[i],
                Time.deltaTime * smoothness
            );
        }
    }


    // Optional: Reset positions when mouse exits
    void OnDisable()
    {
        for (int i = 0; i < parallaxLayers.Length; i++)
        {
            if (parallaxLayers[i].layerTransform != null)
            {
                parallaxLayers[i].layerTransform.position = initialPositions[i];
            }
        }
    }
}
