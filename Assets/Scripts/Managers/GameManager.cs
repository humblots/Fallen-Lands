using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class GameManager : MonoBehaviour
{
    //Singleton pattern
    public static GameManager Instance { get; private set; }

    public ScoreManager ScoreManager { get; private set; }
    public EntitiesManager EntitiesManager { get; private set; }
    public UIManager UIManager { get; private set; }
    public AudioManager AudioManager { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        ScoreManager = GetComponent<ScoreManager>();
        EntitiesManager = GetComponent<EntitiesManager>();
        UIManager = GetComponent<UIManager>();
        AudioManager = GetComponent<AudioManager>();
    }

    private void Start()
    {
        EntitiesManager.Player.OnPlayerDeath += GameOverHandler;
    }

    private void GameOverHandler()
    {
        StopGame();
    }
    
    
    public void StartGame()
    {
        ScoreManager.Reset();
        EntitiesManager.StartGame();
        UIManager.StartGame();
        AudioManager.StartGame();
        
    }

    public void StopGame()
    {
        UIManager.StopGame();
        EntitiesManager.StopGame();
        AudioManager.StopGame();
    }
}