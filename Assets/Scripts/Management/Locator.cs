using UnityEngine;

public class Locator : MonoBehaviour
{
    public static Locator Instance;

    public GameManager GameManager { get; private set; }
    public InputManager InputManager { get; private set; }
    public MusicManager MusicManager { get; private set; }
    public BeatManager BeatManager { get; private set; }
    public PowerUpManager PowerUpManager { get; private set; }
    public Paddle Paddle { get; private set; }
    public Menu Menu { get; private set; }
    public GameOverlay GameOverlay { get; private set; }
    public BrickArea BrickArea { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }

    public void RegisterInstance(GameManager instance)
    {
        if (instance != null) GameManager = instance;
    }

    public void RegisterInstance(InputManager instance)
    {
        if (instance != null) InputManager = instance;
    }

    public void RegisterInstance(MusicManager instance)
    {
        if (instance != null) MusicManager = instance;
    }

    public void RegisterInstance(BeatManager instance)
    {
        if (instance != null) BeatManager = instance;
    }

    public void RegisterInstance(PowerUpManager instance)
    {
        if (instance != null) PowerUpManager = instance;
    }

    public void RegisterInstance(Paddle instance)
    {
        if (instance != null) Paddle = instance;
    }

    public void RegisterInstance(Menu instance)
    {
        if (instance != null) Menu = instance;
    }

    public void RegisterInstance(GameOverlay instance)
    {
        if (instance != null) GameOverlay = instance;
    }

    public void RegisterInstance(BrickArea instance)
    {
        if (instance != null) BrickArea = instance;
    }
}
