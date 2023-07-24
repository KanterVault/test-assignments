using System;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class PreviewGuests : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IScrollHandler
{
    public Action<float> ChangeScaleDelta { get; set; }
    public Action<float> ChangeScaleOffset { get; set; }
    public Action<Vector2> ChangePositionDelta { get; set; }

    [SerializeField] private float scrollSensitivity = 10.0f;

    private Dictionary<int, Vector2> _touchs = new Dictionary<int, Vector2>();
    private float _firstDistance = -1.0f;

    public void OnScroll(PointerEventData eventData) => ChangeScaleDelta?.Invoke(eventData.scrollDelta.y * scrollSensitivity);

    public void OnDrag(PointerEventData eventData)
    {
        if (_touchs.ContainsKey(eventData.pointerId))
        {
            _touchs[eventData.pointerId] = eventData.position;
        }

        if (_touchs.Count == 1)
        {
            ChangePositionDelta?.Invoke(eventData.delta);
        }
        else if (_touchs.Count >= 2)
        {
            var dist = Vector2.Distance(
                _touchs.First().Value,
                _touchs.Skip(1).First().Value);

            if (_firstDistance < 0.0f)
            {
                _firstDistance = dist;
            }

            var resultSize = dist - _firstDistance;
            ChangeScaleOffset?.Invoke(resultSize);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _touchs.TryAdd(eventData.pointerId, eventData.position);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (_touchs.ContainsKey(eventData.pointerId))
        {
            _touchs.Remove(eventData.pointerId);
            _firstDistance = -1.0f;
        }
    }

    private void OnEnable() => Reset();
    private void OnDisable() => Reset();

    private void Reset()
    {
        _touchs.Clear();
        _firstDistance = -1.0f;
    }
}
