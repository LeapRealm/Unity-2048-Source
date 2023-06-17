using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Define;
using static Interpolate;

public class UI_EndGame : UI_Popup
{
	private enum CanvasGroups
	{
		Background,
	}
	
	private enum RectTransforms
	{
		Group,
	}
	
	private enum Texts
	{
		GameOverText,
		RestartText,
		MenuText,
	}
	
	private enum Buttons
	{
		RestartButton,
		MenuButton,
	}
	
	private enum Images
	{
		RestartImage,
		MenuImage,
	}

	private CanvasGroup background;
	private RectTransform group;
	
	public TextMeshProUGUI gameOverText;
	private TextMeshProUGUI restartText;
	private TextMeshProUGUI menuText;

	private Button restartButton;
	private Button menuButton;

	private Image restartImage;
	private Image menuImage;

	private Color tintColor;
	private Color imageBackupColor;
	
	private float animDuration;
	private Vector3 buttonTargetScale;
	
	public override void OnAwake()
	{
		base.OnAwake();
		
		Bind<CanvasGroup>(typeof(CanvasGroups));
		background = Get<CanvasGroup>((int)CanvasGroups.Background);
		
		Bind<RectTransform>(typeof(RectTransforms));
		group = Get<RectTransform>((int)RectTransforms.Group);
		
		BindText(typeof(Texts));
		gameOverText = GetText((int)Texts.GameOverText);
		restartText = GetText((int)Texts.RestartText);
		menuText = GetText((int)Texts.MenuText);
		
		BindButton(typeof(Buttons));
		restartButton = GetButton((int)Buttons.RestartButton);
		menuButton = GetButton((int)Buttons.MenuButton);
		
		BindImage(typeof(Images));
		restartImage = GetImage((int)Images.RestartImage);
		menuImage = GetImage((int)Images.MenuImage);
		
		restartText.text = Managers.Data.GetText(TextID.Restart);
		menuText.text = Managers.Data.GetText(TextID.Menu);
		
		tintColor = new Color(0.78f, 0.78f, 0.78f, 1f);
		imageBackupColor = restartImage.color;

		animDuration = 0.15f;
		buttonTargetScale = Vector3.one * 0.9f;

		restartButton.gameObject.BindEvent(OnClickRestart);
		restartButton.gameObject.BindEvent(() =>
		{
			restartText.color = tintColor;
			restartImage.color = imageBackupColor * tintColor;
			restartImage.transform.Zoom(Ease(EaseType.EaseInQuad), Vector3.one, buttonTargetScale, animDuration);
		}, UIEvent.PointerDown);
		restartButton.gameObject.BindEvent(() =>
		{
			restartText.color = Color.white;
			restartImage.color = imageBackupColor;
			restartImage.transform.Zoom(Ease(EaseType.EaseInQuad), buttonTargetScale, Vector3.one, animDuration);
		}, UIEvent.PointerUp);
		
		menuButton.gameObject.BindEvent(OnClickMenu);
		menuButton.gameObject.BindEvent(() =>
		{
			menuText.color = tintColor;
			menuImage.color = imageBackupColor * tintColor;
			menuImage.transform.Zoom(Ease(EaseType.EaseInQuad), Vector3.one, buttonTargetScale, animDuration);
		}, UIEvent.PointerDown);
		menuButton.gameObject.BindEvent(() =>
		{
			menuText.color = Color.white;
			menuImage.color = imageBackupColor;
			menuImage.transform.Zoom(Ease(EaseType.EaseInQuad), buttonTargetScale, Vector3.one, animDuration);
		}, UIEvent.PointerUp);
		
		background.Alpha(Ease(EaseType.EaseInQuad), 0f, 1f, 0.1f);
		group.Zoom(Ease(EaseType.EaseInQuad), Vector3.zero, Vector3.one * 1.2f, 0.25f, _ =>
		{
			group.Zoom(Ease(EaseType.EaseOutQuad), Vector3.one * 1.2f, Vector3.one, 0.15f);
		});
	}

	public void OnClickRestart()
	{
		Managers.Sound.Play(SoundType.SFX, "Click");
		StartCoroutine(Utils.Timer(Managers.Sound.GetAudioClipLength("Click") / 3.0f * 2.0f, () =>
		{
			Managers.Ads.ShowInterstitialAd();
			Managers.Scene.currentScene.Restart();
		}));
	}

	public void OnClickMenu()
	{
		Managers.Sound.Play(SoundType.SFX, "Click");
		StartCoroutine(Utils.Timer(Managers.Sound.GetAudioClipLength("Click") / 3.0f * 2.0f, () =>
		{
			Managers.Ads.ShowInterstitialAd();
			Managers.Scene.ChangeScene(SceneType.MenuScene);
		}));
	}
}