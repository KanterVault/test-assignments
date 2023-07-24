using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class DataFetcher : MonoBehaviour
{
    public Action<string, Action<Texture2D>> GetTextureFromUrl { get; set; } 
    
    private void OnEnable()
    {
        GetTextureFromUrl += GetTextureFromUrlInternal;
    }

    private void GetTextureFromUrlInternal(string url, Action<Texture2D> complete) =>
        StartCoroutine(DownloadImage(url, complete));

    private void OnDisable()
    {
        GetTextureFromUrl -= GetTextureFromUrlInternal;
    }

    public IEnumerator DownloadImage(string url, Action<Texture2D> result)
    {
        var request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log($"Fetched image: {url}");
            result?.Invoke(((DownloadHandlerTexture)request.downloadHandler).texture);
        }
        else
        {
            Debug.Log($"Image download error: {url}");
            result?.Invoke(null);
        }
    }
}
