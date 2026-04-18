using System.Collections;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    [SerializeField] float fallDelay = 0.35f;
    [SerializeField] float destroyDelay = 2f;

    Rigidbody2D rb;
    bool triggered = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.gravityScale = 1f;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (triggered) return;
        if (!collision.collider.CompareTag("Player")) return;

        triggered = true;
        StartCoroutine(FallRoutine());
    }

    IEnumerator FallRoutine()
    {
        yield return new WaitForSeconds(fallDelay);

        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
        }

        Destroy(gameObject, destroyDelay);
    }
}