using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.AppInputActions;

public class EventBridge : MonoBehaviour, IUIActions
{
    public static EventBridge Instance;

    [HideInInspector] public AppInputActions inputs;

    public Action BackButton { get; set; }

    private void Awake()
    {                     
        Instance = this;
        Screen.orientation = ScreenOrientation.Portrait;

        inputs = new AppInputActions();
        inputs.UI.SetCallbacks(this);
    }

    private void OnEnable()
    {
        inputs.UI.Enable();
    }
    private void OnDisable()
    {
        inputs.UI.Disable();
    }

    //App UI Events

    public void OnCancel(InputAction.CallbackContext callback)
    {
        if (callback.canceled || callback.started) return;
        BackButton?.Invoke();
    }

    public void OnNavigate(InputAction.CallbackContext context)
    {
    }

    public void OnSubmit(InputAction.CallbackContext context)
    {
    }

    public void OnPoint(InputAction.CallbackContext context)
    {
    }

    public void OnClick(InputAction.CallbackContext context)
    {
    }

    public void OnScrollWheel(InputAction.CallbackContext context)
    {
    }

    public void OnMiddleClick(InputAction.CallbackContext context)
    {
    }

    public void OnRightClick(InputAction.CallbackContext context)
    {
    }

    public void OnTrackedDevicePosition(InputAction.CallbackContext context)
    {
    }

    public void OnTrackedDeviceOrientation(InputAction.CallbackContext context)
    {
    }
}
