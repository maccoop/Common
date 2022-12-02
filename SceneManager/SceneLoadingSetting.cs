using LoadSceneMode = UnityEngine.SceneManagement.LoadSceneMode;
using Dictionary = System.Collections.Generic.Dictionary<int, SceneSetting>;
public static class SceneLoadSetting
{
	public static readonly Dictionary settings = new Dictionary()
	{
		{0,new SceneSetting(true, true, LoadSceneMode.Single, false)},
		{-9998,new SceneSetting(false, true, LoadSceneMode.Additive, true)},
		{1,new SceneSetting(true, true, LoadSceneMode.Single, false)},

	};
}