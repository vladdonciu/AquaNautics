using UnityEngine;

public class SubmarineCollector : MonoBehaviour
{
    private SubmarineHealth health;
    public int fishCount = 0;

    void Start()
    {
        health = GetComponentInParent<SubmarineHealth>();
        if (health == null)
        {
            Debug.LogError("SubmarineHealth component not found! Please add SubmarineHealth to the submarine object.");
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("AirBubble"))
        {
            health.AddTime(5f);
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("Fish"))
        {
            fishCount++;
            Destroy(other.gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Mine"))
        {
            health.TakeDamage(20f);
            health.AddTime(-5f); // Penalizare de timp
        }
    }
}

