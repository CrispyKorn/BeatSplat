using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [Tooltip("Music that plays preceeding ball launch.")]
    [SerializeField] private Sound _preLaunch;
    [Tooltip("Music that plays post ball launch.")]
    [SerializeField] private Sound _postLaunch;

    private SoundPlayer _soundPlayer;
    private AudioSource _audioSource;

    private void Awake()
    {
        Locator.Instance.RegisterInstance(this);

        _soundPlayer = GetComponent<SoundPlayer>();
        _audioSource = GetComponent<AudioSource>();
    }

    private async void Start()
    {
        await Awaitable.NextFrameAsync();
        SetMusic(false);
    }

    private void SwitchMusic(Sound newMusic)
    {
        if (_soundPlayer.Sound == newMusic) return;

        _soundPlayer.StopSound();
        _soundPlayer.SwitchSound(newMusic);
        _audioSource.time = Time.time % _audioSource.clip.length;
    }

    public void SetMusic(bool postLaunch)
    {
        if (postLaunch) SwitchMusic(_postLaunch);
        else SwitchMusic(_preLaunch);
    }
}
