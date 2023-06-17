using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class ResourceManager
{
	private Dictionary<string, Object> objects = new Dictionary<string, Object>();
	
	public void Init() { }

	public T Load<T>(string path) where T : Object
	{
		if (typeof(T) == typeof(GameObject))
			path = string.Format($"Prefabs/{path}");

		if (objects.TryGetValue(path, out Object o))
			return o as T;
		
		Object obj = Resources.Load<T>(path);
		if (obj == null)
		{
			Debug.LogError($"Failed to load resource : {path}");
			return null;
		}
		
		objects.Add(path, obj);
		return obj as T;
	}

	public GameObject Instantiate(string path, Transform parent = null)
	{
		GameObject original = Load<GameObject>(path);
		if (original == null)
		{
			Debug.LogError($"Failed to load prefab : {path}");
			return null;
		}

		return Instantiate(original, parent);
	}
	
	public GameObject Instantiate(GameObject prefab, Transform parent = null)
	{
		if (Managers.Pool.IsPooled(prefab) || prefab.GetComponent<Poolable>() != null)
			return Managers.Pool.Pop(prefab, parent).gameObject;

		GameObject go = Object.Instantiate(prefab, parent);
		go.name = prefab.name;
		return go;
	}

	public void Destroy(GameObject go)
	{
		if (go == null)
			return;

		Poolable poolable = go.GetComponent<Poolable>();
		if (poolable != null)
		{
			Managers.Pool.Push(poolable);
			return;
		}
		
		Object.Destroy(go);
	}

	public void Clear()
	{
		objects.Clear();
	}
}