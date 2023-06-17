using System.Collections;
using CodeStage.AntiCheat.Storage;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Define;
using static Interpolate;

public class UI_Menu : UI_Scene
{
	private enum Texts
	{
		BoardText,
		PlayText,
		VersionText,
	}
	
	private enum Buttons
	{
		LeftButton,
		RightButton,
		PlayButton,
		LeaderboardButton,
		AchievementButton,
		SoundButton,
	}
	
	private enum Images
	{
		LeftImage,
		RightImage,
		PlayBackground,
		LeaderboardBackground,
		AchievementBackground,
		SoundBackground,
		LeaderboardIcon,
		AchievementIcon, 
		SoundIcon,       
	}
	
	private enum Scrollbars
	{
		Scrollbar,
	}
	
	private enum Transforms
	{
		Content,
	}
	
	private enum HLayoutGroup
	{
		Content,
	}
	
	private enum ScrollRects
	{
		ScrollView,
	}

	private TextMeshProUGUI boardText;
	private TextMeshProUGUI playText;
	private TextMeshProUGUI versionText;
	
	private Button leftButton;
	private Button rightButton;
	private Button playButton;
	private Button leaderboardButton;
	private Button achievementButton;
	private Button soundButton;

	private Image leftImage;
	private Image rightImage;
	private Image playBackground;
	private Image leaderboardBackground;
	private Image achievementBackground;
	private Image soundBackground;
	
	private Image leaderboardIcon;
	private Image achievementIcon;
	private Image soundIcon;
	
	private Color leftBackupColor;
	private Color rightBackupColor;
	private Color playBackupColor;
	private Color leaderboardBackupColor;
	private Color achievementBackupColor;
	private Color soundBackupColor;

	private HorizontalLayoutGroup hLayoutGroup;
	private ScrollRect scrollRect;
	private Scrollbar scrollbar;
	private Transform content;
	private float[] scrollPageValues;

	private float swipeTime;
	private float swipeDistance;
	private int currPage;
	private int maxPage;
	private float touchStartX;
	private float touchEndX;
	private bool isAnimPlaying;

	private float animDuration;
	private Color tintColor;
	private Vector3 arrowTargetScale;
	private Vector3 buttonTargetScale;
	private Vector3 boardTargetScale;
	
	public override void OnAwake()
	{
		base.OnAwake();

		BindText(typeof(Texts));
		boardText = GetText((int)Texts.BoardText);
		playText = GetText((int)Texts.PlayText);
		versionText = GetText((int)Texts.VersionText);

		versionText.text = Application.version;
		
		BindButton(typeof(Buttons));
		leftButton = GetButton((int)Buttons.LeftButton);
		rightButton = GetButton((int)Buttons.RightButton);
		playButton = GetButton((int)Buttons.PlayButton);
		leaderboardButton = GetButton((int)Buttons.LeaderboardButton);
		achievementButton = GetButton((int)Buttons.AchievementButton);
		soundButton = GetButton((int)Buttons.SoundButton);

		BindImage(typeof(Images));
		leftImage = GetImage((int)Images.LeftImage);
		rightImage = GetImage((int)Images.RightImage);
		playBackground = GetImage((int)Images.PlayBackground);
		leaderboardBackground = GetImage((int)Images.LeaderboardBackground);
		achievementBackground = GetImage((int)Images.AchievementBackground);
		soundBackground = GetImage((int)Images.SoundBackground);
		leaderboardIcon = GetImage((int)Images.LeaderboardIcon);
		achievementIcon = GetImage((int)Images.AchievementIcon);
		soundIcon = GetImage((int)Images.SoundIcon);

		leftBackupColor = leftImage.color;
		rightBackupColor = rightImage.color;
		playBackupColor = playBackground.color;
		leaderboardBackupColor = leaderboardBackground.color;
		achievementBackupColor = achievementBackground.color;
		soundBackupColor = soundBackground.color;
		
		Bind<Scrollbar>(typeof(Scrollbars));
		scrollbar = Get<Scrollbar>((int)Scrollbars.Scrollbar);

		Bind<Transform>(typeof(Transforms));
		content = Get<Transform>((int)Transforms.Content);

		Bind<HorizontalLayoutGroup>(typeof(HLayoutGroup));
		hLayoutGroup = Get<HorizontalLayoutGroup>((int)HLayoutGroup.Content);

		Bind<ScrollRect>(typeof(ScrollRects));
		scrollRect = Get<ScrollRect>((int)ScrollRects.ScrollView);
		
		int padding = (int)(GetComponent<RectTransform>().rect.width / 2.0f - 300.0f);
		hLayoutGroup.padding = new RectOffset(padding, padding, 0, 0);

		scrollPageValues = new float[content.childCount];
		float valueDistance = 1f / (scrollPageValues.Length - 1f);
		for (int i = 0; i < scrollPageValues.Length; i++)
			scrollPageValues[i] = valueDistance * i;
		scrollbar.value = scrollPageValues[0];
		
		swipeTime = 0.2f;
		swipeDistance = 50.0f;
		currPage = 0;
		maxPage = content.childCount;
		isAnimPlaying = false;
		
		animDuration = 0.15f;
		tintColor = new Color(0.78f, 0.78f, 0.78f, 1f);
		arrowTargetScale = Vector3.one * 0.8f;
		buttonTargetScale = Vector3.one * 0.9f;
		boardTargetScale = Vector3.one * 0.6f;

		playText.text = Managers.Data.GetText(TextID.Play);
		
		// LeftButton
		leftButton.gameObject.BindEvent(OnClickLeft);
		leftButton.gameObject.BindEvent(() =>
		{
			leftImage.color = leftBackupColor * tintColor;
			leftImage.transform.Zoom(Ease(EaseType.EaseInQuad), Vector3.one, arrowTargetScale, animDuration);
		}, UIEvent.PointerDown);
		leftButton.gameObject.BindEvent(() =>
		{
			leftImage.color = leftBackupColor;
			leftImage.transform.Zoom(Ease(EaseType.EaseInQuad), arrowTargetScale, Vector3.one, animDuration);
		}, UIEvent.PointerUp);

		// RightButton
		rightButton.gameObject.BindEvent(OnClickRight);
		rightButton.gameObject.BindEvent(() =>
		{
			rightImage.color = rightBackupColor * tintColor;
			rightImage.transform.Zoom(Ease(EaseType.EaseInQuad), Vector3.one, arrowTargetScale, animDuration);
		}, UIEvent.PointerDown);
		rightButton.gameObject.BindEvent(() =>
		{
			rightImage.color = rightBackupColor;
			rightImage.transform.Zoom(Ease(EaseType.EaseInQuad), arrowTargetScale, Vector3.one, animDuration);
		}, UIEvent.PointerUp);

		// PlayButton
		playButton.gameObject.BindEvent(OnClickPlay);
		playButton.gameObject.BindEvent(() =>
		{
			playText.color = tintColor;
			playBackground.color = playBackupColor * tintColor;
			playBackground.transform.Zoom(Ease(EaseType.EaseInQuad), Vector3.one, buttonTargetScale, animDuration);
		}, UIEvent.PointerDown);
		playButton.gameObject.BindEvent(() =>
		{
			playText.color = Color.white;
			playBackground.color = playBackupColor;
			playBackground.transform.Zoom(Ease(EaseType.EaseInQuad), buttonTargetScale, Vector3.one, animDuration);
		}, UIEvent.PointerUp);
		
		// LeaderboardButton
		leaderboardButton.gameObject.BindEvent(OnClickLeaderboard);
		leaderboardButton.gameObject.BindEvent(() =>
		{
			leaderboardIcon.color = tintColor;
			leaderboardBackground.color = leaderboardBackupColor * tintColor;
			leaderboardBackground.transform.Zoom(Ease(EaseType.EaseInQuad), Vector3.one, buttonTargetScale, animDuration);
		}, UIEvent.PointerDown);
		leaderboardButton.gameObject.BindEvent(() =>
		{
			leaderboardIcon.color = Color.white;
			leaderboardBackground.color = leaderboardBackupColor;
			leaderboardBackground.transform.Zoom(Ease(EaseType.EaseInQuad), buttonTargetScale, Vector3.one, animDuration);
		}, UIEvent.PointerUp);

		// AchievementButton
		achievementButton.gameObject.BindEvent(OnClickAchievement);
		achievementButton.gameObject.BindEvent(() =>
		{
			achievementIcon.color = tintColor;
			achievementBackground.color = achievementBackupColor * tintColor;
			achievementBackground.transform.Zoom(Ease(EaseType.EaseInQuad), Vector3.one, buttonTargetScale, animDuration);
		}, UIEvent.PointerDown);
		achievementButton.gameObject.BindEvent(() =>
		{
			achievementIcon.color = Color.white;
			achievementBackground.color = achievementBackupColor;
			achievementBackground.transform.Zoom(Ease(EaseType.EaseInQuad), buttonTargetScale, Vector3.one, animDuration);
		}, UIEvent.PointerUp);

		// SoundButton
		soundButton.gameObject.BindEvent(OnClickSound);
		soundButton.gameObject.BindEvent(() =>
		{
			soundIcon.color = tintColor;
			soundBackground.color = soundBackupColor * tintColor;
			soundBackground.transform.Zoom(Ease(EaseType.EaseInQuad), Vector3.one, buttonTargetScale, animDuration);
		}, UIEvent.PointerDown);
		soundButton.gameObject.BindEvent(() =>
		{
			soundIcon.color = Color.white;
			soundBackground.color = soundBackupColor;
			soundBackground.transform.Zoom(Ease(EaseType.EaseInQuad), buttonTargetScale, Vector3.one, animDuration);
		}, UIEvent.PointerUp);

		scrollRect.gameObject.BindEvent(OnPointerDownScroll, UIEvent.PointerDown);
		scrollRect.gameObject.BindEvent(OnPointerUpScroll, UIEvent.PointerUp);
		
		Sprite sprite;
		bool isMute = Managers.Sound.isMute;
		if (isMute) sprite = Managers.Resource.Load<Sprite>("sprites/SoundOff");
		else        sprite = Managers.Resource.Load<Sprite>("sprites/SoundOn");
		soundIcon.sprite = sprite;
	}

	public void OnClickLeft()
	{
		if (isAnimPlaying)
			return;

		if (currPage == 0)
		{
			Managers.Sound.Play(SoundType.SFX, "Collision");
			return;
		}

		UpdateSwipe(true);
	}

	public void OnClickRight()
	{
		if (isAnimPlaying)
			return;
		
		if (currPage == maxPage - 1)
		{
			Managers.Sound.Play(SoundType.SFX, "Collision");
			return;
		}
		
		UpdateSwipe(false);
	}
	
	public void OnClickPlay()
	{
		Managers.Sound.Play(SoundType.SFX, "Play");
		ObscuredPrefs.Set("BlockCount", currPage + 3);
		Managers.Scene.ChangeScene(SceneType.GameScene);
	}

	public void OnClickLeaderboard()
	{
		Managers.Sound.Play(SoundType.SFX, "Click");
		StartCoroutine(Utils.Timer(Managers.Sound.GetAudioClipLength("Click") / 2.0f, () =>
		{
			Managers.GS.ShowLeaderboard();
		}));
	}
	
	public void OnClickAchievement()
	{
		Managers.Sound.Play(SoundType.SFX, "Click");
		StartCoroutine(Utils.Timer(Managers.Sound.GetAudioClipLength("Click") / 2.0f, () =>
		{
			Managers.GS.ShowAchievement();
		}));
	}
	
	public void OnClickSound()
	{
		Managers.Sound.ToggleMute();
		Managers.Sound.Play(SoundType.SFX, "Click");
		
		Sprite sprite;
		bool isMute = Managers.Sound.isMute;
		if (isMute) sprite = Managers.Resource.Load<Sprite>("sprites/SoundOff");
		else        sprite = Managers.Resource.Load<Sprite>("sprites/SoundOn");
		soundIcon.sprite = sprite;
	}

	public void OnPointerDownScroll()
	{
		if (isAnimPlaying)
			return;
		
		touchStartX = Input.mousePosition.x;
	}

	public void OnPointerUpScroll()
	{
		if (isAnimPlaying)
			return;
		
		touchEndX = Input.mousePosition.x;
		
		if (Mathf.Abs(touchStartX - touchEndX) < swipeDistance)
			return;
		
		if ((currPage == 0 && touchStartX < touchEndX) || 
		    (currPage == maxPage - 1 && touchStartX > touchEndX))
			return;

		UpdateSwipe(touchStartX < touchEndX);
	}

	private void UpdateSwipe(bool swipeRight)
	{
		Managers.Sound.Play(SoundType.SFX, "Swipe");
		
		if (swipeRight)
		{
			if (currPage == 0)
				return;
			currPage--;
		}
		else
		{
			if (currPage == maxPage - 1)
				return;
			currPage++;
		}

		boardText.text = $"{currPage + 3}x{currPage + 3}";
		StartCoroutine(OnSwipeAnimation(swipeRight));
	}
	
	private IEnumerator OnSwipeAnimation(bool swipeRight)
	{
		float startPos = scrollbar.value;
		float currTime = 0;
		float rate = 0;
		
		int otherPage = swipeRight ? currPage + 1 : currPage - 1;
		Vector3 startCurrPageScale = content.GetChild(currPage).localScale;
		Vector3 startOtherPageScale = content.GetChild(otherPage).localScale;

		isAnimPlaying = true;

		while (rate < 1)
		{
			currTime += Time.deltaTime;
			rate = currTime / swipeTime;

			scrollbar.value = Mathf.Lerp(startPos, scrollPageValues[currPage], rate);
			content.GetChild(currPage).localScale = Vector3.Lerp(startCurrPageScale, Vector3.one, rate);
			content.GetChild(otherPage).localScale = Vector3.Lerp(startOtherPageScale, boardTargetScale, rate);
			
			yield return null;
		}

		isAnimPlaying = false;
	}
}