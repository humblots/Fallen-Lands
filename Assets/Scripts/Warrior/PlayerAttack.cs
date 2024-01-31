using System;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public float attackDamage = 10f;
    public float attackRange = 2f;
    public float initialAttackDelay = 2f;
    public float knockbackForce = 1.5f;
    
    private float attackDelay = 0f;
    
    private PlayerMovement playerMovement;
    
    [SerializeField] public Animator animator;
    [SerializeField] public Transform attackPoint;
    [SerializeField] public LayerMask enemyLayer;

    private void Start()
    { 
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        if (attackDelay <= 0)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Attack();
                attackDelay = initialAttackDelay;
            }
        }
        else
        {
            attackDelay -= Time.deltaTime;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    private void Attack()
    {
        animator.SetTrigger("attack");
        var hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);
        foreach (var enemy in hitEnemies)
        {
            var knockBackDir = playerMovement.IsFacingRight ? 1f : -1f;
            enemy.GetComponent<Rigidbody2D>()
                .AddForce(new Vector2(knockBackDir * knockbackForce, 0f), ForceMode2D.Impulse);
            enemy.GetComponent<Entity>().TakeDamage(attackDamage);
        }
    }
}