using UnityEngine;
using UnityEngine.SceneManagement;
using static Define;
using static Interpolate;

public class SceneManagerEx
{
	public BaseScene currentScene;

	public void Init() { }
	
	public void ChangeScene(SceneType type, bool fadeInOut = true)
	{
		if (fadeInOut)
		{
			CanvasGroup canvasGroup = Managers.Resource.Instantiate("UI/UI_Black").GetComponent<CanvasGroup>();
			Object.DontDestroyOnLoad(canvasGroup.gameObject);
			Managers.UI.blackUI = canvasGroup;
		
			canvasGroup.Alpha(Ease(EaseType.Linear), 0f, 1f, 0.15f, _ => LoadScene(type));
		}
		else
		{
			LoadScene(type);
		}
	}

	private string GetSceneName(SceneType type)
	{
		string name = System.Enum.GetName(typeof(SceneType), type);
		return name;
	}

	public void Clear()
	{
		currentScene.Clear();
	}

	private void LoadScene(SceneType type)
	{
		Managers.Clear();
		SceneManager.LoadScene(GetSceneName(type));
	}
}