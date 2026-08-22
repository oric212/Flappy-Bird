using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private GameObject gameOverContainer;
    [SerializeField] private TMP_Text finalScoreText;
    [SerializeField] private GameObject mainMenuContainer;
    [SerializeField] private TMP_Text bestScoreText;
    [SerializeField] private TMP_Text menuBestScoreText;
    [SerializeField] private Button playButton;
    [SerializeField] private Toggle arcadeFeaturesToggle;
    [SerializeField] private BirdController birdController;
    [SerializeField] private TMP_Text speedBoostText;
    [SerializeField] private Image speedBoostProgressFill;
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private TMP_Text arcadeStateText;
    [SerializeField] private Image arcadeStateBackground;
    [SerializeField] private TMP_Text menuHintText;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;

    private bool eventsRegistered;
    [SerializeField] private float speedBoostProgressFullWidth = 216f;

    private void Start()
    {
        arcadeFeaturesToggle.SetIsOnWithoutNotify(gameManager.ArcadeFeaturesEnabled);
        playButton.onClick.AddListener(gameManager.StartRun);
        arcadeFeaturesToggle.onValueChanged.AddListener(
            gameManager.SetArcadeFeaturesEnabled);
        musicVolumeSlider.SetValueWithoutNotify(audioManager.MusicVolume);
        sfxVolumeSlider.SetValueWithoutNotify(audioManager.SfxVolume);
        musicVolumeSlider.onValueChanged.AddListener(audioManager.SetMusicVolume);
        sfxVolumeSlider.onValueChanged.AddListener(audioManager.SetSfxVolume);
        restartButton.onClick.AddListener(gameManager.RestartGame);
        mainMenuButton.onClick.AddListener(gameManager.ReturnToMainMenu);
        eventsRegistered = true;
        RefreshDisplay();
    }

    private void OnDestroy()
    {
        if (!eventsRegistered)
        {
            return;
        }

        playButton.onClick.RemoveListener(gameManager.StartRun);
        arcadeFeaturesToggle.onValueChanged.RemoveListener(
            gameManager.SetArcadeFeaturesEnabled);
        musicVolumeSlider.onValueChanged.RemoveListener(audioManager.SetMusicVolume);
        sfxVolumeSlider.onValueChanged.RemoveListener(audioManager.SetSfxVolume);
        restartButton.onClick.RemoveListener(gameManager.RestartGame);
        mainMenuButton.onClick.RemoveListener(gameManager.ReturnToMainMenu);
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
        mainMenuContainer.SetActive(gameManager.IsWaitingToStart);
        scoreText.gameObject.SetActive(gameManager.IsPlaying);
        bestScoreText.text = "BEST: " + gameManager.HighScore;
        menuBestScoreText.text = "BEST: " + gameManager.HighScore;
        bool arcadeEnabled = gameManager.ArcadeFeaturesEnabled;
        arcadeStateText.text = arcadeEnabled ? "ON" : "OFF";
        arcadeStateBackground.color = arcadeEnabled
            ? new Color(0.96f, 0.65f, 0.18f, 1f)
            : new Color(0.25f, 0.29f, 0.31f, 1f);
        menuHintText.text = arcadeEnabled
            ? "SPACE / CLICK  •  FLAP\nCOINS +1  •  SONIC SPEED BOOST"
            : "SPACE / CLICK  •  FLAP";
        bool showBoost = gameManager.IsPlaying && birdController.IsSpeedBoosted;
        speedBoostText.gameObject.SetActive(showBoost);
        Vector2 fillSize = speedBoostProgressFill.rectTransform.sizeDelta;
        fillSize.x = speedBoostProgressFullWidth * birdController.SpeedBoostProgress;
        speedBoostProgressFill.rectTransform.sizeDelta = fillSize;
        if (showBoost)
        {
            speedBoostText.text = "SONIC BOOST";
        }
    }
}
