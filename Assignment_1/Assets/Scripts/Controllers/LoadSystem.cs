using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoadSystem : MonoBehaviour
{
    public Action<string, Action> LoadScene { get; set; }
    public Action<string, Action> UnloadScn { get; set; }
    public Action<float, Action> ProgressBar { get; set; }

    [SerializeField] private GameObject loadScreenPanel;
    [SerializeField] private Slider progressBarLine;
    [SerializeField] private TMP_Text progressPercentsText;
    [SerializeField] private float loadingSpeed = 0.05f;
    [SerializeField] private RectTransform loadingIndicator;

    private void Awake()
    {
        ProgressBar += SetLoadProgressInternal;
        LoadScene += LoadSceneInternal;
        UnloadScn += UnloadSceneInternal;
    }

    private void Update()
    {
        _indicatorAngle -= 100.0f * Time.deltaTime;
        loadingIndicator.localRotation = Quaternion.AngleAxis(_indicatorAngle, Vector3.forward);
    }

    private IEnumerator _loadingProgress;
    private float _targetPercents = 0.0f;
    private float _currentPercents = 0.0f;
    private void SetLoadProgressInternal(float value, Action action)
    {
        _targetPercents = value;
        if (_targetPercents < _currentPercents) _currentPercents = _targetPercents;
        if (!loadScreenPanel.activeInHierarchy)
        {
            loadScreenPanel.gameObject.SetActive(true);
        }
        if (_loadingProgress != null)
        {
            StopCoroutine(_loadingProgress);
            _loadingProgress = null;
        }
        if (_loadingProgress == null)
        {
            _loadingProgress = UpdateLoadBar(action);
            StartCoroutine(_loadingProgress);
        }
    }

    private float _indicatorAngle = 0.0f;
    private IEnumerator UpdateLoadBar(Action complete)
    {
        var waiter = new WaitForSeconds(0.05f);
        while (true)
        {
            _currentPercents = Mathf.Clamp(_currentPercents + loadingSpeed, 0.0f, 1.0f);
            if (loadScreenPanel.activeInHierarchy)
            {
                progressBarLine.value = _currentPercents;
                progressPercentsText.text = $"{(int)(_currentPercents * 100.0f)} %";
            }
            if (_currentPercents >= _targetPercents)
            {
                yield return waiter;
                if (_currentPercents >= 0.98f)
                {
                    if (loadScreenPanel.activeInHierarchy)
                    {
                        loadScreenPanel.gameObject.SetActive(false);
                    }
                }
                complete?.Invoke();
                yield break;
            }
            yield return waiter;
        }
    }

    private void UnloadSceneInternal(string sceneName, Action complete)
    {
        var scene = SceneManager.GetSceneByName(sceneName);
        if (scene == default)
        {
            complete?.Invoke();
        }
        else
        {
            SceneManager.UnloadSceneAsync(scene.name).completed += _ =>
            {
                complete?.Invoke();
            };
        }
    }

    private void LoadSceneInternal(string sceneName, Action complete)
    {
        var scene = SceneManager.GetSceneByName(sceneName);
        if (scene == default)
        {
            SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive).completed += _ =>
            {
                scene = SceneManager.GetSceneByName(sceneName);
                SceneManager.SetActiveScene(scene);
                complete?.Invoke();
            };
        }
        else
        {
            SceneManager.SetActiveScene(scene);
            complete?.Invoke();
        }
    }
}