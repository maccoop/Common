using LoadSceneMode = UnityEngine.SceneManagement.LoadSceneMode;

public class SceneSetting
{
    public static SceneSetting Default = new SceneSetting(false, true, LoadSceneMode.Single, false);
    private bool _active, _sceneLoading, _addressable;
    private LoadSceneMode _mode;
    public SceneSetting(bool active, bool sceneLoading, LoadSceneMode mode, bool addressable)
    {
        this._active = active;
        this._sceneLoading = sceneLoading;
        this._addressable = addressable;
        this._mode = mode;
    }

    public bool Active => _active;
    public bool SceneLoading => _sceneLoading;
    public bool Addressable => _addressable;
    public LoadSceneMode Mode => _mode;
}