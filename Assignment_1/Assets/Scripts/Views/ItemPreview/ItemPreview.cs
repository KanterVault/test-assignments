using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ItemPreview : MonoBehaviour
{
    public Action ResetPreview { get; set; }
    public Action<float> RescalePreviewOffset { get; set; }
    public Action<float> RescalePreviewDelta { get; set; }
    public Action<Vector2> ChangePositionDelta { get; set; }
    public Action<StoredImage> UpdatePreview { get; set; }

    [SerializeField] private bool debug;
    [SerializeField] private Texture2D debugImage;

    [Space(20)]
    [SerializeField] private Image itemImage;
    [SerializeField] private float deadZone = 10.0f;
    [SerializeField] private float zoomSensitivity = 0.001f;

    [SerializeField] private RectTransform viewportRect;
    [SerializeField] private Canvas canvas;

    private void Awake()
    {
        UpdatePreview += UpdatePreviewInternal;
        RescalePreviewOffset += RescalePreviewOffsetInternal;
        RescalePreviewDelta += RescalePreviewDeltaInternal;
        ResetPreview += ResetPreviewInternal;
        ChangePositionDelta += ChangePositionDeltaInternal;

        if (debug) UpdatePreviewInternal(new StoredImage() { Guid = Guid.NewGuid(), Index = 1, Texture = debugImage });
    }

    private void ChangePositionDeltaInternal(Vector2 delta)
    {
        itemImage.rectTransform.anchoredPosition += delta;
    }

    private void RescalePreviewDeltaInternal(float delta)
    {
        itemImage.rectTransform.localScale = Vector3.one * Mathf.Clamp(
           itemImage.rectTransform.localScale.x + (delta * zoomSensitivity * itemImage.rectTransform.localScale.x),
           0.5f, 20.0f);
    }

    private float _lastScale = 1.0f;
    private void RescalePreviewOffsetInternal(float sizeOffset)
    {
        if (Mathf.Abs(sizeOffset) < deadZone)
        {
            _lastScale = itemImage.rectTransform.localScale.x;
        }

        itemImage.rectTransform.localScale = Vector3.one * Mathf.Clamp(
            _lastScale + (sizeOffset * zoomSensitivity * itemImage.rectTransform.localScale.x / canvas.scaleFactor),
            0.25f, 20.0f);
    }

    private void UpdatePreviewInternal(StoredImage image)
    {
        if (image == null || image.Texture == null)
        {
            itemImage.sprite = null;                                                                                      
        }
        else
        {
            var sprite = Sprite.Create(
                image.Texture,
                new Rect(0, 0, image.Texture.width, image.Texture.height),
                Vector2.one * 0.5f);

            itemImage.type = Image.Type.Simple;
            itemImage.sprite = sprite;

            itemImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, image.Texture.width);
            itemImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, image.Texture.height);
            ResetPreviewInternal();
        }
    }

    private void ResetPreviewInternal() => StartCoroutine(UpdateSize());
    private IEnumerator UpdateSize()
    {
        yield return null;
        itemImage.rectTransform.localScale = viewportRect.rect.height > viewportRect.rect.width ?
            Vector3.one * viewportRect.rect.width / itemImage.rectTransform.rect.width :
            Vector3.one * viewportRect.rect.height / itemImage.rectTransform.rect.height;
        itemImage.rectTransform.anchoredPosition = Vector2.zero;
        _lastScale = itemImage.rectTransform.localScale.x;
    }

    private void OnDisable()
    {
        UpdatePreview -= UpdatePreviewInternal;
        RescalePreviewOffset -= RescalePreviewOffsetInternal;
        RescalePreviewDelta -= RescalePreviewDeltaInternal;
        ResetPreview -= ResetPreviewInternal;
        ChangePositionDelta -= ChangePositionDeltaInternal;
    }
}
