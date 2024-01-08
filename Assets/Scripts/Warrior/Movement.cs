using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{

    public float movementSpeed;
    public float jumpForce;
    public bool isGrounded;
    public bool isJumping;
    
    public Rigidbody2D body;
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    
    public Transform groundCheckLeft;
    public Transform groundCheckRight;
    
    private Vector3 velocity = Vector3.zero;

    void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapArea(groundCheckLeft.position, groundCheckRight.position);
        float horizontalMovement = Input.GetAxis("Horizontal") * movementSpeed * Time.deltaTime;

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            isJumping = true;
        }

        MoveWarrior(horizontalMovement);
        
        Flip(body.velocity.x);
        animator.SetFloat("Speed", Math.Abs(body.velocity.x));
    }

    void MoveWarrior(float _horizontalMovement)
    {
        Vector3 targetVelocity = new Vector2(_horizontalMovement, body.velocity.y);
        body.velocity = Vector3.SmoothDamp(body.velocity, targetVelocity, ref velocity, .05f);

        if (isJumping)
        {
            body.AddForce(new Vector2(0f, jumpForce));
            isJumping = false; 
        } 
    }

    void Flip(float _velocity)
    {
        if (_velocity > 0.1f)
        {
            spriteRenderer.flipX = false;
        } else if (_velocity < -0.1f)
        {
            spriteRenderer.flipX = true;
        }
    }
}
