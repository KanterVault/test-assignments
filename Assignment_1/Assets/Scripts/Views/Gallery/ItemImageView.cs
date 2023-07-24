using UnityEngine;
using UnityEngine.UI;

public class ItemImageView : ItemBase
{
    [SerializeField] private Button button;

    public override void UpdateContent()
    {
        base.UpdateContent();
        
        if (Content != null)
        {
            var tex = (Texture2D) base.Content;
            var image = GetComponent<Image>();

            image.type = Image.Type.Simple;
            image.sprite = Sprite.Create(
                tex,
                new Rect(0, 0, tex.width, tex.height),
                Vector2.one * 0.5f);
        }

        button.onClick.AddListener(() =>
        {
            GridViewEventBridge.Instance.SelectItemButton?.Invoke(base.Guid);
        });
    }
}
