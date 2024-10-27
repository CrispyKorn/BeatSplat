using System;
using UnityEngine;

public class BeatManager : MonoBehaviour
{
    [Tooltip("The BPM of the music.")]
    [SerializeField] private int _bpm = 120;
    [Tooltip("Music that plays preceeding ball launch.")]
    [SerializeField] private AudioSource _preLaunchLoop;
    [Tooltip("The bars on the side that show the beat.")]
    [SerializeField] private GameObject _changeIndicator;
    [SerializeField] private float _musicOffset;

    private float _crotchetTime, _semiQuaverTime;

    public int TickCount { get; private set; }
    public int BeatCount { get; private set; }

    public event Action<int> OnSemiQuaver;
    public event Action<int> OnBeat;

    private void Awake()
    {
        Locator.Instance.RegisterInstance(this);

        _crotchetTime = 60f / _bpm;
        _semiQuaverTime = _crotchetTime / 4f;
    }

    private void Update()
    {
        var musicBase = _preLaunchLoop.time - _musicOffset;
        var musicTime = musicBase % _crotchetTime;

        UpdateBeatFX(musicBase);

        bool canTick = musicTime >= _semiQuaverTime * TickCount && musicTime < _semiQuaverTime * (TickCount + 1);

        if (canTick) UpdateTick();
    }

    private void UpdateBeatFX(float musicBase)
    {
        // Pulse in time with music
        var timeInBar = musicBase % (_crotchetTime * 4);
        var baseCameraSize = 5f;
        var beatPulseStrength = 4f;
        var barPulseStrength = 8f;
        var barOffset = timeInBar / barPulseStrength;
        var timeInBeat = musicBase % _crotchetTime;
        var beatOffset = timeInBeat / beatPulseStrength;
        var cameraSize = baseCameraSize + barOffset + beatOffset;
        Camera.main.orthographicSize = cameraSize;
        _changeIndicator.transform.localScale = new Vector3(1f, timeInBar * cameraSize, 1f);
    }

    private void UpdateTick()
    {
        TickCount++;
        //Debug.Log($"Tick {TickCount}");

        OnSemiQuaver?.Invoke(TickCount);

        TickCount %= 4;

        if (TickCount == 1) UpdateBeat();
    }

    private void UpdateBeat()
    {
        BeatCount++;
        //Debug.Log($"Beat {BeatCount}");

        OnBeat?.Invoke(BeatCount);
    }
}
