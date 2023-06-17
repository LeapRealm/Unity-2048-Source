using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using static Define;

public abstract class UI_Base : MonoBehaviour
{
	public Dictionary<Type, Object[]> objects = new Dictionary<Type, Object[]>();

	public virtual void OnAwake() { }
	public virtual void OnStart() { }
	public virtual void OnUpdate() { }
	public virtual void RefreshUI() { }

	public void Awake()
	{
		OnAwake();
	}

	public void Start()
	{
		OnStart();
	}

	public void Update()
	{
		OnUpdate();
	}

	public void Bind<T>(Type type) where T : Object
	{
		string[] names = Enum.GetNames(type);
		Object[] objs = new Object[names.Length];
		objects.Add(typeof(T), objs);

		for (int i = 0; i < names.Length; i++)
		{
			if (typeof(T) == typeof(GameObject))
				objs[i] = Utils.FindChildGameObject(gameObject, names[i], true);
			else
				objs[i] = Utils.FindChildComponent<T>(gameObject, names[i], true);

			if (objs[i] == null)
				Debug.LogWarning($"Failed to bind({names[i]})");
		}
	}
	
	public void BindObject(Type type) { Bind<GameObject>(type); }
	public void BindImage(Type type) { Bind<Image>(type); }
	public void BindText(Type type) { Bind<TextMeshProUGUI>(type); }
	public void BindButton(Type type) { Bind<Button>(type); }

	public T Get<T>(int idx) where T : Object
	{
		Object[] objs = null;
		if (objects.TryGetValue(typeof(T), out objs) == false)
			return null;

		return objs[idx] as T;
	}
	
	public GameObject GetGameObject(int idx) { return Get<GameObject>(idx); }
	public Image GetImage(int idx) { return Get<Image>(idx); }
	public TextMeshProUGUI GetText(int idx) { return Get<TextMeshProUGUI>(idx); }
	public Button GetButton(int idx) { return Get<Button>(idx); }

	public static void BindEvent(GameObject go, Action action, UIEvent type = UIEvent.Click)
	{
		UI_EventHandler evt = Utils.GetOrAddComponent<UI_EventHandler>(go);

		switch (type)
		{
			case UIEvent.Click:
				evt.onClickHandler -= action;
				evt.onClickHandler += action;
				break;
			case UIEvent.Pressed:
				evt.onPressedHandler -= action;
				evt.onPressedHandler += action;
				break;
			case UIEvent.PointerDown:
				evt.onPointerDownHandler -= action;
				evt.onPointerDownHandler += action;
				break;
			case UIEvent.PointerUp:
				evt.onPointerUpHandler -= action;
				evt.onPointerUpHandler += action;
				break;
			case UIEvent.Drag:
				evt.onDragHandler -= action;
				evt.onDragHandler += action;
				break;
			case UIEvent.EndDrag:
				evt.onEndDragHandler -= action;
				evt.onEndDragHandler += action;
				break;
			case UIEvent.PointerEnter:
				evt.onPointerEnterHandler -= action;
				evt.onPointerEnterHandler += action;
				break;
			case UIEvent.PointerExit:
				evt.onPointerExitHandler -= action;
				evt.onPointerExitHandler += action;
				break;
		}
	}
}