using LoadSceneMode = UnityEngine.SceneManagement.LoadSceneMode;
using Dictionary = System.Collections.Generic.Dictionary<int, SceneSetting>;
public static class SceneLoadSetting
{
	public static readonly Dictionary settings = new Dictionary()
	{
		{0,new SceneSetting(true, true, LoadSceneMode.Single, false,false,"MiniGame2006")}
	};
}