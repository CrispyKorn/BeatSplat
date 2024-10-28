using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "Sound", menuName = "Scriptable Objects/Sound")]
public class Sound : ScriptableObject
{
    [SerializeField] private AudioClip _clip;
    [SerializeField] private AudioMixerGroup _mixer;
    [SerializeField, Range(0f, 1f)] private float _volume = 1f;
    [SerializeField, Range(-3f, 3f)] private float _pitch = 1f;
    [SerializeField] private Vector2 _volumeRange = Vector2.one;
    [SerializeField] private Vector2 _pitchRange = Vector2.one;
    [SerializeField] private bool _loop = false;
    [SerializeField] private bool _playOnAwake = false;

    public AudioClip Clip { get => _clip; }
    public AudioMixerGroup Mixer { get => _mixer; }
    public float Volume { get => _volume; set => _volume = Mathf.Clamp(value, 0f, 1f); }
    public float Pitch { get => _pitch; set => _pitch = Mathf.Clamp(value, -3f, 3f); }
    public Vector2 VolumeRange { get => _volumeRange; }
    public Vector2 PitchRange { get => _pitchRange; }
    public bool Loop { get => _loop; }
    public bool PlayOnAwake { get => _playOnAwake; }
}
