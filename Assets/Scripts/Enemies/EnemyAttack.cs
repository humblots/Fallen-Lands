using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public float attackDamage = 10f;
    public float attackRange = 2f;
    public float initialAttackDelay = 2f;
    public float knockbackForce = 1.5f;
    
    private float _attackDelay;
    
    [SerializeField] public Transform attackPoint;
    [SerializeField] public LayerMask playerLayer;
    
    private Animator _animator;
    
    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    public void Attack(GameObject player, float knockbackDir)
    {
        if (_attackDelay <= 0)
        {       
            _animator.SetTrigger("attack");
            player.GetComponent<Rigidbody2D>()
                .AddForce(new Vector2(knockbackDir * knockbackForce, 0f), ForceMode2D.Impulse);
            player.GetComponent<Entity>().TakeDamage(attackDamage);
            _attackDelay = initialAttackDelay;
        }
        else
        {
            _attackDelay -= Time.deltaTime;
        }
    }
    
    public bool IsPlayerInRange()
    {
        return Physics2D.OverlapCircle(attackPoint.position, attackRange, playerLayer);
    }
    
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
