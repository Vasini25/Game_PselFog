using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movimento : MonoBehaviour
{
    private float horizontal;
    private float horizontalFaster;
    private float speed = 4f;
    private float jumpingPower = 16f;
    private float repellingPower = 12f;
    bool isFacingRight = true;
    public bool isAlive = true;
    private bool jumped = false;

    private bool isWallSliding;
    private float wallSlidingSpeed = 2f;
    private bool isWallJumping;
    private float wallJumpingDirection;
    private float wallJumpingTime = 0.2f;
    private float wallJumpingCounter;
    private float wallJumpingDuration = 0.4f;
    private Vector2 wallJumpingPower = new Vector2(8f, 16f);

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask wallLayer;
    private SpriteRenderer sr;
    private BoxCollider2D bc;

    Animator animator;
    
    AudioSource jumpSound;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        jumpSound = GetComponent<AudioSource>();
        bc = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        horizontalFaster = Input.GetAxis("fasterRun");

        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            jumped = true;
            jumpSound.Play();
        }

        animator.SetBool("jump", !IsGrounded());

        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }

        animator.SetBool("walk", Mathf.Abs(rb.velocity.x) > 0.5f);
        animator.SetBool("run", Mathf.Abs(rb.velocity.x) > Mathf.Abs(horizontal * speed));

        WallSlide();
        WallJump();

        if(!isWallJumping && isAlive)
        {
            Flip();
        }

        animator.SetBool("wall", isWallSliding);
    }

    private void FixedUpdate()
    {
        if(!isWallJumping && isAlive)
        {
            rb.velocity = new Vector2(horizontal * speed * ((horizontalFaster*1.25f) + 1), rb.velocity.y);
        }

        if(jumped)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
            jumped = false;
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private bool IsWalled()
    {
        return Physics2D.OverlapCircle(wallCheck.position, 0.2f, wallLayer);
    }

    private void WallSlide()
    {
        if (IsWalled() && !IsGrounded() && horizontal != 0f)
        {
            isWallSliding = true;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue));
        }
        else
        {
            isWallSliding = false;
        }
    }

    private void WallJump()
    {
        if (isWallSliding)
        {
            isWallJumping = false;
            wallJumpingDirection = -transform.localScale.x;
            wallJumpingCounter = wallJumpingTime;

            CancelInvoke(nameof(StopWallJumping));
        }
        else
        {
            wallJumpingCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump") && wallJumpingCounter > 0f)
        {
            jumpSound.Play();
            isWallJumping = true;
            rb.velocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);
            wallJumpingCounter = 0f;

            if (transform.localScale.x != wallJumpingDirection)
            {
                isFacingRight = !isFacingRight;
                Vector3 localScale = transform.localScale;
                localScale.x *= -1f;
                transform.localScale = localScale;
            }

            Invoke(nameof(StopWallJumping), wallJumpingDuration);
        }
    }

    private void StopWallJumping()
    {
        isWallJumping = false;
    }


    private void Flip()
    {
        if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            var enemy = other.gameObject.GetComponent<EnemyController>();
            rb.velocity = new Vector2(rb.velocity.x, repellingPower);
            enemy.Kill();
        }

        else if (other.gameObject.CompareTag("Trampoline"))
        {
            rb.velocity = new Vector2(rb.velocity.x, repellingPower * 2f);
            jumpSound.Play();
        }
    }

    void OnCollisionStay2D(Collision2D col) {
 
        if (col.gameObject.tag == "Enemy")
        {
            var enemy = col.gameObject.GetComponent<EnemyController>();

            if(enemy.isAlive >= 1f && enemy.attack)
            {
                Die(enemy);
            }
        }
    }

    public void Die(EnemyController enemy)
    {
        if(isAlive)
        {
            enemy.deathSound.Play();
        }

        this.isAlive = false;
        animator.SetTrigger("die");
        this.enabled = false;
    }
}