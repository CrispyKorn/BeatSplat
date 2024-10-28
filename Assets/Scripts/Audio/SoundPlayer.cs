using UnityEngine;

[RequireComponent (typeof(AudioSource))]
public class SoundPlayer : MonoBehaviour
{
    [SerializeField] private Sound _sound;

    private AudioSource _source;

    public Sound Sound { get => _sound; }
    public bool IsPlaying { get => _source.isPlaying; }

    private void Awake()
    {
        _source = GetComponent<AudioSource>();
        SetupAudio();
    }

    /// <summary>
    /// Sets up the AudioSource to play the sound, playing it if required.
    /// </summary>
    private void SetupAudio()
    {
        if (_sound is null) return;

        _source.playOnAwake = false;
        _source.clip = _sound.Clip;
        _source.outputAudioMixerGroup = _sound.Mixer;
        _source.volume = _sound.Volume;
        _source.pitch = _sound.Pitch;
        _source.loop = _sound.Loop;

        if (_sound.PlayOnAwake) PlaySound();
    }

    /// <summary>
    /// Plays the sound.
    /// </summary>
    /// <param name="mutate">Whether or not to make minor modifications to add variety.</param>
    public void PlaySound(bool mutate = false, bool canStack = false)
    {
        if (_sound is null) return;

        if (!mutate)
        {
            if (canStack) _source.PlayOneShot(_source.clip);
            else _source.Play();
        }
        else
        {
            var prevVolume = _sound.Volume;
            var prevPitch = _sound.Pitch;

            _sound.Volume *= Random.Range(_sound.VolumeRange.x, _sound.VolumeRange.y);
            _sound.Pitch *= Random.Range(_sound.PitchRange.x, _sound.PitchRange.y);

            if (canStack) _source.PlayOneShot(_source.clip);
            else _source.Play();

            _sound.Volume = prevVolume;
            _sound.Pitch = prevPitch;
        }
    }

    /// <summary>
    /// Stops the sound.
    /// </summary>
    public void StopSound()
    {
        _source.Stop();
    }

    /// <summary>
    /// Switches the sound for a new one.
    /// </summary>
    /// <param name="newSound">The new sound to replace the old one with.</param>
    public void SwitchSound(Sound newSound)
    {
        if (newSound == _sound) return;

        _sound = newSound;
        SetupAudio();
    }
}
