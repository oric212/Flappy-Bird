using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private float restartInputDelay = 0.35f;

    public bool IsGameOver { get; private set; }
    public int GameOverCount { get; private set; }

    private float gameOverTime;

    private void Update()
    {
        if (!IsGameOver || Time.unscaledTime - gameOverTime < restartInputDelay)
        {
            return;
        }

        bool spacePressed = Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame;
        bool mousePressed = Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;

        if (spacePressed || mousePressed)
        {
            RestartGame();
        }
    }

    public void RestartGame()
    {
        if (!IsGameOver)
        {
            return;
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().path);
    }

    public void TriggerGameOver()
    {
        if (IsGameOver)
        {
            return;
        }

        IsGameOver = true;
        GameOverCount++;
        gameOverTime = Time.unscaledTime;
    }
}
