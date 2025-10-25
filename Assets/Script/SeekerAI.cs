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
    [Tooltip("Sprite tubuh karakter (tidak ikut rotate, hanya ganti arah).")]
    public SpriteRenderer bodyRenderer;

    [Tooltip("Sprite cahaya yang akan ikut rotate arah pandang).")]
    public Transform lightVisual;

    [Tooltip("Transform collider (biasanya di root).")]
    public Transform colliderVisual;

    [Header("Body Direction Sprites")]
    public Sprite spriteUp;
    public Sprite spriteDown;
    public Sprite spriteLeft;
    public Sprite spriteRight;

    public float rotationLerpSpeed = 10f; // untuk rotasi cahaya dan collider

    private Transform player;
    private Rigidbody2D rb;
    private CapsuleCollider2D detectionCollider;
    private bool isChasing = false;
    private float loseTimer;
    private Vector2 lastDir = Vector2.right;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        detectionCollider = GetComponent<CapsuleCollider2D>();
        detectionCollider.isTrigger = true;

        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (bodyRenderer == null)
            bodyRenderer = GetComponentInChildren<SpriteRenderer>();

        if (lightVisual == null)
            Debug.LogWarning("⚠️ Assign 'lightVisual' (sprite cahaya) di Inspector!");
        if (colliderVisual == null)
            colliderVisual = detectionCollider.transform;
    }

    private void Update()
    {
        if (isChasing && player != null)
            ChasePlayer();
        else
            Patrol();

        UpdateBodySprite();
        RotateLightAndCollider();
    }

    private void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        Vector2 target = patrolPoints[currentPoint].position;
        Vector2 dir = (target - (Vector2)transform.position).normalized;
        lastDir = dir;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, obstacleAvoidanceDistance, LayerMask.GetMask("Obstacle"));
        if (hit.collider == null)
            rb.velocity = dir * patrolSpeed;
        else
            rb.velocity = Vector2.Perpendicular(dir) * patrolSpeed;

        if (Vector2.Distance(transform.position, target) < 0.3f)
            currentPoint = (currentPoint + 1) % patrolPoints.Length;
    }

    private void ChasePlayer()
    {
        if (player == null) return;

        Vector2 dir = (player.position - transform.position).normalized;
        lastDir = dir;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, obstacleAvoidanceDistance, LayerMask.GetMask("Obstacle"));
        if (hit.collider == null)
            rb.velocity = dir * chaseSpeed;
        else
            rb.velocity = Vector2.Perpendicular(dir) * chaseSpeed;

        if (loseTimer > 0)
        {
            loseTimer -= Time.deltaTime;
            if (loseTimer <= 0)
                isChasing = false;
        }
    }

    private void UpdateBodySprite()
    {
        Vector2 v = rb.velocity;

        // Tentukan arah dominan
        if (Mathf.Abs(v.x) > Mathf.Abs(v.y))
        {
            if (v.x > 0.1f) bodyRenderer.sprite = spriteRight;
            else if (v.x < -0.1f) bodyRenderer.sprite = spriteLeft;
        }
        else
        {
            if (v.y > 0.1f) bodyRenderer.sprite = spriteUp;
            else if (v.y < -0.1f) bodyRenderer.sprite = spriteDown;
        }
    }

    private void RotateLightAndCollider()
    {
        if (lastDir == Vector2.zero) return;

        // Arah dominan
        float angle = 0f;
        if (Mathf.Abs(lastDir.x) > Mathf.Abs(lastDir.y))
        {
            // Kiri / kanan
            angle = (lastDir.x > 0) ? 0f : 180f;
        }
        else
        {
            // Atas / bawah
            angle = (lastDir.y > 0) ? 90f : -90f;
        }

        Quaternion targetRot = Quaternion.Euler(0, 0, angle);

        // Rotasi cahaya (sprite)
        if (lightVisual != null)
            lightVisual.rotation = Quaternion.Lerp(lightVisual.rotation, targetRot, Time.deltaTime * rotationLerpSpeed);

        // Rotasi collider (CapsuleCollider2D horizontal)
        if (colliderVisual != null)
            colliderVisual.rotation = Quaternion.Lerp(colliderVisual.rotation, targetRot, Time.deltaTime * rotationLerpSpeed);
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
