using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
public class SeekerAI : MonoBehaviour
{
    [Header("Movement Settings")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 4f;
    public float obstacleAvoidanceDistance = 1f;

    [Header("Patrol Points")]
    public Transform[] patrolPoints;
    private int currentPoint = 0;

    [Header("Detection Settings")]
    public float loseSightDelay = 2f;

    [Header("Visual Settings")]
    public SpriteRenderer spriteRenderer; // untuk flip arah
    private bool facingRight = true;

    private Transform player;
    private Rigidbody2D rb;
    private CapsuleCollider2D detectionCollider;
    private bool isChasing = false;
    private float loseTimer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        detectionCollider = GetComponent<CapsuleCollider2D>();
        detectionCollider.isTrigger = true;

        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        // kalau lupa assign SpriteRenderer di Inspector, cari otomatis
        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Update()
    {
        if (isChasing && player != null)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
        }

        HandleFlip(); // update arah visual setiap frame
    }

    private void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        Vector2 target = patrolPoints[currentPoint].position;
        Vector2 dir = (target - (Vector2)transform.position).normalized;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, obstacleAvoidanceDistance, LayerMask.GetMask("Obstacle"));
        if (hit.collider == null)
        {
            rb.velocity = dir * patrolSpeed;
        }
        else
        {
            Vector2 perpendicular = Vector2.Perpendicular(dir).normalized;
            rb.velocity = perpendicular * patrolSpeed;
        }

        if (Vector2.Distance(transform.position, target) < 0.3f)
            currentPoint = (currentPoint + 1) % patrolPoints.Length;
    }

    private void ChasePlayer()
    {
        if (player == null) return;

        Vector2 dir = (player.position - transform.position).normalized;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, obstacleAvoidanceDistance, LayerMask.GetMask("Obstacle"));
        if (hit.collider == null)
            rb.velocity = dir * chaseSpeed;
        else
        {
            Vector2 perpendicular = Vector2.Perpendicular(dir).normalized;
            rb.velocity = perpendicular * chaseSpeed;
        }

        if (loseTimer > 0)
        {
            loseTimer -= Time.deltaTime;
            if (loseTimer <= 0)
                isChasing = false;
        }
    }

    private void HandleFlip()
    {
        // jika bergerak ke kanan, pastikan menghadap kanan, dan sebaliknya
        if (rb.velocity.x > 0.1f && !facingRight)
        {
            facingRight = true;
            spriteRenderer.flipX = false;
        }
        else if (rb.velocity.x < -0.1f && facingRight)
        {
            facingRight = false;
            spriteRenderer.flipX = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isChasing = true;
            loseTimer = 0f;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            loseTimer = loseSightDelay;
        }
    }
}
