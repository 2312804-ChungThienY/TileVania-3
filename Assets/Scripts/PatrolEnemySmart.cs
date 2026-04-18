using UnityEngine;

public class PatrolEnemySmart : MonoBehaviour
{
    [SerializeField] Transform leftPoint;
    [SerializeField] Transform rightPoint;
    [SerializeField] float moveSpeed = 2.8f;
    [SerializeField] float waitTimeAtPoint = 0.2f;

    Rigidbody2D rb;
    Vector3 startScale;
    bool movingRight = true;
    float waitCounter = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startScale = transform.localScale;

        if (leftPoint != null) leftPoint.SetParent(null);
        if (rightPoint != null) rightPoint.SetParent(null);
    }

    void Update()
    {
        if (rb == null || leftPoint == null || rightPoint == null) return;

        if (waitCounter > 0f)
        {
            waitCounter -= Time.deltaTime;
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        Transform target = movingRight ? rightPoint : leftPoint;
        float dir = Mathf.Sign(target.position.x - transform.position.x);

        rb.linearVelocity = new Vector2(dir * moveSpeed, rb.linearVelocity.y);

        if (Mathf.Abs(transform.position.x - target.position.x) < 0.08f)
        {
            movingRight = !movingRight;
            waitCounter = waitTimeAtPoint;
        }

        if (Mathf.Abs(dir) > 0.01f)
        {
            transform.localScale = new Vector3(
                Mathf.Sign(dir) * Mathf.Abs(startScale.x),
                startScale.y,
                startScale.z
            );
        }
    }
}