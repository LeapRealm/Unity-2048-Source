using UnityEngine;
using UnityEngine.EventSystems;
using static Interpolate;

public abstract class BaseScene : MonoBehaviour
{
	private void Awake()
	{
		Application.targetFrameRate = Screen.currentResolution.refreshRate;
		
		Managers.Scene.currentScene = this;

		CanvasGroup canvasGroup = Managers.UI.blackUI;
		if (canvasGroup != null)
		{
			canvasGroup.Alpha(Ease(EaseType.Linear), 1f, 0f, 0.15f, _ =>
			{
				Managers.UI.blackUI = null;
				Destroy(canvasGroup.gameObject);
			});
		}
		
		Object obj = FindObjectOfType(typeof(EventSystem));
		if (obj == null)
			Managers.Resource.Instantiate("UI/EventSystem").name = "EventSystem";
	}

	private void Start()
	{
		OnStart();
	}

	private void Update()
	{
		OnUpdate();
	}

	protected virtual void OnStart() { }
	protected virtual void OnUpdate() { }

	public abstract void Restart();
	public abstract void Clear();
}