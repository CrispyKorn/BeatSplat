using UnityEngine;

public class GameManager : MonoBehaviour
{
    private Locator _locator;

    private void Awake()
    {
        _locator = Locator.Instance;
        _locator.RegisterInstance(this);
    }

    private void Start()
    {
        _locator.InputManager.SetCursorMode(true);
        _locator.InputManager.SetActionMap(InputManager.ActionMap.Menu);
    }
}
