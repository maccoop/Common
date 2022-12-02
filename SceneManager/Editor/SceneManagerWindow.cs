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
    [ShowIf("addressable"), ReadOnly]
    public string addressableName;


    private void OnChangeAsset()
    {
        if (asset != null)
            addressableName = asset.name;
        else
            addressableName = "";
    }
    private void OnActiveChange()
    {
        if (active)
        {
            addressable = false;
        }
    }
    private void OnAddressableChange()
    {
        if (addressable)
        {
            active = false;
        }
    }

}

public class SceneManagerWindow : OdinEditorWindow
{
    private string AddressableNameTemplate;
    private string SceneLoadingSettingTemplate;
    private string SceneNameTemplate;


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
        foreach (var e in list)
        {
            var scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(e.path);
            var setting = SceneSetting.Default;
            if (SceneLoadSetting.settings.ContainsKey(index))
            {
                setting = SceneLoadSetting.settings[index];
            }
            m_SceneAssets.Add(new SceneObjectData()
            {
                asset = scene,
                active = e.enabled,
                activeLoading = setting.SceneLoading,
                mode = setting.Mode,
                addressable = setting.Addressable,
                addressableName = scene.name
            });
            index++;
        }
    }

    private void LoadTemplate()
    {
        throw new NotImplementedException();
    }

    [Button("Apply")]
    [Tooltip("Change Build Settings window Scene list and enum SceneName.cs")]
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
        var strAddressable = headerAddressableName;
        var strSceneName = headerSceneName;
        var strSceneSetting = headerSceneLoadingSetting;
        int index = 0;
        int max = 9999;
        for (int i = 0; i < m_SceneAssets.Count; i++)
        {
            if (m_SceneAssets[i].asset == null)
            {
                continue;
            }
            if (m_SceneAssets[i].addressable)
            {
                var cindex = i - max;
                strAddressable += GetDataStringOfAddressableName(cindex, i);
            }
            if (!m_SceneAssets[i].active)
            {
                var cindex = i - max;
                strSceneName += GetDataStringOfSceneName(cindex, i);
                strSceneSetting += GetDataStringOfSceneSetting(cindex, i);
            }
            else
            {
                strSceneName += GetDataStringOfSceneName(index, i);
                strSceneSetting += GetDataStringOfSceneSetting(index, i);
                index++;
            }
        }
        strSceneName += footerSceneName;
        strSceneSetting += footerSceneLoadingSetting;
        strAddressable += footerAddressableName;
        SaveScripts(strSceneName, "SceneName");
        SaveScripts(strSceneSetting, "SceneLoadingSetting");
        SaveScripts(strAddressable, "AddressableName");
        AssetDatabase.Refresh();
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
            $"{m_SceneAssets[i].addressable.ToString().ToLower()}" +
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
        result += $"{{{index},\"{m_SceneAssets[i].addressableName}\"}}";
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

    const string headerSceneLoadingSetting = "using LoadSceneMode = UnityEngine.SceneManagement.LoadSceneMode;" +
        "\nusing Dictionary = System.Collections.Generic.Dictionary<int, SceneSetting>;" +
        "\npublic static class SceneLoadSetting" +
        "\n{" +
        "\n\tpublic static readonly Dictionary settings = new Dictionary()" +
        "\n\t{\n";
    const string footerSceneLoadingSetting = "\n\t};\n}";
    const string headerAddressableName = "using LoadSceneMode = UnityEngine.SceneManagement.LoadSceneMode;" +
        "\nusing Dictionary = System.Collections.Generic.Dictionary<int, string>;" +
        "\npublic static class AddressableName" +
        "\n{" +
        "\n\tpublic static readonly Dictionary addressables = new Dictionary()" +
        "\n\t{\n";
    const string footerAddressableName = "\n\t};\n}";
    const string headerSceneName = "public enum SceneName\n{\n";
    const string footerSceneName = "\n}";
    const string objectPattern = ",\n";
}
