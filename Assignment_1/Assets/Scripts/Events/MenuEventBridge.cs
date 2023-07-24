using UnityEngine;
using UnityEngine.UI;

public class MenuEventBridge : MonoBehaviour
{
    public static MenuEventBridge Instance;

    [SerializeField] private Button galeryButton;
    [SerializeField] private Button quitButton;

    private void OnEnable()
    {
        Instance = this;
        Screen.orientation = ScreenOrientation.Portrait;
        if (EventBridge.Instance == null) return;
        galeryButton.onClick.AddListener(() => EventBridge.Instance.LoadItemsListEvent_Event());
        quitButton.onClick.AddListener(() => Application.Quit());
    }

    private void OnDisable()
    {
        galeryButton.onClick.RemoveAllListeners();
        quitButton.onClick.RemoveAllListeners();
    }
}
