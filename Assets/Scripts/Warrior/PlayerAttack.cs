using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public float attackDamage = 10f;
    public float attackSpeed = 2f;
    public float attackRange = 2f;

    [SerializeField] public Animator animator;
    [SerializeField] public Transform attackPoint;
    [SerializeField] public LayerMask enemyLayer;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) Attack();
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
        foreach (var enemy in hitEnemies) enemy.GetComponent<Entity>().TakeDamage(attackDamage);
    }
}