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
        Debug.Log(name + " died");
        animator.SetBool("isDead", true);
        GetComponent<Collider2D>().enabled = false;
        enabled = false;
    }
}