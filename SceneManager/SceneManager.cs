using UnityEngine;
using SceneManagement = UnityEngine.SceneManagement.SceneManager;
using LoadSceneMode = UnityEngine.SceneManagement.LoadSceneMode;
using AppSceneSetting = SceneLoadSetting;
using IEnumerator = System.Collections.IEnumerator;
using System;
using System.Collections.Generic;

public class SceneManager : SingletonBehaviour<SceneManager>
{
    private QueueService<(int, LoadSceneMode)> _queue;
    private ILog? Debug;
    public delegate void VoidDelegate(int indexSceneBuild);
    public delegate void BoolDelegate(int indexSceneBuild, bool value);
    public delegate void FloatDelegate(int indexSceneBuild, float value);
    public delegate void StringDelegate(int indexSceneBuild, string message);
    public delegate void AssetDelegate(string assetName);

    public VoidDelegate OnBeginLoadScene;
    public AssetDelegate OnLoadSceneAddressable;
    public AssetDelegate OnLoadSceneAssetBundle;
    public FloatDelegate OnLoadSceneProgress;
    public BoolDelegate OnLoadSceneCompleted;
    public BoolDelegate OnActiveLoading;
    public StringDelegate OnLoadSceneError;

    private List<SceneName> _histories;

    protected override void Init()
    {
        DontDestroyOnLoad(gameObject);
        OnBeginLoadScene        += new VoidDelegate((name) =>           Debug?.Log($"Begin Load Scene: {name}"));
        OnLoadSceneCompleted    += new BoolDelegate((name, value) =>    Debug?.Log($"Load Scene {name}: done. Status: {value}"));
        OnLoadSceneProgress     += new FloatDelegate((name, value) =>   Debug?.Log($"On Load Scene {name}: {value}%"));
        OnActiveLoading         += new BoolDelegate((name, value) =>    Debug?.Log($"Active Loading Scene {name}: {value}"));
        OnLoadSceneError        += new StringDelegate((name, message) => Debug?.LogError($"Loading Scene Error {name}: {message}"));
        OnLoadSceneAddressable += new AssetDelegate((name) =>       Debug?.Log($"Loading Scene Addressable: {name}"));
        OnLoadSceneAssetBundle  += new AssetDelegate((name) =>       Debug?.Log($"Loading Scene Assetbundle: {name}"));
        _queue = new QueueService<(int, LoadSceneMode)>();
        _queue.OnDequeue.AddListener(LoadSceneAction);
        _histories = new List<SceneName>();
        //Debug = new SangCustomLog();
    }

    public void UnloadScene(SceneName scene)
    {
        SceneSetting setting;
        int indexSceneBuild = (int)scene;
        GetSettingScene(scene, indexSceneBuild, out setting);
        if(setting.Mode == LoadSceneMode.Single)
        {
            Debug.LogError("Scene Load Mode single, can't Unload Scene!");
            return;
        }
        else
        {

        }
        /// continue
    }

    public void LoadScene(SceneName scene)
    {
        SceneSetting setting;
        int indexSceneBuild = (int)scene;
        GetSettingScene(scene, indexSceneBuild, out setting);
        if (setting.Addressable)
        {
            OnLoadSceneAddressable.Invoke(setting.AssetName);
            return;
        }
        if (setting.AssetBundle)
        {
            OnLoadSceneAssetBundle.Invoke(setting.AssetName);
            return;
        }
        if (!setting.Active)
        {
            OnLoadSceneError.Invoke(indexSceneBuild, string.Format("Scene {0} can't load, scene inactive on scene build setting", indexSceneBuild));
            return;
        }
        if (setting.SceneLoading)
            OnActiveLoading.Invoke(indexSceneBuild, true);
        _queue.AddQueue((indexSceneBuild, setting.Mode));
    }

    private static void GetSettingScene(SceneName scene, int indexSceneBuild, out SceneSetting setting)
    {
        setting = SceneSetting.Default;
        if (AppSceneSetting.settings.ContainsKey(indexSceneBuild))
        {
            setting = AppSceneSetting.settings[indexSceneBuild];
        }
    }

    private void LoadSceneAction((int indexSceneBuild, LoadSceneMode mode) setting)
    {
        switch (setting.mode)
        {
            case LoadSceneMode.Single:
                {
                    LoadSceneSingle(setting.indexSceneBuild);
                    break;
                }
            case LoadSceneMode.Additive:
                {
                    LoadSceneAddtive(setting.indexSceneBuild);
                    break;
                }
        }
    }

    private void LoadSceneAddtive(int indexSceneBuild)
    {
        StartCoroutine(LoadSceneCoroutine(indexSceneBuild, LoadSceneMode.Additive));
    }

    private void LoadSceneSingle(int indexSceneBuild)
    {
        StartCoroutine(LoadSceneCoroutine(indexSceneBuild, LoadSceneMode.Single));
    }

    private IEnumerator LoadSceneCoroutine(int indexSceneBuild, LoadSceneMode mode)
    {
        OnBeginLoadScene.Invoke(indexSceneBuild);
        AsyncOperation scene = null;
        try
        {
            scene = SceneManagement.LoadSceneAsync(indexSceneBuild, mode);
        }
        catch (Exception es)
        {
            OnLoadSceneError.Invoke(indexSceneBuild, es.Message);
        }
        if (scene != null)
        {
            while (!scene.isDone)
            {
                OnLoadSceneProgress.Invoke(indexSceneBuild, scene.progress);
                yield return new WaitForEndOfFrame();
            }
            OnLoadSceneCompleted.Invoke(indexSceneBuild, scene.isDone);
        }
        else
        {
            OnLoadSceneCompleted.Invoke(indexSceneBuild, false);
        }
        yield return new WaitForEndOfFrame();
        OnActiveLoading.Invoke(indexSceneBuild, false);
        _queue.EndQueue();
    }
}
