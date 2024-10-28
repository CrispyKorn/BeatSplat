using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [Tooltip("The colour theme to use.")]
    [SerializeField] private List<Theme> _themes;

    private Locator _locator;

    public Theme Theme { get; private set; }

    private void Awake()
    {
        _locator = Locator.Instance;
        _locator.RegisterInstance(this);

        Theme = _themes[Random.Range(0, _themes.Count)];
    }

    private void Start()
    {
        _locator.InputManager.SetCursorMode(true);
        _locator.InputManager.SetActionMap(InputManager.ActionMap.Menu);
    }
}
