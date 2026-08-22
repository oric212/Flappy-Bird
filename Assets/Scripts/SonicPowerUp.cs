using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class SonicPowerUp : MonoBehaviour
{
    [SerializeField] private AudioManager audioManager;

    private bool hasBeenCollected;

    public void Initialize(AudioManager audio)
    {
        audioManager = audio;
        hasBeenCollected = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasBeenCollected)
        {
            return;
        }

        BirdController bird = other.GetComponent<BirdController>();
        if (bird == null)
        {
            return;
        }

        if (!bird.ApplySpeedBoost())
        {
            return;
        }

        hasBeenCollected = true;
        audioManager?.PlayPowerUp();
        Destroy(gameObject);
    }
}
