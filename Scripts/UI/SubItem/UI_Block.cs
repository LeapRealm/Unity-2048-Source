using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Interpolate;
using static Define;

public class UI_Block : UI_SubItem
{
	private enum Texts
	{
		NumText,
	}
	
	private Color[] blockColors =
	{
		new Color(0.93f, 0.89f, 0.85f, 1.0f),
		new Color(0.93f, 0.88f, 0.78f, 1.0f),
		new Color(0.95f, 0.69f, 0.47f, 1.0f),
		new Color(0.96f, 0.58f, 0.39f, 1.0f),
		new Color(0.96f, 0.49f, 0.38f, 1.0f),
		new Color(0.96f, 0.37f, 0.23f, 1.0f),
		new Color(0.93f, 0.81f, 0.45f, 1.0f),
		new Color(0.93f, 0.80f, 0.38f, 1.0f),
		new Color(0.93f, 0.78f, 0.31f, 1.0f),
		new Color(0.93f, 0.77f, 0.25f, 1.0f),
		new Color(0.93f, 0.76f, 0.18f, 1.0f),
	};
	
	private Image blockImage;
	private TextMeshProUGUI blockNumText;
	public UI_Node target;

	private bool combine;
	public bool needDestroy;

	private int num;
	public int Num
	{
		set
		{
			num = value;
			blockNumText.text = value.ToString();
			blockImage.color = blockColors[(int)Mathf.Log(value, 2) - 1];

			if (value == 2 || value == 4) 
				blockNumText.color = new Color(0.47f, 0.43f, 0.40f, 1.0f);
			else 
				blockNumText.color = Color.white;
		}
		get => num;
	}

	public override void OnAwake()
	{
		base.OnAwake();

		blockImage = GetComponent<Image>();
		BindText(typeof(Texts));
		blockNumText = GetText((int)Texts.NumText);
	}

	public void Setup()
	{
		target = null;
		combine = false;
		needDestroy = false;
		
		Num = Random.Range(0, 100) < 90 ? 2 : 4;
		transform.Zoom(Ease(EaseType.EaseInQuad), Vector3.one * 0.5f, Vector3.one, 0.15f);
	}

	public void MoveToNode(UI_Node to)
	{
		target = to;
		combine = false;
	}

	public void CombineToNode(UI_Node to)
	{
		target = to;
		combine = true;
	}

	public void StartMove()
	{
		float moveTime = 0.1f;
		transform.Move(Ease(EaseType.EaseInQuad), transform.localPosition, target.localPosition, moveTime, OnMoveComplete);
	}

	private void OnMoveComplete(GameObject go)
	{
		if (target != null)
		{
			if (combine)
			{
				target.placedBlock.Num *= 2;


				switch (target.placedBlock.Num)
				{
					case 8:
					{
						Managers.Sound.Play(SoundType.SFX, "Match_8");
						break;
					}
					case 16:
					{
						Managers.Sound.Play(SoundType.SFX, "Match_16");
						break;
					}
					case 32:
					{
						Managers.Sound.Play(SoundType.SFX, "Match_32");
						break;
					}
					case 64:
					{
						Managers.Sound.Play(SoundType.SFX, "Match_64");
						break;
					}
					case 128:
					{
						Managers.Sound.Play(SoundType.SFX, "Match_128");
						break;
					}
#if UNITY_ANDROID
					case 256:
					{
						Managers.Sound.Play(SoundType.SFX, "Match_256");
						Managers.GS.UnlockAchievement(GPGSIds.achievement_make_256);
						break;
					}
					case 512:
					{
						Managers.Sound.Play(SoundType.SFX, "Match_512");
						Managers.GS.UnlockAchievement(GPGSIds.achievement_make_512);  
						break;
					}
					case 1024:
					{
						Managers.Sound.Play(SoundType.SFX, "Match_1024");
						Managers.GS.UnlockAchievement(GPGSIds.achievement_make_1024); 
						break;
					}
#endif
				}
				
				target.placedBlock.transform.Zoom(Ease(EaseType.EaseInQuad), Vector3.one, Vector3.one * 1.25f, 0.075f, 
					_ => target.placedBlock.transform.Zoom(Ease(EaseType.EaseInQuad), Vector3.one * 1.25f, Vector3.one, 0.075f, OnZoomComplete));
				gameObject.SetActive(false);
			}
			else
			{
				target = null;
			}
		}
	}

	private void OnZoomComplete(GameObject go)
	{
		target = null;
		needDestroy = true;
	}
}