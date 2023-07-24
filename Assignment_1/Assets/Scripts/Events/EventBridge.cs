using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.AppInputActions;

public class EventBridge : MonoBehaviour, IUIActions
{
    public static EventBridge Instance;

    [HideInInspector] public GameState gameState = GameState.None;
    [HideInInspector] public AppInputActions inputs;
    [HideInInspector] public ImageLocalCacheData StoredContainer = new ImageLocalCacheData();

    [SerializeField] private string bootstrapScene = "Bootstrap";
    [SerializeField] private string menuScene = "Menu";
    [SerializeField] private string gridViewScene = "GridViewScene";
    [SerializeField] private string itemPreviewScene = "ItemPreviewScene";

    [SerializeField] private Bootstrap bootstrap;
    [SerializeField] private LoadSystem loader;
    public DataFetcher DataFetcher;

    public Action<int> ContentLoadedProgress { get; set; }
    public Action BackButton { get; set; }

    private void Awake()
    {                     
        Instance = this;
        Screen.orientation = ScreenOrientation.Portrait;

        bootstrap.StartGame += () =>
            loader.LoadScene(menuScene, () =>
                loader.UnloadScn(bootstrapScene, () => gameState = GameState.Menu));

        inputs = new AppInputActions();
        inputs.UI.SetCallbacks(this);
    }

    private void OnEnable()
    {
        inputs.UI.Enable();
        ContentLoadedProgress += ContentLoaded;
        BackButton += BackButtonEvent;
    }
    private void OnDisable()
    {
        inputs.UI.Disable();
        ContentLoadedProgress -= ContentLoaded;
        BackButton -= BackButtonEvent;
    }

    public enum GameState
    {
        None,
        Menu,
        ItemsListView,
        ItemPreview
    }

    public void LoadMenu_Event(Action complete = null)
    {
        gameState = GameState.None;
        loader.ProgressBar(0.00f, () =>
        loader.ProgressBar(0.25f, () => loader.LoadScene(menuScene, () =>
        loader.ProgressBar(0.50f, () => loader.UnloadScn(gridViewScene, () =>
        loader.ProgressBar(0.75f, () => loader.UnloadScn(itemPreviewScene, () =>
        loader.ProgressBar(1.00f, () =>
        {
            gameState = GameState.Menu;
            complete?.Invoke();
        }
        ))))))));
    }

    private Action completeItemsViewScene;
    public void ContentLoaded(int index)
    {
        if (index > 10) return;
        var percents = index / 10.0f;
        loader.ProgressBar(percents, () =>
        {
            if (percents > 0.95f)
            {
                gameState = GameState.ItemsListView;
                completeItemsViewScene?.Invoke();
            }
        });
    }
    public void LoadItemsListEvent_Event(Action complete = null)
    {
        completeItemsViewScene = complete;
        gameState = GameState.None;
        loader.ProgressBar(0.00f, () =>
        loader.ProgressBar(0.05f, () => loader.LoadScene(gridViewScene, () =>
            loader.UnloadScn(itemPreviewScene, () =>
            loader.UnloadScn(menuScene, null)
        ))));
    }

    public void LoadItemPreview_Event(Action complete = null)
    {
        gameState = GameState.None;
        loader.ProgressBar(0.00f, () =>
        loader.ProgressBar(0.25f, () => loader.LoadScene(itemPreviewScene, () =>
        loader.ProgressBar(0.50f, () => loader.UnloadScn(menuScene, () =>
        loader.ProgressBar(0.75f, () => loader.UnloadScn(gridViewScene, () =>
        loader.ProgressBar(1.00f, () =>
        {
            gameState = GameState.ItemPreview;
            complete?.Invoke();
        }
        ))))))));
    }

    private void BackButtonEvent()
    {
        switch (gameState)
        {
            case GameState.ItemPreview: LoadItemsListEvent_Event(); break;
            case GameState.ItemsListView: LoadMenu_Event(); break;
            case GameState.Menu: Application.Quit(); break;
            default: break;
        }
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
