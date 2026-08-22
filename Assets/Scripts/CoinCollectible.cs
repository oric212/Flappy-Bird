using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CoinCollectible : MonoBehaviour
{
    private ScoreManager scoreManager;
    private AudioManager audioManager;
    private bool collected;

    public void Initialize(ScoreManager score, AudioManager audio)
    {
        scoreManager = score;
        audioManager = audio;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collected || other.GetComponent<BirdController>() == null)
        {
            return;
        }

        if (!scoreManager.TryAddPoint())
        {
            return;
        }

        collected = true;
        audioManager?.PlayScore();
        Destroy(gameObject);
    }
}
