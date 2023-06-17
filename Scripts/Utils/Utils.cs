using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public static class Utils
{
	private static Dictionary<float, WaitForSeconds> waitForSeconds = new Dictionary<float, WaitForSeconds>();
	
	public static T ParseEnum<T>(string value, bool ignoreCase = true)
	{
		return (T)Enum.Parse(typeof(T), value, ignoreCase);
	}
	
	public static T GetOrAddComponent<T>(GameObject go) where T : Component
	{
		T component = go.GetComponent<T>();
		if (component == null)
			component = go.AddComponent<T>();
		return component;
	}

	public static GameObject FindChildGameObject(GameObject go, string name = null, bool recursive = false)
	{
		Transform transform = FindChildComponent<Transform>(go, name, recursive);
		if (transform != null)
			return transform.gameObject;
		return null;
	}

	public static T FindChildComponent<T>(GameObject go, string name = null, bool recursive = false) where T : Object
	{
		if (go == null)
			return null;

		if (recursive == false)
		{
			Transform transform = go.transform.Find(name);
			if (transform != null)
				return transform.GetComponent<T>();
		}
		else
		{
			foreach (T component in go.GetComponentsInChildren<T>())
			{
				if (string.IsNullOrEmpty(name) || component.name == name)
					return component;
			}
		}

		return null;
	}
	
	public static float CurrToTargetAngle(float currDegree, float targetDegree, float speed)
	{
		if (currDegree == targetDegree)
			return currDegree;

		float diff = targetDegree - currDegree;
		float ad = Mathf.Abs(diff);
		if (ad > 180)
			ad = Mathf.Abs(ad - 360);
		float r = speed / ad;
		if (r < 1.0f)
		{
			if (diff > 360) diff -= 360;
			if (diff > 180) diff -= 360;
			currDegree = currDegree + diff * r;
		}
		else
		{
			currDegree = targetDegree;
		}

		return currDegree;
	}
	
	public static WaitForSeconds WaitForSeconds(float seconds)
	{
		WaitForSeconds wfs;
		if (!waitForSeconds.TryGetValue(seconds, out wfs))
			waitForSeconds.Add(seconds, wfs = new WaitForSeconds(seconds));
		return wfs;
	}
	
	public static IEnumerator Timer(float duration, Action action)
	{
		yield return WaitForSeconds(duration);
		action?.Invoke();
	}
}