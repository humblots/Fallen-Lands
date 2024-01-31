using System;
using System.Collections.Generic;
using UnityEngine;

public class EntitiesManager : MonoBehaviour
{
    public GameObject PlayerObject { get; private set; }
    public List<GameObject> AvailableEnemies { get; private set; }
    
    public Transform Container { get; private set; }
    private GameObject[] _currentWave;
    
    public Player Player { get; private set; }
        
    void Start()
    {
    }

    void Update()
    {
    }
    
    public void StartGame()
    {
        // To be implemented
        PlayerObject = Instantiate(Resources.Load<GameObject>("Prefabs/Player"));
        Player = PlayerObject.GetComponent<Player>();
    }
    
    public void StopGame()
    {
        // To be implemented
    }
    
    // Spawn 5 random enemies from the list Enemies
    public void SpawnEnemiesWave()
    {
        _currentWave = new GameObject[5];
        for (int i = 0; i < 5; i++)
        {
            _currentWave[i] = Instantiate(
                AvailableEnemies[UnityEngine.Random.Range(0, AvailableEnemies.Count)],
                new Vector3(0, 0, 0), 
                Quaternion.identity, 
                Container
            );
        }
    }
    
    // On enemy death, remove it from the current wave
    public void RemoveEnemyFromWave(GameObject enemy)
    {
        for (int i = 0; i < _currentWave.Length; i++)
        {
            if (_currentWave[i] == enemy)
            {
                _currentWave[i] = null;
                break;
            }
        }
    }
}
