using LoadSceneMode = UnityEngine.SceneManagement.LoadSceneMode;

public class SceneSetting
{
    public static SceneSetting Default = new SceneSetting(false, true, LoadSceneMode.Single, false, false);
    private bool _active, _sceneLoading, _addressable, _assetBundle;
    private string _assetName;
    private LoadSceneMode _mode;
    public SceneSetting(bool active, bool sceneLoading, LoadSceneMode mode, bool addressable, bool assetBundle = false, string assetName = null)
    {
        this._active = active;
        this._sceneLoading = sceneLoading;
        this._addressable = addressable;
        this._mode = mode;
        this._assetBundle = assetBundle;
        this._assetName = assetName;
    }

    public bool Active => _active;
    public bool SceneLoading => _sceneLoading;
    public bool Addressable => _addressable;
    public LoadSceneMode Mode => _mode;
    public bool AssetBundle => _assetBundle;
    public string AssetName => _assetName;
}