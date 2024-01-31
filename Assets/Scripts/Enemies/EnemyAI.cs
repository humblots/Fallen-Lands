using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    
    private EnemyAttack _enemyAttack;
    private EnemyMovement _enemyMovement;
    
    private GameObject _player;
    
    void Start()
    {
        _enemyAttack = GetComponent<EnemyAttack>();
        _enemyMovement = GetComponent<EnemyMovement>();
        _player = GameObject.FindWithTag("Player");
    }

    void Update()
    {
        if (_enemyAttack.IsPlayerInRange())
        {
            _enemyAttack.Attack(_player, _enemyMovement.GetPlayerDirection(_player.transform));
        }
        else
        {
            _enemyMovement.MoveToPlayer(_player.transform);
        }
    }
}
