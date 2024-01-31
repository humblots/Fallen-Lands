using UnityEngine;

public class Entity : MonoBehaviour
{
    public float maxHealth = 200f;
    
    private Animator Animator;
    private Rigidbody2D RB;
    private Collider2D Collider;
    
    private float currentHealth;

    private void Start()
    {
        Animator = GetComponent<Animator>();
        RB = GetComponent<Rigidbody2D>();
        Collider = GetComponent<Collider2D>();
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Animator.SetTrigger("hurt");
        if (currentHealth <= 0) Die();
    }

    public void Die()
    {
        Animator.SetBool("isDead", true);
        RB.bodyType = RigidbodyType2D.Static;
        Collider.enabled = false;
        enabled = false;
    }
    
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}