using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public enum ActionMap
    {
        None,
        Player,
        Menu
    }

    public enum PlayerInputDevice
    {
        KBM,
        Gamepad
    }

    #region Input Events
    #region Player
    public event Action OnMove_Started;
    public event Action<float> OnMove;
    public event Action OnMove_Ended;

    public event Action OnRelease_Started;
    public event Action OnRelease;
    public event Action OnRelease_Ended;
    #endregion

    #region Menu
    public event Action<Vector2> OnNavigate;

    public event Action OnSubmit_Started;
    public event Action OnSubmit;
    public event Action OnSubmit_Ended;

    public event Action OnCancel_Started;
    public event Action OnCancel;
    public event Action OnCancel_Ended;
    #endregion
    #endregion

    private PlayerInput m_playerInput;

    public ActionMap CurrentActionMap { get; private set; } = ActionMap.None;
    public PlayerInputDevice CurrentInputDevice { get; private set; } = PlayerInputDevice.KBM;

    private void Awake()
    {
        Locator.Instance.RegisterInstance(this);
        m_playerInput = new PlayerInput();
    }

    private void OnEnable()
    {
        #region Player
        m_playerInput.Player.Move.started += Move_started;
        m_playerInput.Player.Move.performed += Move_performed;
        m_playerInput.Player.Move.canceled += Move_canceled;

        m_playerInput.Player.Release.started += Release_started;
        m_playerInput.Player.Release.performed += Release_performed;
        m_playerInput.Player.Release.canceled += Release_canceled;
        #endregion

        #region Menu
        m_playerInput.Menu.Navigate.performed += Navigate_performed;

        m_playerInput.Menu.Submit.started += Submit_started;
        m_playerInput.Menu.Submit.performed += Submit_performed;
        m_playerInput.Menu.Submit.canceled += Submit_canceled;

        m_playerInput.Menu.Cancel.started += Cancel_started;
        m_playerInput.Menu.Cancel.performed += Cancel_performed;
        m_playerInput.Menu.Cancel.canceled += Cancel_canceled;
        #endregion
    }

    private void OnDisable()
    {
        #region Player
        m_playerInput.Player.Move.started -= Move_started;
        m_playerInput.Player.Move.performed -= Move_performed;
        m_playerInput.Player.Move.canceled -= Move_canceled;

        m_playerInput.Player.Release.started -= Release_started;
        m_playerInput.Player.Release.performed -= Release_performed;
        m_playerInput.Player.Release.canceled -= Release_canceled;
        #endregion

        #region Menu
        m_playerInput.Menu.Navigate.performed -= Navigate_performed;

        m_playerInput.Menu.Submit.started -= Submit_started;
        m_playerInput.Menu.Submit.performed -= Submit_performed;
        m_playerInput.Menu.Submit.canceled -= Submit_canceled;

        m_playerInput.Menu.Cancel.started -= Cancel_started;
        m_playerInput.Menu.Cancel.performed -= Cancel_performed;
        m_playerInput.Menu.Cancel.canceled -= Cancel_canceled;
        #endregion
    }

    #region Input Methods
    #region Player
    #region Move
    private void Move_started(InputAction.CallbackContext context)
    {
        UpdateInputDevice(context.control.device);
        OnMove_Started?.Invoke();
    }

    private void Move_performed(InputAction.CallbackContext context)
    {
        UpdateInputDevice(context.control.device);
        var inputValue = context.ReadValue<Vector2>().x;
        OnMove?.Invoke(inputValue);
    }

    private void Move_canceled(InputAction.CallbackContext context)
    {
        UpdateInputDevice(context.control.device);
        OnMove_Ended?.Invoke();
    }
    #endregion

    #region Release
    private void Release_started(InputAction.CallbackContext context)
    {
        UpdateInputDevice(context.control.device);
        OnRelease_Started?.Invoke();
    }

    private void Release_performed(InputAction.CallbackContext context)
    {
        UpdateInputDevice(context.control.device);
        OnRelease?.Invoke();
    }

    private void Release_canceled(InputAction.CallbackContext context)
    {
        UpdateInputDevice(context.control.device);
        OnRelease_Ended?.Invoke();
    }
    #endregion
    #endregion

    #region Menu
    #region Navigate
    private void Navigate_performed(InputAction.CallbackContext context)
    {
        UpdateInputDevice(context.control.device);
        var inputValue = context.ReadValue<Vector2>();
        OnNavigate?.Invoke(inputValue);
    }
    #endregion

    #region Submit
    private void Submit_started(InputAction.CallbackContext context)
    {
        UpdateInputDevice(context.control.device);
        OnSubmit_Started?.Invoke();
    }
    private void Submit_performed(InputAction.CallbackContext context)
    {
        UpdateInputDevice(context.control.device);
        OnSubmit?.Invoke();
    }
    private void Submit_canceled(InputAction.CallbackContext context)
    {
        UpdateInputDevice(context.control.device);
        OnSubmit_Ended?.Invoke();
    }
    #endregion

    #region Cancel
    private void Cancel_started(InputAction.CallbackContext context)
    {
        UpdateInputDevice(context.control.device);
        OnCancel_Started?.Invoke();
    }

    private void Cancel_performed(InputAction.CallbackContext context)
    {
        UpdateInputDevice(context.control.device);
        OnCancel?.Invoke();
    }

    private void Cancel_canceled(InputAction.CallbackContext context)
    {
        UpdateInputDevice(context.control.device);
        OnCancel_Ended?.Invoke();
    }
    #endregion
    #endregion
    #endregion

    private void UpdateInputDevice(InputDevice inputDevice)
    {
        if (inputDevice != null) CurrentInputDevice = PlayerInputDevice.Gamepad;
        if (inputDevice != null || inputDevice != null) CurrentInputDevice = PlayerInputDevice.KBM;
    }

    public void SetCursorMode(bool locked)
    {
        if (locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void SetActionMap(ActionMap actionMap)
    {
        switch (actionMap)
        {
            case ActionMap.None:
                {
                    m_playerInput.Player.Disable();
                    m_playerInput.Menu.Disable();
                }
                break;
            case ActionMap.Player:
                {
                    m_playerInput.Player.Enable();
                    m_playerInput.Menu.Disable();
                }
                break;
            case ActionMap.Menu:
                {
                    m_playerInput.Player.Disable();
                    m_playerInput.Menu.Enable();
                }
                break;
        }
    }
}
