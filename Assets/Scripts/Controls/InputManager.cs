using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public event Action OnMove_Started;
    public event Action<float> OnMove;
    public event Action OnMove_Ended;

    public event Action OnRelease_Started;
    public event Action OnRelease;
    public event Action OnRelease_Ended;

    private PlayerInput m_playerInput;

    private void Awake()
    {
        Locator.Instance.RegisterInstance(this);
        m_playerInput = new PlayerInput();

        m_playerInput.Player.Enable();
    }

    private void OnEnable()
    {
        m_playerInput.Player.Move.started += Move_started;
        m_playerInput.Player.Move.performed += Move_performed;
        m_playerInput.Player.Move.canceled += Move_canceled;

        m_playerInput.Player.Release.started += Release_started;
        m_playerInput.Player.Release.performed += Release_performed;
        m_playerInput.Player.Release.canceled += Release_canceled;
    }

    private void OnDisable()
    {
        m_playerInput.Player.Move.started -= Move_started;
        m_playerInput.Player.Move.performed -= Move_performed;
        m_playerInput.Player.Move.canceled -= Move_canceled;

        m_playerInput.Player.Release.started -= Release_started;
        m_playerInput.Player.Release.performed -= Release_performed;
        m_playerInput.Player.Release.canceled -= Release_canceled;
    }

    #region Player
    #region Move
    private void Move_started(InputAction.CallbackContext context)
    {
        OnMove_Started?.Invoke();
    }

    private void Move_performed(InputAction.CallbackContext context)
    {
        OnMove?.Invoke(context.ReadValue<float>());
    }

    private void Move_canceled(InputAction.CallbackContext context)
    {
        OnMove_Ended?.Invoke();
    }
    #endregion

    #region Release
    private void Release_started(InputAction.CallbackContext context)
    {
        OnRelease_Started?.Invoke();
    }

    private void Release_performed(InputAction.CallbackContext context)
    {
        OnRelease?.Invoke();
    }

    private void Release_canceled(InputAction.CallbackContext context)
    {
        OnRelease_Ended?.Invoke();
    }
    #endregion
    #endregion

    #region Menu

    #endregion
}
