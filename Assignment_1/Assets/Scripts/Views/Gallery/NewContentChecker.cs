using System;
using UnityEngine;
using UnityEngine.UI;

public class NewContentChecker : MonoBehaviour
{
    public Action NewContent { get; set; }
    public Action CheckForNewContent { get; set; }

    [SerializeField] private RectTransform content;
    [SerializeField] private RectTransform viewPort;
    [SerializeField] private ScrollRect scrollRect;

    private void OnEnable()
    {
        scrollRect.onValueChanged.AddListener(OnScrollbarValueChanged);
    }

    private void OnScrollbarValueChanged(Vector2 vec)
    {
        if (content.rect.height - content.anchoredPosition.y - viewPort.rect.height / 2.0f < viewPort.rect.height)
        {
            NewContent?.Invoke();
        }
    }

    private void OnDisable()
    {
        scrollRect.onValueChanged.RemoveListener(OnScrollbarValueChanged);
    }
}
