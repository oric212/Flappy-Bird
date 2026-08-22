using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;

    public int CurrentScore { get; private set; }

    public bool TryAddPoint()
    {
        if (!gameManager.IsPlaying)
        {
            return false;
        }

        CurrentScore++;
        return true;
    }
}
