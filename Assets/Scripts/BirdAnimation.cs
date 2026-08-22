using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class BirdAnimation : MonoBehaviour
{
    [SerializeField] private SpriteRenderer targetRenderer;
    [SerializeField] private Sprite[] flyingFrames;
    [SerializeField] private float framesPerSecond = 8f;
    [SerializeField] private GameManager gameManager;

    private float animationTime;

    private void Awake()
    {
        if (targetRenderer == null)
        {
            targetRenderer = GetComponent<SpriteRenderer>();
        }
    }

    private void Update()
    {
        if (gameManager == null || gameManager.IsGameOver
            || flyingFrames == null || flyingFrames.Length == 0)
        {
            return;
        }

        animationTime += Time.deltaTime;
        int frameIndex = Mathf.FloorToInt(animationTime * framesPerSecond)
            % flyingFrames.Length;
        targetRenderer.sprite = flyingFrames[frameIndex];
    }
}
