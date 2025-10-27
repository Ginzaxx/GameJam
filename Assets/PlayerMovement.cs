using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float flipThreshold = 0.1f; // ambang untuk memicu flip

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Animator animationController;
    private SpriteRenderer spriteRenderer;
    private bool isFacingRight = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animationController = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Pastikan state awal sesuai spriteRenderer.flipX
        isFacingRight = !spriteRenderer.flipX;
    }

    void Update()
    {
        // Jika kamu ingin gerakan top-down: gunakan kedua sumbu (X dan Y)
        // Jika ingin side-scroller: gunakan hanya X dan pertahankan rb.velocity.y
        rb.velocity = moveInput * moveSpeed;

        // Set parameter animator (kamu pakai parameter "Idle" sebelumnya)
        // Biasanya nama parameter lebih cocok "Speed" atau "Move", tapi saya pakai "Idle" sesuai script awal
        animationController.SetFloat("Idle", rb.velocity.sqrMagnitude);

        // Cek dan lakukan flip berdasarkan velocity.x
        HandleFlip();
    }

    private void HandleFlip()
    {
        float vx = rb.velocity.x;

        // Hanya flip jika melewati ambang kecil sehingga tidak flicker ketika speed ~ 0
        if (vx > flipThreshold && !isFacingRight)
        {
            spriteRenderer.flipX = false; // pastikan sprite menghadap kanan
            isFacingRight = true;
        }
        else if (vx < -flipThreshold && isFacingRight)
        {
            spriteRenderer.flipX = true; // balik sprite sehingga menghadap kiri
            isFacingRight = false;
        }
    }

    // Input System callback
    public void Move(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        SoundManager.PlaySound("Walk");
    }
}
