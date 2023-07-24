using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Bootstrap : MonoBehaviour
{
    public Action StartGame { get; set; }

    [SerializeField] private List<GameObject> dontDestroyOnLoad;

    private IEnumerator Start()
    {
        Screen.fullScreen = false;
        Application.targetFrameRate = Screen.currentResolution.refreshRate;
        QualitySettings.vSyncCount = 0;
        dontDestroyOnLoad.ForEach(o => DontDestroyOnLoad(o));
        yield return null;
        StartGame();
    }
}
