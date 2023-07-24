using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[AddComponentMenu("Layout/Grid Child Size Fitter", 153)]
[RequireComponent(typeof(GridLayoutGroup))]
public class GridChildSizeFitter : UIBehaviour, ILayoutSelfController
{
    [SerializeField] private RectTransform content;
    [SerializeField] private RectTransform viewPort;
    private GridLayoutGroup _grid;

    public void SetLayoutHorizontal() => UpdateLayout();
    public void SetLayoutVertical() => UpdateLayout();

    private void UpdateLayout()
    {
        _grid = null;
        TryGetComponent(out _grid);
        if (_grid == null) return;
        var size =
            content.rect.width / _grid.constraintCount -
            _grid.padding.right -
            _grid.spacing.x / _grid.constraintCount;
        _grid.cellSize = new Vector2(size, size);
        //_grid.padding.bottom = (int)(viewPort.rect.height / 2.0f);
    }
}
