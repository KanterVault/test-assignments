using System;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using System.Collections;

public class GridViewEventBridge : MonoBehaviour
{
    public static GridViewEventBridge Instance;

    public Action<Guid> SelectItemButton { get; set; }

    [SerializeField] private Button backButton;
    [SerializeField] private NewContentChecker contentChecker;
    [SerializeField] private ContentSpawner contentSpawner;

    private void Awake()
    {
        Instance = this;
        Screen.orientation = ScreenOrientation.Portrait;
    }

    private void OnEnable()
    {
        _enabledFetch = true;
        _imageIndex = 0;
        SelectItemButton += ItemClickEvent;
        if (EventBridge.Instance?.StoredContainer != null)
        {
            backButton.onClick.AddListener(BackButtonEvent);
            contentChecker.NewContent += AddImageData;
            for (var i = 0; i < 10; i++) AddImageData();
            StartCoroutine(DrawItems());
        }
    }

    private void BackButtonEvent() => EventBridge.Instance.BackButton?.Invoke();

    private void ItemClickEvent(Guid guid)
    {
        var selected = EventBridge.Instance.StoredContainer.StoredImages.FirstOrDefault(f => f.Guid.Equals(guid));
        if (selected != default)
        {
            EventBridge.Instance.StoredContainer.CurrentSelected = selected;
            EventBridge.Instance.LoadItemPreview_Event(() => Debug.Log($"Loaded preview. Guid: {guid}"));
        }
    }

    private IEnumerator DrawItems()
    {
        var _currentViewIndex = 0;
        while (true)
        {
            var currentItem = EventBridge.Instance.StoredContainer.StoredImages.FirstOrDefault(f => f.Index == _currentViewIndex + 1);
            if (currentItem != default && contentSpawner != null && contentSpawner.AddNewItem != null)
            {
                contentSpawner.AddNewItem.Invoke(currentItem.Guid, currentItem.Texture);
                _currentViewIndex++;
                EventBridge.Instance.ContentLoadedProgress?.Invoke(_currentViewIndex);
                _requestCounts = Mathf.Clamp(_requestCounts - 1, 0, int.MaxValue);
            }
            yield return null;
        }
    }

    private bool _enabledFetch = true;
    private int _imageIndex = 0;
    private int _requestCounts = 0;
    private void AddImageData()
    {
        if (!_enabledFetch) return;
        if (_requestCounts > 10) return;

        _imageIndex++;
        var currentIndex = _imageIndex;

        var currentItem = EventBridge.Instance
            .StoredContainer.StoredImages
            .FirstOrDefault(f => f.Index == currentIndex);

        if (currentItem != default)
        {
            return;
        }
  
        _requestCounts++;
        var url = $"http://data.ikppbb.com/test-task-unity-data/pics/{currentIndex}.jpg";
        EventBridge.Instance.DataFetcher.GetTextureFromUrl(url, tex =>
        {
            if (contentSpawner != null && tex != null)
            {
                EventBridge.Instance.StoredContainer.StoredImages.Add(new StoredImage()
                {
                    Guid = Guid.NewGuid(),
                    Index = currentIndex,
                    Texture = tex
                });
            }
            else
            {
                _enabledFetch = false;
            }
        });
    }

    private void OnDisable()
    {
        if (EventBridge.Instance?.StoredContainer != null)
        {
            StopCoroutine(DrawItems());
            contentChecker.NewContent -= AddImageData;
            backButton.onClick.RemoveListener(BackButtonEvent);
        }
        SelectItemButton -= ItemClickEvent;
        _enabledFetch = false;
        _imageIndex = 0;
        _requestCounts = 0;
    }
}
