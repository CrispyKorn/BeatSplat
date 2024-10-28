using System.Collections.Generic;
using UnityEngine;

public class PowerUpManager : MonoBehaviour
{
    /*
        This script is for controlling powerups
        This allows for power-ups to be created, move, actiavated, timed and deactivated
    */

    // This is used for activated powerups, it holds the timer and power up
    // Allows the powerup manager to handle the timer and powerup position
    private struct PowerUpInfo
    {
        public PowerUp powerUp;
        public TimerBar timer;
    }

    public Paddle Controller => _controller;
    public int PlayerLayer => _controller.gameObject.layer; // This is to get the player layer, this is used so a power up knows it hit the player and nothing else

    [Tooltip("This is for how many power ups to think ahead for, this makes sure we dont see the same power up in at least # of moves")]
    [SerializeField] private int _rememberedPowerUps = 3;
    [Tooltip("Chance of dropping power ups when a brick is destroyed.")]
    [SerializeField, Range(0f, 1f)] private float _powerUpPercentage = 0.5f;
    [Tooltip("Speed of falling power-ups in units/sec.")]
    [SerializeField, Range(0f, 2f)] public float _powerUpFallSpeed = 0.8f;
    [Tooltip("Timerbar used to display the time left on a power-up")]
    [SerializeField] private TimerBar _timerPrefab;
    [Tooltip("What power-ups are in play.")]
    [SerializeField] private List<PowerUp> _powerUpList = new();
    [Tooltip("Location of powers when activated")]
    [SerializeField] private List<Transform> _powerUpHolders = new();

    private Paddle _controller;
    private List<PowerUpInfo> _activatedPowerUps = new();
    private List<int> _upcomingPowerups = new();

    public float PowerUpPercentage { get => _powerUpPercentage; }

    private void Awake()
    {
        Locator.Instance.RegisterInstance(this);
    }

    private void Start()
    {
        _controller = Locator.Instance.Paddle;

        if (_rememberedPowerUps > _powerUpList.Count) _rememberedPowerUps = _powerUpList.Count;

        RefreshNextPowerUps();
    }

    private void RefreshNextPowerUps()
    {
        while (_upcomingPowerups.Count < _rememberedPowerUps)
        {
            int next = Random.Range(0, _powerUpList.Count);

            if (!_upcomingPowerups.Contains(next)) _upcomingPowerups.Add(next);
        }
    }

    // This is used when the controller thinks a power up should come out of a brick
    // Creates the power up and randomly chooses one, sets all the settings it needs
    public void SpawnPowerup(Vector2 hitBrick)
    {
        if (_powerUpList.Count > 0)
        {
            PowerUp powerUp = Instantiate(_powerUpList[_upcomingPowerups[0]], transform);

            powerUp.SetGravity(_powerUpFallSpeed);
            powerUp.gameObject.transform.position = hitBrick;

            _upcomingPowerups.RemoveAt(0);
            RefreshNextPowerUps();
        }
        else Debug.LogError("No power ups attached to power up manager");
    }

    public void ActivatePowerUp(PowerUp powerUp)
    {
        PowerUpInfo activatedPowerUp = new PowerUpInfo();

        if (_activatedPowerUps.Count >= _powerUpHolders.Count) RemoveActivatedPower(_activatedPowerUps[0]); // Make sure there is a holder spare. Otherwise, delete the first one

        powerUp.Freeze();
        powerUp.ActivatePowerUp();
        activatedPowerUp.powerUp = powerUp;

        //Create the timer for the power up
        if (_timerPrefab is TimerBar)
        {
            TimerBar tempTimer = Instantiate(_timerPrefab, powerUp.transform);
            tempTimer.TimeLeft = powerUp.Duration;
            tempTimer.Active = true;

            activatedPowerUp.timer = tempTimer;
        }
        else Debug.Log("Missing Timer in Power-Up Manager!");

        //Make sure the powerup and the timer are set, add to the list of activated power ups and also move to the correct location/holder
        if (activatedPowerUp.powerUp is PowerUp && activatedPowerUp.timer is TimerBar)
        {
            if (activatedPowerUp.timer.TimeLeft > 0f) activatedPowerUp.timer.OnTimerFinished += RemovePowerUp;
            MoveToHolder(activatedPowerUp, _powerUpHolders[_activatedPowerUps.Count]);
            _activatedPowerUps.Add(activatedPowerUp);
        }
    }

    private void RemovePowerUp(TimerBar timer)
    {
        if (!_activatedPowerUps.Exists(p => p.timer == timer)) return;

        PowerUpInfo powerUp = _activatedPowerUps.Find(p => p.timer == timer);
        RemoveActivatedPower(powerUp);
    }

    //This is to deactivate and remove a power up
    private void RemoveActivatedPower(PowerUpInfo powerUp)
    {
        powerUp.powerUp.DeactivatePowerUp();
        
        Destroy(powerUp.powerUp.gameObject);
        Destroy(powerUp.timer.gameObject);

        _activatedPowerUps.Remove(powerUp);
    }

    //This function moves a powerup to the correct position and scale
    private PowerUpInfo MoveToHolder(PowerUpInfo activatedPowerUp, Transform holder)
    {
        activatedPowerUp.powerUp.transform.parent = holder; // Move the powerup to be a child of the holder
        activatedPowerUp.powerUp.transform.localPosition = Vector2.zero;
        activatedPowerUp.timer.transform.localPosition = Vector2.zero;
        activatedPowerUp.powerUp.transform.localRotation = Quaternion.identity;
        activatedPowerUp.powerUp.transform.localScale = Vector2.one;

        return activatedPowerUp;
    }

    public void KillPowerUps()
    {
        foreach (var powerUp in _activatedPowerUps)
        {
            powerUp.powerUp.DeactivatePowerUp();

            Destroy(powerUp.powerUp.gameObject);
            Destroy(powerUp.timer.gameObject);
        }

        _activatedPowerUps.Clear();
    }
    
    private void Update()
    {
        if (_activatedPowerUps.Count == 0) return;

        for (var i = 0; i < _activatedPowerUps.Count; i++)
        {
            MoveToHolder(_activatedPowerUps[i], _powerUpHolders[i]);
        }
    }
}
