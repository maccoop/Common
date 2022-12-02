using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingManager : MonoBehaviour
{
    public delegate void BoolDelegate(bool active);
    public BoolDelegate OnActiveLoading;

    List<int> _cacheSceneLoading;
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
                    break;
                }
        }
        OnActiveLoading.Invoke(_cacheSceneLoading.Count != 0);
    }
}
