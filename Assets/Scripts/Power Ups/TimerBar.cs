using UnityEngine;

public class TimerBar : MonoBehaviour
{
    //This is used to allow the timer to tick forward, if not set, it wont move
    public bool Active { get => _active; set => _active = value; }

    public float TimeLeft
    {
        get
        {
            return _timeLeft;
        }
        set
        {
            _maxTime = value;
            _timeLeft = value;
        }
    }

    [Tooltip("The timer bar used to show the beat.")]
    [SerializeField] private Transform _frontOfBar;

    private float _maxTime = 100f;
    private float _timeLeft;
    private bool _active = false;

    private void Update()
    {
        if (!_active) return;

        _timeLeft -= Time.unscaledDeltaTime;

        if (_timeLeft < 0) _timeLeft = 0;

        _frontOfBar.localScale = new Vector2(_timeLeft / _maxTime, _frontOfBar.localScale.y);
    }
}
