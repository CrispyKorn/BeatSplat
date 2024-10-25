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
        var musicBase = _preLaunchLoop.time - _semiQuaverTime;
        var musicTime = musicBase % _crotchetTime;

        UpdateBeatFX(musicBase);

        bool canTick = musicTime >= _semiQuaverTime * TickCount && musicTime < _semiQuaverTime * (TickCount + 1);

        if (canTick) UpdateTick();
    }

    private void UpdateBeatFX(float musicOffset)
    {
        var timeInBar = musicOffset % (_crotchetTime * 4);
        _changeIndicator.transform.localScale = new Vector3(1f, timeInBar * 5f, 1f);

        // Pulse camera in time with music
        var barOffset = timeInBar / 8f;
        var timeInBeat = musicOffset % _crotchetTime;
        var beatOffset = timeInBeat / 4f;
        Camera.main.orthographicSize = 5f + barOffset + beatOffset;
    }

    private void UpdateTick()
    {
        TickCount++;
        //Debug.Log($"Tick {TickCount}");

        OnSemiQuaver?.Invoke(TickCount);

        if (TickCount == 4)
        {
            UpdateBeat();
        }

        TickCount %= 4;
    }

    private void UpdateBeat()
    {
        BeatCount++;
        //Debug.Log($"Beat {BeatCount}");

        OnBeat?.Invoke(BeatCount);
    }
}
