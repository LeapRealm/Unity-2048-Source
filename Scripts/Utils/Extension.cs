using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;
using static Define;

public static class Extension
{
	private static Random rand = new Random(DateTime.Now.Millisecond);
	
	public static T GetOrAddComponent<T>(this GameObject go) where T : Component
	{
		return Utils.GetOrAddComponent<T>(go);
	}

	public static void BindEvent(this GameObject go, Action action, UIEvent type = UIEvent.Click)
	{
		UI_Base.BindEvent(go, action, type);
	}

	public static bool IsValid(this GameObject go)
	{
		return go != null && go.activeSelf;
	}

	public static void Shuffle<T>(this IList<T> list)
	{
		int n = list.Count;
		while (n > 1)
		{
			n--;
			int k = rand.Next(n + 1);
			T value = list[k];
			list[k] = list[n];
			list[n] = value;
		}
	}

	public static T GetRandom<T>(this IList<T> list)
	{
		int index = rand.Next(list.Count);
		return list[index];
	}

	public static void Move(this Transform transform, EaseFunc easeFunc, 
		Vector3 startLocalPos, Vector3 endLocalPos, float duration, OnAnimComplete onAnimComplete = null)
	{
		transform.localPosition = startLocalPos;
		Vector3 distance = endLocalPos - startLocalPos;
		MoveData moveData = new MoveData(transform, easeFunc, startLocalPos, distance, duration, onAnimComplete);
		Managers.Anim.AddMoveData(ref moveData);
	}
	
	public static void Zoom(this Transform transform, EaseFunc easeFunc, 
		Vector3 startLocalScale, Vector3 endLocalScale, float duration, OnAnimComplete onAnimComplete = null)
	{
		transform.localScale = startLocalScale;
		Vector3 amount = endLocalScale - startLocalScale;
		ZoomData zoomData = new ZoomData(transform, easeFunc, startLocalScale, amount, duration, onAnimComplete);
		Managers.Anim.AddZoomData(ref zoomData);
	}

	public static void Alpha(this CanvasGroup group, EaseFunc easeFunc,
		float startAlpha, float endAlpha, float duration, OnAnimComplete onAnimComplete = null)
	{
		group.alpha = startAlpha;
		float amount = endAlpha - startAlpha;
		AlphaData alphaData = new AlphaData(group, easeFunc, startAlpha, amount, duration, onAnimComplete);
		Managers.Anim.AddAlphaData(ref alphaData);
	}
}