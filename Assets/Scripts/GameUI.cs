using TMPro;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private GameObject gameOverContainer;
    [SerializeField] private TMP_Text finalScoreText;

    private void Start()
    {
        RefreshDisplay();
    }

    private void Update()
    {
        RefreshDisplay();
    }

    private void RefreshDisplay()
    {
        int score = scoreManager.CurrentScore;
        scoreText.text = score.ToString();
        finalScoreText.text = "SCORE: " + score;
        gameOverContainer.SetActive(gameManager.IsGameOver);
    }
}
