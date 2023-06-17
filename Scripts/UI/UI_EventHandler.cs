using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_EventHandler : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, 
	IPointerUpHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
	public Action onClickHandler = null;
	public Action onPressedHandler = null;
	public Action onPointerDownHandler = null;
	public Action onPointerUpHandler = null;
	public Action onDragHandler = null;
	public Action onEndDragHandler = null;
	public Action onPointerEnterHandler = null;
	public Action onPointerExitHandler = null;

	public bool isPressed = false;

	public void Update()
	{
		if (isPressed)
			onPressedHandler?.Invoke();
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		onClickHandler?.Invoke();
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		isPressed = true;
		onPointerDownHandler?.Invoke();
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		isPressed = false;
		onPointerUpHandler?.Invoke();
	}

	public void OnDrag(PointerEventData eventData)
	{
		onDragHandler?.Invoke();
	}
	
	public void OnEndDrag(PointerEventData eventData)
	{
		onEndDragHandler?.Invoke();
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		onPointerEnterHandler?.Invoke();
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		onPointerExitHandler?.Invoke();
	}
}