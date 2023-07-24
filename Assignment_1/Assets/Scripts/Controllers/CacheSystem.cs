using System;
using System.IO;
using UnityEngine;

public class CacheSystem : MonoBehaviour
{
    public Action<string, byte[], Action> SaveToCache { get; set; }
    public Action<string, Action<byte[]>> LoadFromCache { get; set; }

    private string _cachePath = string.Empty;

    private void OnEnable()
    {
        _cachePath = Path.Combine(Application.persistentDataPath, "Cache");
        SaveToCache += SaveToCacheInternal;
        LoadFromCache += LoadFromCacheInternal;
    }

    private void OnDisable()
    {
        SaveToCache -= SaveToCacheInternal;
        LoadFromCache -= LoadFromCacheInternal;
    }

    private async void SaveToCacheInternal(string name, byte[] data, Action complete)
    {
        var filePath = Path.Combine(_cachePath, name);
        await File.WriteAllBytesAsync(filePath, data);
        complete?.Invoke();
    }

    private async void LoadFromCacheInternal(string name, Action<byte[]> data)
    {
        var filePath = Path.Combine(_cachePath, name);
        var loadedData = await File.ReadAllBytesAsync(filePath);
        data?.Invoke(loadedData);
    }
}
