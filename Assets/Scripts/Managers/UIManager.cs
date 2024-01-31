using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private GameManager _gameManager;
    public TextMeshProUGUI score;
    public TextMeshProUGUI bestScore;
    public TextMeshProUGUI life;
    public GameObject title;
    public GameObject startButton;
    
    private Entity _player;

    private void Awake()
    {
        _gameManager = GameManager.Instance;
        _player = _gameManager.EntitiesManager.Player.GetComponent<Entity>();
    }

    void Update()
    {
        score.text = $"Waves beaten: {_gameManager.ScoreManager.Score}";
        bestScore.text = $"Best Score: {_gameManager.ScoreManager.BestScore}";
        life.text = $"Life: {_player.currentHealth}";
    }

    public void StartGame()
    {
        startButton.SetActive(false);
        title.SetActive(false);
        
    }

    public void StopGame()
    {
        startButton.SetActive(true);
    }
}