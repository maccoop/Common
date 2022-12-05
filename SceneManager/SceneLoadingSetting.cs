using LoadSceneMode = UnityEngine.SceneManagement.LoadSceneMode;
using Dictionary = System.Collections.Generic.Dictionary<int, SceneSetting>;
public static class SceneLoadSetting
{
	public static readonly Dictionary settings = new Dictionary()
	{
		{0,new SceneSetting(true, true, LoadSceneMode.Single, false,false,"SampleScene")},
		{1,new SceneSetting(true, true, LoadSceneMode.Additive, false,false,"New Scene")},
		{2,new SceneSetting(true, true, LoadSceneMode.Single, false,false,"New Scene 1")},
		{-9996,new SceneSetting(false, true, LoadSceneMode.Single, false,true,"MiniGame2006")},

	};
}