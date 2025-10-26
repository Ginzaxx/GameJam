using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
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
    private float loseTimer;

    [Header("Visual Settings")]
    public SpriteRenderer spriteRenderer;
    private bool facingRight = true;

    [Header("Vision Settings")]
    public Light2D visionLight;          // Light milik seeker
    public Transform visionCollider;     // Collider trigger penglihatan (child)
    public float rotationSpeed = 10f;    // Seberapa cepat rotasi mengikuti arah

    private Rigidbody2D rb;
    private Transform player;
    private bool isChasing = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Update()
    {
        if (isChasing && player != null)
            ChasePlayer();
        else
            Patrol();

        HandleFlip();
        RotateVisionToDirection();
    }

    private void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        Vector2 target = patrolPoints[currentPoint].position;
        Vector2 dir = (target - (Vector2)transform.position).normalized;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, obstacleAvoidanceDistance, LayerMask.GetMask("Obstacle"));
        if (hit.collider == null)
            rb.velocity = dir * patrolSpeed;
        else
            rb.velocity = Vector2.Perpendicular(dir).normalized * patrolSpeed;

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
            rb.velocity = Vector2.Perpendicular(dir).normalized * chaseSpeed;

        if (loseTimer > 0)
        {
            loseTimer -= Time.deltaTime;
            if (loseTimer <= 0)
                isChasing = false;
        }
    }

    private void HandleFlip()
    {
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

    private void RotateVisionToDirection()
    {
        if (rb.velocity.sqrMagnitude < 0.01f) return;

        Vector2 moveDir = rb.velocity.normalized;
        float angle = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg;

        Quaternion targetRot = Quaternion.Euler(0, 0, angle + 0f);

        if (visionLight != null)
            visionLight.transform.rotation = Quaternion.Lerp(visionLight.transform.rotation, targetRot, Time.deltaTime * rotationSpeed);

        if (visionCollider != null)
            visionCollider.rotation = Quaternion.Lerp(visionCollider.rotation, targetRot, Time.deltaTime * rotationSpeed);
    }

    // ================================
    // 👁️ Vision Collider (Trigger)
    // ================================
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Kalau player masuk ke area cahaya penglihatan
        if (other.CompareTag("Player"))
        {
            isChasing = true;
            loseTimer = 0f;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            loseTimer = loseSightDelay;
    }

    // ================================
    // 🧍 Body Collider (Non-trigger)
    // ================================
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            TriggerGameOver();
        }
    }

    private void TriggerGameOver()
    {
        Debug.Log("💀 GAME OVER: Player tertangkap oleh Seeker!");
        rb.velocity = Vector2.zero;
        Time.timeScale = 0f;
        GameOverUI.Show();
    }
}
