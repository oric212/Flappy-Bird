using UnityEngine;

public class PipeObstacle : MonoBehaviour
{
    public bool HasScored { get; private set; }
    public string ObstacleType { get; private set; }

    private Transform bird;
    private ScoreManager scoreManager;

    public void Initialize(Transform birdTransform, ScoreManager manager, string typeName)
    {
        bird = birdTransform;
        scoreManager = manager;
        ObstacleType = typeName;
        HasScored = false;
    }

    private void Update()
    {
        if (!HasScored && bird != null && bird.position.x > transform.position.x)
        {
            scoreManager.AddPoint();
            HasScored = true;
        }
    }
}
