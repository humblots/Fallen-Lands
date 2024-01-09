using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private Animator animator;
    [SerializeField] private TrailRenderer tr;
    private readonly float dashingCooldown = 2f;
    private readonly float dashingPower = 24f;
    private readonly float dashingTime = 0.2f;


    private readonly float jumpingPower = 16f;
    private readonly float slidingCooldown = 1f;
    private readonly float slidingPower = 8f;
    private readonly float slidingTime = 0.5f;
    private readonly float speed = 10f;
    private readonly float wallJumpingDuration = 0.4f;
    private readonly Vector2 wallJumpingPower = new(4f, 8f);
    private readonly float wallJumpingTime = 0.2f;
    private readonly float wallSlidingSpeed = 2f;

    private bool canDash = true;
    private bool canSlide = true;
    private float horizontal;
    private bool isDashing;
    private bool isFacingRight = true;
    private bool isSliding;
    private bool isWallJumping;
    private bool isWallSliding;
    private float wallJumpingCounter;
    private float wallJumpingDirection;

    private void Update()
    {
        if (isDashing) return;

        horizontal = Input.GetAxisRaw("Horizontal");

        var isGrounded = IsGrounded();
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
            animator.SetBool("isJumping", true);
        }
        else
        {
            animator.SetBool("isJumping", false);
        }

        if (Input.GetButtonDown("Jump") && rb.velocity.y > 0f)
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);


        var isMoving = rb.velocity.x != 0f;
        // Gotta use a double click to dash
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash) StartCoroutine(Dash());
        if (Input.GetKeyDown(KeyCode.LeftControl) && isGrounded && isMoving && canSlide) StartCoroutine(Slide());

        WallSlide();
        WallJump();
        if (!isWallJumping) Flip();
    }

    private void FixedUpdate()
    {
        if (isDashing || isSliding) return;

        if (!isWallJumping)
        {
            rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
            animator.SetBool("isRunning", horizontal != 0f);
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
            animator.SetBool("isWallSliding", true);
        }
        else
        {
            isWallSliding = false;
            animator.SetBool("isWallSliding", false);
        }
    }

    private void WallJump()
    {
        if (isWallSliding)
        {
            isWallJumping = false;
            animator.SetBool("isJumping", false);
            wallJumpingDirection = -transform.localScale.x;
            wallJumpingCounter = wallJumpingTime;
            CancelInvoke(nameof(StopWalljumping));
        }
        else
        {
            wallJumpingCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump") && wallJumpingCounter > 0f)
        {
            isWallJumping = true;
            animator.SetBool("isJumping", true);
            rb.velocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);
            wallJumpingCounter = 0f;

            if (transform.localScale.x != wallJumpingDirection)
            {
                isFacingRight = !isFacingRight;
                var localScale = transform.localScale;
                localScale.x *= -1f;
                transform.localScale = localScale;
            }

            Invoke(nameof(StopWalljumping), wallJumpingDuration);
        }
    }

    private void StopWalljumping()
    {
        isWallJumping = false;
    }

    private void Flip()
    {
        if ((isFacingRight && horizontal < 0f) || (!isFacingRight && horizontal > 0f))
        {
            isFacingRight = !isFacingRight;
            var localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    private IEnumerator Slide()
    {
        canSlide = false;
        isSliding = true;
        rb.velocity = new Vector2(transform.localScale.x * slidingPower, 0f);
        animator.SetBool("isSliding", true);
        yield return new WaitForSeconds(slidingTime);
        animator.SetBool("isSliding", false);
        isSliding = false;
        yield return new WaitForSeconds(slidingCooldown);
        canSlide = true;
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        var originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.velocity = new Vector2(transform.localScale.x * dashingPower, 0f);
        animator.SetBool("isDashing", true);
        tr.emitting = true;
        yield return new WaitForSeconds(dashingTime);
        animator.SetBool("isDashing", false);
        tr.emitting = false;
        rb.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }
}