using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public float attackDamage = 10f;
    public float attackSpeed = 2f;
    public float attackRange = 2f;

    [SerializeField] public Animator animator;
    [SerializeField] public Transform attackPoint;
    [SerializeField] public LayerMask enemyLayer;

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetButtonDown("Fire1")) Attack();
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    private void Attack()
    {
        // Trigger animation
        animator.SetTrigger("attack");

        // Get enemies to hit
        var hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);

        // Damage them
        foreach (var enemy in hitEnemies) enemy.GetComponent<Entity>().TakeDamage(attackDamage);
    }
}