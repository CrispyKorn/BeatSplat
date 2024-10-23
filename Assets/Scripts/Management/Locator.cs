using UnityEngine;

public class Locator : MonoBehaviour
{
    public static Locator Instance;

    public InputManager InputManager { get; private set; }
    public Controller Controller { get; private set; }
    public PowerUpManager PowerUpManager { get; private set; }
    public Menu Menu { get; private set; }
    public GameOverlay GameOverlay { get; private set; }
    public BrickArea BrickArea { get; private set; }

    private void Awake()
    {
        if (Instance is null) Instance = this;
        else Destroy(this);
    }

    public void RegisterInstance(InputManager instance)
    {
        if (instance is InputManager) InputManager = instance;
    }

    public void RegisterInstance(Controller instance)
    {
        if (instance is Controller) Controller = instance;
    }

    public void RegisterInstance(PowerUpManager instance)
    {
        if (instance is PowerUpManager) PowerUpManager = instance;
    }

    public void RegisterInstance(Menu instance)
    {
        if (instance is Menu) Menu = instance;
    }

    public void RegisterInstance(GameOverlay instance)
    {
        if (instance is GameOverlay) GameOverlay = instance;
    }

    public void RegisterInstance(BrickArea instance)
    {
        if (instance is BrickArea) BrickArea = instance;
    }
}
