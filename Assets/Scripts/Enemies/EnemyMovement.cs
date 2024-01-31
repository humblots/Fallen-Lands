using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float runMaxSpeed = 15f;
    public float runAcceleration = 3f;
    public float runDecceleration = 6f;
    
    public bool IsFacingRight { get; private set; }
    
    private Rigidbody2D _rb;
    private Animator _animator;
    
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        IsFacingRight = true;
    }
    
    // Move to the player position
    public void MoveToPlayer(Transform player)
    {
        var direction = GetPlayerDirection(player);
        if (direction > 0 && !IsFacingRight || direction < 0 && IsFacingRight) Turn();
        
        float targetSpeed = direction * runMaxSpeed;

        float accelRate;
        accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? runAcceleration : runDecceleration;
        
        float speedDif = targetSpeed - _rb.velocity.x;
        float movement = speedDif * accelRate;

        _rb.AddForce(movement * Vector2.right, ForceMode2D.Force);
        _animator.SetFloat("xVelocity", Mathf.Abs(_rb.velocity.x));
    }
    
    // Get the direction of the player
    public float GetPlayerDirection(Transform player)
    {
        return player.position.x - transform.position.x;
    }
    
    public void Turn ()
    {
        Vector3 scale = transform.localScale; 
        scale.x *= -1;
        transform.localScale = scale;
        IsFacingRight = !IsFacingRight;
    }
}
