using System;
using UnityEngine;          
using System.Collections.Generic;

public class ContentSpawner : MonoBehaviour
{
    public Action<Guid, object> AddNewItem;

    [SerializeField] private RectTransform content;
    [SerializeField] private ItemBase itemPrefab;
    private List<GameObject> _contentItems = new List<GameObject>();

    private void OnEnable()
    {
        AddNewItem += AddNewItemInternal;
        _contentItems = new List<GameObject>();
    }

    private void AddNewItemInternal(Guid guid, object itemContent)
    {
        var item = Instantiate(itemPrefab.gameObject, content).GetComponent<ItemBase>();
        item.Guid = guid;
        item.Content = itemContent;
        item.UpdateContent();
        _contentItems.Add(item.gameObject);
    }

    private void OnDisable()
    {
        AddNewItem -= AddNewItemInternal;
        _contentItems.ForEach(f =>
        {
            if (f != null) Destroy(f);
        });
        _contentItems.Clear();
    }
}
