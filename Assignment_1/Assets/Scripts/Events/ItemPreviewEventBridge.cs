using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ItemPreviewEventBridge : MonoBehaviour
{
    public static ItemPreviewEventBridge Instance;

    [SerializeField] private ItemPreview itemPreview;
    [SerializeField] private PreviewGuests previewGuests;
    [SerializeField] private Button backButton;
    [SerializeField] private Button resetViewButton;

    private void Awake()
    {
        Instance = this;
        Screen.orientation =
            ScreenOrientation.Portrait |
            ScreenOrientation.LandscapeLeft |
            ScreenOrientation.LandscapeRight;
    }

    private void OnEnable()
    {
        if (EventBridge.Instance?.StoredContainer?.CurrentSelected != default)
        {
            itemPreview.UpdatePreview?.Invoke(EventBridge.Instance.StoredContainer.CurrentSelected);
            backButton.onClick.AddListener(BackButtonEvent);
        }
        previewGuests.ChangePositionDelta += ChangePositionDeltaEvent;
        previewGuests.ChangeScaleOffset += RescaleImageOffsetEvent;
        previewGuests.ChangeScaleDelta += RescaleImageDeltaEvent;
        resetViewButton.onClick.AddListener(ResetViewButtonEvent);
        StartCoroutine(CheckOrientation());
    }

    private IEnumerator CheckOrientation()
    {
        var checkDelay = new WaitForSeconds(1.0f);
        var lastOrientation = Input.deviceOrientation;
        var lastResolution = new Vector2(Screen.width, Screen.height);
        while (true)
        {
            if (lastResolution.x != Screen.width || lastResolution.y != Screen.height)
            {
                lastResolution = new Vector2(Screen.width, Screen.height);
                ChangeResolution();
            }

            if (lastOrientation != Input.deviceOrientation)
            {
                lastOrientation = Input.deviceOrientation;
                ChangeOrientation();
            }
            yield return checkDelay;
        }
    }

    private void ChangePositionDeltaEvent(Vector2 delta) => itemPreview.ChangePositionDelta?.Invoke(delta);
    private void ChangeResolution() => ResetViewButtonEvent();
    private void ChangeOrientation() => ResetViewButtonEvent();
    private void RescaleImageOffsetEvent(float size) => itemPreview.RescalePreviewOffset?.Invoke(size);
    private void RescaleImageDeltaEvent(float size) => itemPreview.RescalePreviewDelta?.Invoke(size);
    private void BackButtonEvent() => EventBridge.Instance.BackButton?.Invoke();
    private void ResetViewButtonEvent() => itemPreview.ResetPreview?.Invoke();

    private void OnDisable()
    {
        StopCoroutine(CheckOrientation());
        backButton.onClick.RemoveListener(BackButtonEvent);
        resetViewButton.onClick.RemoveListener(ResetViewButtonEvent);
        previewGuests.ChangePositionDelta -= ChangePositionDeltaEvent;
        previewGuests.ChangeScaleOffset -= RescaleImageOffsetEvent;
        previewGuests.ChangeScaleDelta -= RescaleImageDeltaEvent;
    }
}
