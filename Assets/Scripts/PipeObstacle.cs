using UnityEngine;

public class PipeObstacle : MonoBehaviour
{
    public bool HasScored { get; private set; }
    public string ObstacleType { get; private set; }

    private Transform bird;
    private ScoreManager scoreManager;
    private AudioManager audioManager;

    public void Initialize(Transform birdTransform, ScoreManager manager,
        AudioManager audio, string typeName)
    {
        bird = birdTransform;
        scoreManager = manager;
        audioManager = audio;
        ObstacleType = typeName;
        HasScored = false;
    }

    private void Update()
    {
        if (!HasScored && bird != null && bird.position.x > transform.position.x)
        {
            if (scoreManager.TryAddPoint())
            {
                audioManager?.PlayScore();
                HasScored = true;
            }
        }
    }
}
