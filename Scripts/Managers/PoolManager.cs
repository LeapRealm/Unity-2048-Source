using System.Collections.Generic;
using UnityEngine;

public class PoolManager
{
	#region Pool
	private class Pool
	{
		public GameObject Original { get; private set; }
		public Transform Root { get; set; }

		private Stack<Poolable> poolStack = new Stack<Poolable>();

		public void Init(GameObject original, int count = 1)
		{
			Original = original;
			Root = new GameObject().transform;
			Root.name = $"{original.name}_Root";

			for (int i = 0; i < count; i++)
				Push(Create());
		}

		private Poolable Create()
		{
			GameObject go = Object.Instantiate(Original);
			go.name = Original.name;
			return go.GetOrAddComponent<Poolable>();
		}

		public void Push(Poolable poolable)
		{
			if (poolable == null)
				return;

			poolable.transform.SetParent(Root);
			poolable.gameObject.SetActive(false);
			poolable.isUsing = false;
			
			poolStack.Push(poolable);
		}

		public Poolable Pop(Transform parent)
		{
			Poolable poolable;

			if (poolStack.Count > 0)
				poolable = poolStack.Pop();
			else
				poolable = Create();
			
			poolable.gameObject.SetActive(true);

			if (parent == null)
				poolable.transform.SetParent(Managers.Scene.currentScene.transform);

			poolable.transform.SetParent(parent);
			poolable.isUsing = true;

			return poolable;
		}
	}
	#endregion

	private Dictionary<string, Pool> pools = new Dictionary<string, Pool>();
	private Transform root;

	public void Init()
	{
		if (root == null)
		{
			root = new GameObject { name = "@Pool_Root" }.transform;
			Object.DontDestroyOnLoad(root);
		}
	}

	public bool IsPooled(GameObject original)
	{
		return pools.ContainsKey(original.name);
	}

	public void Create(string path, int count = 1)
	{
		GameObject original = Managers.Resource.Load<GameObject>(path);
		if (original == null)
			return;
		
		Create(original, count);
	}
	
	public void Create(GameObject original, int count = 1)
	{
		Pool pool = new Pool();
		pool.Init(original, count);
		pool.Root.SetParent(root);
		
		pools.Add(original.name, pool);
	}

	public void Push(Poolable poolable)
	{
		string name = poolable.gameObject.name;
		if (pools.ContainsKey(name) == false)
		{
			Object.Destroy(poolable.gameObject);
			return;
		}
		
		pools[name].Push(poolable);
	}

	public Poolable Pop(string path, Transform parent = null)
	{
		GameObject original = Managers.Resource.Load<GameObject>(path);
		if (original == null)
			return null;
		
		return Pop(original, parent);
	}
	
	public Poolable Pop(GameObject original, Transform parent = null)
	{
		if (pools.ContainsKey(original.name) == false)
			Create(original);

		return pools[original.name].Pop(parent);
	}

	public void Clear()
	{
		foreach (Transform child in root)
			Object.Destroy(child.gameObject);
		pools.Clear();
	}
}