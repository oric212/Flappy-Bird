using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static bool startRunAfterReload;

    public enum GameState
    {
        MainMenu,
        Playing,
        GameOver
    }

    private const string HighScoreKey = "FlappyBirdHighScore";
    private const string ArcadeFeaturesKey = "ArcadeFeaturesEnabled";

    [SerializeField] private float restartInputDelay = 0.35f;
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private AudioManager audioManager;

    public GameState CurrentState { get; private set; } = GameState.MainMenu;
    public bool IsWaitingToStart => CurrentState == GameState.MainMenu;
    public bool IsPlaying => CurrentState == GameState.Playing;
    public bool IsGameOver => CurrentState == GameState.GameOver;
    public bool ArcadeFeaturesEnabled { get; private set; }
    public int GameOverCount { get; private set; }
    public int HighScore { get; private set; }

    private float gameOverTime;
    private int gameStartedFrame = -1;

    public bool CanAcceptGameplayInput => IsPlaying
        && Time.frameCount > gameStartedFrame;

    private void Awake()
    {
        HighScore = PlayerPrefs.GetInt(HighScoreKey, 0);
        ArcadeFeaturesEnabled = PlayerPrefs.GetInt(ArcadeFeaturesKey, 1) == 1;

        if (startRunAfterReload)
        {
            startRunAfterReload = false;
            StartRun();
        }
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
#if !UNITY_EDITOR
            Application.Quit();
#endif
        }

        if (CurrentState == GameState.MainMenu
            && Keyboard.current != null
            && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            StartRun();
            return;
        }

        if (CurrentState != GameState.GameOver
            || Time.unscaledTime - gameOverTime < restartInputDelay)
        {
            return;
        }

        bool spacePressed = Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame;
        bool mousePressed = Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
        bool pointerOverUi = EventSystem.current != null
            && EventSystem.current.IsPointerOverGameObject();

        if (spacePressed || (mousePressed && !pointerOverUi))
        {
            RestartGame();
        }
    }

    public void StartRun()
    {
        if (CurrentState == GameState.MainMenu)
        {
            CurrentState = GameState.Playing;
            gameStartedFrame = Time.frameCount;
        }
    }

    public void SetArcadeFeaturesEnabled(bool enabled)
    {
        ArcadeFeaturesEnabled = enabled;
        PlayerPrefs.SetInt(ArcadeFeaturesKey, enabled ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void RestartGame()
    {
        if (!IsGameOver)
        {
            return;
        }

        startRunAfterReload = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().path);
    }

    public void ReturnToMainMenu()
    {
        if (!IsGameOver)
        {
            return;
        }

        startRunAfterReload = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().path);
    }

    public void TriggerGameOver()
    {
        if (IsGameOver)
        {
            return;
        }

        CurrentState = GameState.GameOver;
        GameOverCount++;
        gameOverTime = Time.unscaledTime;
        audioManager?.PlayDeath();

        int finalScore = scoreManager != null ? scoreManager.CurrentScore : 0;
        if (finalScore > HighScore)
        {
            HighScore = finalScore;
            PlayerPrefs.SetInt(HighScoreKey, HighScore);
            PlayerPrefs.Save();
        }
    }
}
