using UnityEngine;

public class Entity : MonoBehaviour
{
    public float maxHealth = 200f;

    [SerializeField] public Animator animator;
    private float currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        animator.SetTrigger("hurt");
        if (currentHealth <= 0) Die();
    }

    public void Die()
    {
        animator.SetBool("isDead", true);
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        GetComponent<Collider2D>().enabled = false;
        enabled = false;
    }
    
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}