using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public int Score { get; private set; }
    public int BestScore { get; private set; }

    public void Awake()
    {
        BestScore = PlayerPrefs.GetInt("best-score", 0);
    }

    public void Reset()
    {
        Score = 0;
    }

    public void IncrementScore()
    {
        Score += 1;
        if (Score > BestScore)
        {
            BestScore = Score;
            PlayerPrefs.SetInt("best-score", BestScore);
        }
    }
}