using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float speed = 15f;
    
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

        _rb.velocity = new Vector2(direction * speed, _rb.velocity.y);
        _animator.SetFloat("xVelocity", Mathf.Abs(_rb.velocity.x));
    }
    
    // Get the direction of the player
    public float GetPlayerDirection(Transform player)
    {
        var direction = player.position.x - transform.position.x;
        return direction < 0 ? -1 : 1;
    }
    
    public void Turn ()
    {
        Vector3 scale = transform.localScale; 
        scale.x *= -1;
        transform.localScale = scale;
        IsFacingRight = !IsFacingRight;
    }
}
