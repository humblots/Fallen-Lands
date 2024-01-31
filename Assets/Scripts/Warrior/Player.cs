using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Action OnPlayerDeath;
    
    private Entity _entity;
    void Start()
    {
        _entity = GetComponent<Entity>();
    }

    void Update()
    {
        if (_entity.enabled == false && _entity.currentHealth <= 0)
        {
            OnPlayerDeath?.Invoke();
        }
    }
}
