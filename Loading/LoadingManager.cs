using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingManager : MonoBehaviour
{
    const float DELAY_TIME_DISABLE_LOADING_SCREEN = 0.4f;
    public delegate void BoolDelegate(bool active);
    public BoolDelegate OnActiveLoading;

    private List<int> _cacheSceneLoading;
    private Coroutine _disableLoading;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        _cacheSceneLoading = new List<int>();
        SceneManager.Singleton.OnActiveLoading += ActiveLoading;
    }

    private void ActiveLoading(int indexSceneBuild, bool value)
    {
        if (_disableLoading != null)
            StopCoroutine(_disableLoading);
        switch (value)
        {
            case true:
                {
                    _cacheSceneLoading.Add(indexSceneBuild);
                    break;
                }
            case false:
                {
                    if (_cacheSceneLoading.Contains(indexSceneBuild))
                        _cacheSceneLoading.Remove(indexSceneBuild);
                    _disableLoading = StartCoroutine(DisableLoading());
                    break;
                }
        }
    }

    private IEnumerator DisableLoading()
    {
        yield return new WaitForSecondsRealtime(DELAY_TIME_DISABLE_LOADING_SCREEN);
        OnActiveLoading.Invoke(_cacheSceneLoading.Count != 0);
    }
}
