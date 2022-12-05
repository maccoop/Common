using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector;
using System.Linq;
using System;
using LoadSceneMode = UnityEngine.SceneManagement.LoadSceneMode;


[Serializable]
public class SceneObjectData
{
    [OnValueChanged(nameof(OnChangeAsset))]
    public SceneAsset asset;
    [OnValueChanged(nameof(OnActiveChange))]
    public bool active;
    public bool activeLoading;
    public LoadSceneMode mode;
    [OnValueChanged(nameof(OnAddressableChange))]
    public bool addressable;
    [OnValueChanged(nameof(OnAssetBundleChange))]
    public bool assetBundle;
    [ShowIf("@this.assetBundle || this.addressable"), ReadOnly]
    public string assetName;


    private void OnChangeAsset()
    {
        if (asset != null)
            assetName = asset.name;
        else
            assetName = "";
    }
    private void OnActiveChange()
    {
        if (active)
        {
            addressable = false;
            assetBundle = false;
        }
    }
    private void OnAddressableChange()
    {
        if (addressable)
        {
            active = false;
            assetBundle = false;
        }
    }
    private void OnAssetBundleChange()
    {
        if (assetBundle)
        {
            active = false;
            addressable = false;
        }
    }

}

public class SceneManagerWindow : OdinEditorWindow
{
    private string SceneLoadingSettingTemplate;
    private string SceneNameTemplate;
    private int max = 9999;


    [Title("Scene In Build"), TableList(CellPadding = 5)]
    public List<SceneObjectData> m_SceneAssets;

    [MenuItem("Tools/SceneManager")]
    static void OpenWindow()
    {
        // Get existing open window or if none, make a new one:
        GetWindow<SceneManagerWindow>().Show();
        GetWindow<SceneManagerWindow>().Init();
    }

    public void Init()
    {
        var list = EditorBuildSettings.scenes.ToList();
        m_SceneAssets = new List<SceneObjectData>();
        LoadTemplate();
        int index = 0;
        for (int i = 0; i < list.Count; i++)
        {
            var e = list[i];
            var scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(e.path);
            if (scene == null)
                continue;
            var setting = SceneSetting.Default;
            if (SceneLoadSetting.settings.ContainsKey(index))
            {
                setting = SceneLoadSetting.settings[index];
            }
            else
            {
                var cindex = GetIndexAssetData(max, i);
                if (SceneLoadSetting.settings.ContainsKey(cindex))
                    setting = SceneLoadSetting.settings[cindex];
            }
            m_SceneAssets.Add(new SceneObjectData()
            {
                asset = scene,
                active = e.enabled,
                activeLoading = setting.SceneLoading,
                mode = setting.Mode,
                addressable = setting.Addressable,
                assetName = scene.name,
                assetBundle = setting.AssetBundle
            });
            index++;
        }
    }

    private void LoadTemplate()
    {
        var path2 = "Assets/Common/SceneManager/Editor/tmp_sceneLoadingSetting.txt";
        var path3 = "Assets/Common/SceneManager/Editor/tmp_sceneName.txt";
        var tmp_sceneLoadSetting = (TextAsset)AssetDatabase.LoadAssetAtPath(path2, typeof(TextAsset));
        var tmp_SceneName = (TextAsset)AssetDatabase.LoadAssetAtPath(path3, typeof(TextAsset));
        SceneLoadingSettingTemplate = tmp_sceneLoadSetting.text;
        SceneNameTemplate = tmp_SceneName.text;
    }

    [Button("Apply")]
    [Title("Change Build Settings window Scene list and enum SceneName.cs")]
    public void Genarate()
    {
        if (m_SceneAssets.Count > 0)
        {
            GenarateEnum();
            GenarateSceneBuid();
        }
    }

    private void GenarateSceneBuid()
    {
        List<EditorBuildSettingsScene> editorBuildSettingsScenes = new List<EditorBuildSettingsScene>();
        foreach (var sceneAsset in m_SceneAssets)
        {
            if (sceneAsset.asset == null)
                continue;
            string scenePath = AssetDatabase.GetAssetPath(sceneAsset.asset);
            if (!string.IsNullOrEmpty(scenePath))
                editorBuildSettingsScenes.Add(new EditorBuildSettingsScene(scenePath, sceneAsset.active));
        }

        // Set the Build Settings window Scene list
        EditorBuildSettings.scenes = editorBuildSettingsScenes.ToArray();
    }

    private void GenarateEnum()
    {
        var strSceneName = SceneNameTemplate;
        var strSceneSetting = SceneLoadingSettingTemplate;
        int index = 0;
        var caddress = "";
        var cscene = "";
        var csetting = "";
        for (int i = 0; i < m_SceneAssets.Count; i++)
        {
            if (m_SceneAssets[i].asset == null)
            {
                continue;
            }
            if (!m_SceneAssets[i].active)
            {
                int cindex = GetIndexAssetData(max, i);
                cscene += GetDataStringOfSceneName(cindex, i);
                csetting += GetDataStringOfSceneSetting(cindex, i);
            }
            else
            {
                cscene += GetDataStringOfSceneName(index, i);
                csetting += GetDataStringOfSceneSetting(index, i);
                index++;
            }
        }
        strSceneName = strSceneName.Replace("@", cscene);
        strSceneSetting = strSceneSetting.Replace("@", csetting);
        //strSceneName += footerSceneName;
        //strSceneSetting += footerSceneLoadingSetting;
        //strAddressable += footerAddressableName;
        SaveScripts(strSceneName, "SceneName");
        SaveScripts(strSceneSetting, "SceneLoadingSetting");
        AssetDatabase.Refresh();
    }

    private static int GetIndexAssetData(int max, int i)
    {
        return i - max;
    }

    private void SaveScripts(string data, string filename)
    {
        string path = Application.dataPath + $"/Common/SceneManager/{filename}.cs";
        File.WriteAllText(path, data.ToString());
    }

    private string GetDataStringOfSceneSetting(int index, int i)
    {
        var result = "\t\t";
        result += $"{{{index}," + "new SceneSetting(" +
            $"{m_SceneAssets[i].active.ToString().ToLower()}, " +
            $"{m_SceneAssets[i].activeLoading.ToString().ToLower()}, " +
            $"{nameof(LoadSceneMode)}.{m_SceneAssets[i].mode.ToString()}, " +
            $"{m_SceneAssets[i].addressable.ToString().ToLower()}," +
            $"{m_SceneAssets[i].assetBundle.ToString().ToLower()}," +
            $"\"{m_SceneAssets[i].assetName}\"" +
            $")}}";
        if (index < m_SceneAssets.Count - 1)
        {
            result += objectPattern;
        }
        return result;
    }

    private string GetDataStringOfAddressableName(int index, int i)
    {
        var result = "\t\t";
        result += $"{{{index},\"{m_SceneAssets[i].assetName}\"}}";
        if (index < m_SceneAssets.Count - 1)
        {
            result += objectPattern;
        }
        return result;
    }

    private string GetDataStringOfSceneName(int index, int i)
    {
        var result = "\t";
        var name = m_SceneAssets[i].asset.name.ToString().Replace(" ", "_");
        name = name.Replace("-", "_");
        name = name.Replace(".", "_");
        name = name.ToUpper();
        result += name + " = " + index;
        if (index < m_SceneAssets.Count - 1)
        {
            result += objectPattern;
        }
        return result;
    }
    const string objectPattern = ",\n";

    static string tmp_sceneLoadingSetting = null;
    static string tmp_sceneName = null;
}
