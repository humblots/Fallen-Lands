using UnityEngine;

public class Entity : MonoBehaviour
{
    public float maxHealth = 200f;
    
    private Animator _animator;
    private Rigidbody2D _rb;
    private Collider2D _collider;

    public float currentHealth { get; private set; }

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        _animator.SetTrigger("hurt");
        if (currentHealth <= 0) Die();
    }

    public void Die()
    {
        _animator.SetBool("isDead", true);
        _rb.bodyType = RigidbodyType2D.Static;
        _collider.enabled = false;
        enabled = false;
    }
    
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}