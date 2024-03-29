using UnityEditor;
using System.IO;
using System;
using UnityEngine.UIElements;
using UnityEngine;
using Object = UnityEngine.Object;

public class ModuleFolderPath : EditorWindow
{
    private string moduleName;
    private static TextField inputName;
    private static TextField inputAssembly;
    private static Button button;
    private static Toggle toggle;
    public TextAsset assemplyExample;


    [MenuItem("Assets/Create/Module", priority = -1)]
    public static void Init()
    {
#if UNITY_EDITOR
        ModuleFolderPath window = (ModuleFolderPath)EditorWindow.GetWindow(typeof(ModuleFolderPath));
        window.minSize = new Vector2(300, 100);
        window.maxSize = new Vector2(300, 100);
        window.Show();
#else
        Debug.Log("Plaftform not support");
#endif
    }

    private void CreateGUI()
    {
#if UNITY_EDITOR
        inputName = new TextField("Input name module: ");
        button = new Button(() => CreateModulePath());
        button.text = "Create";
        toggle = new Toggle("Create Assembly: ");
        inputAssembly = new TextField("Input namespace: ");


        rootVisualElement.Add(new Label("Input Module"));
        rootVisualElement.Add(inputName);
        rootVisualElement.Add(toggle);
        rootVisualElement.Add(inputAssembly);
        rootVisualElement.Add(button);
#else
        Debug.Log("Plaftform not support");
#endif
    }

    public void CreateModulePath()
    {
        moduleName = inputName.value;
        var path = GetCurrentPath();
        if (CheckFolderExist(path, moduleName))
        {
            Debug.LogError("Module is exist!");
            return;
        }
        CreatePath(moduleName, path);
        if (toggle.value)
            CreateAssembly(moduleName, path);
        AssetDatabase.Refresh();
        Close();
    }

    private bool CheckFolderExist(string path, string name)
    {
        return AssetDatabase.IsValidFolder(path + "/" + name);
    }

    private void CreateAssembly(string name, string path)
    {
        if (assemplyExample == null)
        {
            Debug.LogError("Example assemply is missing!");
            return;
        }
        var pathAssembly = string.Format("{2}/{0}/{1}/{1}.asmdef", path, name, Application.dataPath.Replace("/Assets", ""));
        Debug.Log(pathAssembly);
        var content = assemplyExample.text;
        content = content.Replace("%name%", name);
        content = content.Replace("%namespace%", inputAssembly.value);
        File.WriteAllText(pathAssembly, content);
    }

    [MenuItem("Assets/Create/Module Script Path", priority = -1)]
    public static void CreateModuleScriptPath()
    {
        var path = GetCurrentPath();
        AssetDatabase.CreateFolder(path, "config");
        AssetDatabase.CreateFolder(path, "controller");
        AssetDatabase.CreateFolder(path, "entity");
        AssetDatabase.CreateFolder(path, "enum");
        AssetDatabase.CreateFolder(path, "model");
        AssetDatabase.CreateFolder(path, "service");
    }

    private static void CreatePath(string name, string path)
    {
        AssetDatabase.CreateFolder(path, name);
        path += "/" + name;
        AssetDatabase.CreateFolder(path, "animations");
        AssetDatabase.CreateFolder(path, "assets");
        AssetDatabase.CreateFolder(path, "scene");
        AssetDatabase.CreateFolder(path, "scripts");

        /// for script path
        //path += "/" + "scripts";
        //AssetDatabase.CreateFolder(path, "config");
        //AssetDatabase.CreateFolder(path, "controller");
        //AssetDatabase.CreateFolder(path, "entity");
        //AssetDatabase.CreateFolder(path, "enum");
        //AssetDatabase.CreateFolder(path, "model");
        //AssetDatabase.CreateFolder(path, "service");
    }

    private static string GetCurrentPath()
    {
#if UNITY_EDITOR
        string clickedAssetGuid = Selection.assetGUIDs[0];
        string obj = AssetDatabase.GUIDToAssetPath(clickedAssetGuid);
        Debug.Log(obj);
        return obj;
#else
        Debug.Log("Plaftform not support");
        return null;
#endif
    }
}
