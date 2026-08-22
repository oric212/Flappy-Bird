using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private const string MusicVolumeKey = "MusicVolume";
    private const string SfxVolumeKey = "SfxVolume";

    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource effectsSource;
    [SerializeField] private AudioClip flapClip;
    [SerializeField] private AudioClip scoreClip;
    [SerializeField] private AudioClip powerUpClip;
    [SerializeField] private AudioClip deathClip;

    public float MusicVolume => musicSource != null ? musicSource.volume : 0f;
    public float SfxVolume => effectsSource != null ? effectsSource.volume : 0f;

    private void Awake()
    {
        SetSourceVolume(musicSource, PlayerPrefs.GetFloat(MusicVolumeKey, 0.25f));
        SetSourceVolume(effectsSource, PlayerPrefs.GetFloat(SfxVolumeKey, 0.7f));
    }

    private void Start()
    {
        if (musicSource != null && musicSource.clip != null && !musicSource.isPlaying)
        {
            musicSource.Play();
        }
    }

    public void PlayFlap() => PlayEffect(flapClip);
    public void PlayScore() => PlayEffect(scoreClip);
    public void PlayPowerUp() => PlayEffect(powerUpClip);
    public void PlayDeath() => PlayEffect(deathClip);

    public void SetMusicVolume(float volume)
    {
        volume = Mathf.Clamp01(volume);
        SetSourceVolume(musicSource, volume);
        PlayerPrefs.SetFloat(MusicVolumeKey, volume);
        PlayerPrefs.Save();
    }

    public void SetSfxVolume(float volume)
    {
        volume = Mathf.Clamp01(volume);
        SetSourceVolume(effectsSource, volume);
        PlayerPrefs.SetFloat(SfxVolumeKey, volume);
        PlayerPrefs.Save();
    }

    private void PlayEffect(AudioClip clip)
    {
        if (effectsSource != null && clip != null)
        {
            effectsSource.PlayOneShot(clip);
        }
    }

    private static void SetSourceVolume(AudioSource source, float volume)
    {
        if (source != null)
        {
            source.volume = Mathf.Clamp01(volume);
        }
    }
}
