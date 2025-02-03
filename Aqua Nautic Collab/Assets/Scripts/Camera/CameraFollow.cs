using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform submarine;
    public float smoothSpeed = 0.125f;
    public Vector3 offset = new Vector3(0, 0, -10);

    // Add damping and boundaries
    public float dampingTime = 0.15f;
    private Vector3 velocity = Vector3.zero;

    void LateUpdate()
    {
        if (submarine != null)
        {
            // Calculate desired position
            Vector3 desiredPosition = submarine.position + offset;

            // Use SmoothDamp instead of Lerp for smoother movement
            transform.position = Vector3.SmoothDamp(
                transform.position,
                desiredPosition,
                ref velocity,
                dampingTime
            );
        }
    }
}
