using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;

    public int CurrentScore { get; private set; }

    public void AddPoint()
    {
        if (gameManager.IsGameOver)
        {
            return;
        }

        CurrentScore++;
    }
}
