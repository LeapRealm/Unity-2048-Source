using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIManager
{
	private int order = -20;
	private Stack<UI_Popup> popupStack = new Stack<UI_Popup>();
	private Transform root;
	
	public UI_Scene SceneUI { get; private set; }
	public CanvasGroup blackUI;

	public void Init()
	{
		if (root == null)
		{
			GameObject go = GameObject.Find("@UI_Root");
			if (go == null)
			{
				root = new GameObject { name = "@UI_Root" }.transform;
				Object.DontDestroyOnLoad(root);
			}
			else
			{
				root = go.transform;
			}
		}
	}

	public void SetCanvas(GameObject go, bool sort = true)
	{
		Canvas canvas = Utils.GetOrAddComponent<Canvas>(go);
		canvas.renderMode = RenderMode.ScreenSpaceOverlay;
		canvas.overrideSorting = true;

		if (sort)
		{
			canvas.sortingOrder = order;
			order++;
		}
		else
		{
			canvas.sortingOrder = 0;
		}
	}

	public T MakeSubItem<T>(Transform parent = null, string name = null) where T : UI_Base
	{
		if (string.IsNullOrEmpty(name))
			name = typeof(T).Name;

		GameObject prefab = Managers.Resource.Load<GameObject>($"UI/SubItem/{name}");
		
		GameObject go = Managers.Resource.Instantiate(prefab);
		if (parent != null)
			go.transform.SetParent(parent);
		
		go.transform.localScale = Vector3.one;
		go.transform.localPosition = prefab.transform.position;

		return Utils.GetOrAddComponent<T>(go);
	}

	public T ShowSceneUI<T>(string name = null) where T : UI_Scene
	{
		if (string.IsNullOrEmpty(name))
			name = typeof(T).Name;

		GameObject go = Managers.Resource.Instantiate($"UI/Scene/{name}");
		SetCanvas(go, false);
		go.transform.SetParent(root.transform);
		
		T sceneUI = Utils.GetOrAddComponent<T>(go);
		SceneUI = sceneUI;

		return sceneUI;
	}

	public T ShowPopupUI<T>(string name = null, Transform parent = null) where T : UI_Popup
	{
		if (string.IsNullOrEmpty(name))
			name = typeof(T).Name;

		GameObject prefab = Managers.Resource.Load<GameObject>($"UI/Popup/{name}");
		GameObject go = Managers.Resource.Instantiate(prefab);
		SetCanvas(go);
		T popup = Utils.GetOrAddComponent<T>(go);
		popupStack.Push(popup);
		
		if (parent != null)
			go.transform.SetParent(parent);
		else if (SceneUI != null)
			go.transform.SetParent(SceneUI.transform);
		else
			go.transform.SetParent(root.transform);
		
		go.transform.localScale = Vector3.one;
		go.transform.localPosition = Vector3.zero;

		return popup;
	}

	public T FindPopup<T>() where T : UI_Popup
	{
		return popupStack.Where(x => x.GetType() == typeof(T)).FirstOrDefault() as T;
	}

	public T PeekPopupUI<T>() where T : UI_Popup
	{
		if (popupStack.Count == 0) 
			return null;

		return popupStack.Peek() as T;
	}

	public void ClosePopupUI(UI_Popup popup)
	{
		if (popupStack.Count == 0)
			return;

		if (popupStack.Peek() != popup)
		{
			Debug.LogError("Close Popup Failed!");
			return;
		}
		
		ClosePopupUI();
	}

	public void ClosePopupUI()
	{
		if (popupStack.Count == 0)
			return;

		UI_Popup popup = popupStack.Pop();
		Managers.Resource.Destroy(popup.gameObject);
		popup = null;
		order--;
	}

	public void CloseAllPopupUI()
	{
		while (popupStack.Count > 0)
			ClosePopupUI();
	}

	public void Clear()
	{
		while (popupStack.Count > 0)
		{
			UI_Popup popup = popupStack.Pop();
			Object.Destroy(popup.gameObject);
		}

		if (SceneUI != null)
		{
			Object.Destroy(SceneUI.gameObject);
			SceneUI = null;
		}
	}
}