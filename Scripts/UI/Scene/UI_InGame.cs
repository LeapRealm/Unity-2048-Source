using CodeStage.AntiCheat.ObscuredTypes;
using System.Collections.Generic;
using CodeStage.AntiCheat.Storage;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using static Interpolate;
using static Define;

public enum State { WaitForInput = 0, Process, Ended }

public class UI_InGame : UI_Scene
{
	#region UI Enums
	private enum RectTransforms
	{
		Nodes,
		Blocks,
	}

	private enum Texts
	{
		CurrentScoreText,
		CurrentScoreNum,
		BestScoreText,
		BestScoreNum,
	}

	private enum GridLayoutGroups
	{
		Nodes,
	}
	
	private enum Buttons
	{
		HomeButton,
		RestartButton,
		SoundButton,
		SwipeRect,
	}
	
	private enum Images
	{
		HomeBackground,
		RestartBackground,
		SoundBackground,
		HomeIcon,
		RestartIcon,
		SoundIcon,
	}
	#endregion

	#region UI Variables
	private RectTransform nodeParentRect;
	private RectTransform blockParentRect;
	
	private TextMeshProUGUI currentScoreText;
	private TextMeshProUGUI currentScoreNum;
	private TextMeshProUGUI bestScoreText;
	public TextMeshProUGUI bestScoreNum;
	
	private GridLayoutGroup gridLayoutGroup;
	
	private Button homeButton;
	private Button restartButton;
	private Button soundButton;
	private Button swipeRect;
	
	private Image homeBackground;
	private Image restartBackground;
	private Image soundBackground;
	private Image homeIcon;
	private Image restartIcon;
	private Image soundIcon;
	
	private Vector3 arrowTargetScale;
	private Vector3 buttonTargetScale;
	private Vector3 swipeTargetScale;
	
	private Color tintColor;
	private Color homeBackupColor;
	private Color restartBackupColor;
	private Color soundBackupColor;
	
	private float animDuration;
	#endregion

	#region InGame Variables
	private GameObject nodePrefab;
	private GameObject blockPrefab;

	public List<UI_Node> nodeList;
	private List<UI_Block> blockList;
	
	public Vector2Int blockCnt;

	private State state = State.WaitForInput;

	private float blockSize;
	private float spacing;

	private float swipeDistance;
	private Vector3 touchStart, touchEnd;
	
	private ObscuredLong currentScore;
	public long CurrentScore
	{
		get => currentScore;
		set
		{
			currentScore = value;
			currentScoreNum.text = value.ToString();
		}
	}

	private ObscuredLong bestScore;
	public long BestScore
	{
		get => bestScore;
		set
		{
			bestScore = value;
			bestScoreNum.text = value.ToString();
		}
	}
	#endregion

	#region Init Functions
	public override void OnAwake()
	{
		base.OnAwake();

		#region UI Bind
		Bind<RectTransform>(typeof(RectTransforms));
		nodeParentRect = Get<RectTransform>((int)RectTransforms.Nodes);
		blockParentRect = Get<RectTransform>((int)RectTransforms.Blocks);

		BindText(typeof(Texts));
		currentScoreText = GetText((int)Texts.CurrentScoreText);
		currentScoreNum = GetText((int)Texts.CurrentScoreNum);
		bestScoreText = GetText((int)Texts.BestScoreText);
		bestScoreNum = GetText((int)Texts.BestScoreNum);
		
		Bind<GridLayoutGroup>((typeof(GridLayoutGroups)));
		gridLayoutGroup = Get<GridLayoutGroup>((int)GridLayoutGroups.Nodes);
		
		BindButton(typeof(Buttons));
		homeButton = GetButton((int)Buttons.HomeButton);
		restartButton = GetButton((int)Buttons.RestartButton);
		soundButton = GetButton((int)Buttons.SoundButton);
		swipeRect = GetButton((int)Buttons.SwipeRect);
		
		BindImage(typeof(Images));
		homeBackground = GetImage((int)Images.HomeBackground);
		restartBackground = GetImage((int)Images.RestartBackground);
		soundBackground = GetImage((int)Images.SoundBackground);
		homeIcon = GetImage((int)Images.HomeIcon);
		restartIcon = GetImage((int)Images.RestartIcon);
		soundIcon = GetImage((int)Images.SoundIcon);
		#endregion

		#region UI BindEvent
		// HomeButton
		homeButton.gameObject.BindEvent(OnClickHome);
		homeButton.gameObject.BindEvent(() =>
		{
			homeIcon.color = tintColor;
			homeBackground.color = homeBackupColor * tintColor;
			homeBackground.transform.Zoom(Ease(EaseType.EaseInQuad), Vector3.one, buttonTargetScale, animDuration);
		}, UIEvent.PointerDown);
		homeButton.gameObject.BindEvent(() =>
		{
			homeIcon.color = Color.white;
			homeBackground.color = homeBackupColor;
			homeBackground.transform.Zoom(Ease(EaseType.EaseInQuad), buttonTargetScale, Vector3.one, animDuration);
		}, UIEvent.PointerUp);
		
		// RestartButton
		restartButton.gameObject.BindEvent(OnClickRestart);
		restartButton.gameObject.BindEvent(() =>
		{
			restartIcon.color = tintColor;
			restartBackground.color = restartBackupColor * tintColor;
			restartBackground.transform.Zoom(Ease(EaseType.EaseInQuad), Vector3.one, buttonTargetScale, animDuration);
		}, UIEvent.PointerDown);
		restartButton.gameObject.BindEvent(() =>
		{
			restartIcon.color = Color.white;
			restartBackground.color = restartBackupColor;
			restartBackground.transform.Zoom(Ease(EaseType.EaseInQuad), buttonTargetScale, Vector3.one, animDuration);
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
		
		swipeRect.gameObject.BindEvent(OnPointerDownSwipe, UIEvent.PointerDown);
		swipeRect.gameObject.BindEvent(OnPointerUpSwipe, UIEvent.PointerUp);
		#endregion
		
		currentScoreText.text = Managers.Data.GetText(TextID.Score);
		bestScoreText.text = Managers.Data.GetText(TextID.Best);

		tintColor = new Color(0.78f, 0.78f, 0.78f, 1f);
		homeBackupColor = homeBackground.color;
		restartBackupColor = restartBackground.color;
		soundBackupColor = soundBackground.color;
		
		animDuration = 0.15f;
		buttonTargetScale = Vector3.one * 0.9f;
		
		Sprite sprite;
		bool isMute = Managers.Sound.isMute;
		if (isMute) sprite = Managers.Resource.Load<Sprite>("sprites/SoundOff");
		else        sprite = Managers.Resource.Load<Sprite>("sprites/SoundOn");
		soundIcon.sprite = sprite;

		int count = ObscuredPrefs.Get("BlockCount", 3);
		blockCnt = new Vector2Int(count, count);

		spacing = 16.0f;
		blockSize = (900.0f - (blockCnt.x + 1) * spacing) / blockCnt.x;
		
		CurrentScore = 0;
		swipeDistance = 50.0f;

#if UNITY_ANDROID
		string id = string.Empty;
		switch (blockCnt.x)
		{
			case 3: id = GPGSIds.leaderboard_3x3; break;
			case 4: id = GPGSIds.leaderboard_4x4; break;
			case 5: id = GPGSIds.leaderboard_5x5; break;
			case 6: id = GPGSIds.leaderboard_6x6; break;
		}

		BestScore = 0;
		Managers.GS.UpdateBestScore(id);
#endif

		nodePrefab = Managers.Resource.Load<GameObject>("UI/SubItem/Node");
		Managers.Pool.Create(nodePrefab, blockCnt.x * blockCnt.y);
		
		blockPrefab = Managers.Resource.Load<GameObject>("UI/SubItem/Block");
		Managers.Pool.Create(blockPrefab, blockCnt.x * blockCnt.y);
		
		nodeList = SpawnNodes();
		blockList = new List<UI_Block>();
	}

	public override void OnStart()
	{
		base.OnStart();

		LayoutRebuilder.ForceRebuildLayoutImmediate(nodeParentRect);
		foreach (UI_Node node in nodeList)
			node.localPosition = node.transform.localPosition;
		
		SpawnBlockToRandomNode();
		SpawnBlockToRandomNode();
	}
	#endregion

	#region Input & One-Time Process Functions
	public void OnPointerDownSwipe()
	{
		if (state != State.WaitForInput)
			return;
		
		touchStart = Input.mousePosition;
	}

	public void OnPointerUpSwipe()
	{
		if (state != State.WaitForInput)
			return;
		
		touchEnd = Input.mousePosition;

		if ((touchStart - touchEnd).magnitude < swipeDistance)
			return;
		
		Direction direction = Direction.None;
		
		float deltaX = touchEnd.x - touchStart.x;
		float deltaY = touchEnd.y - touchStart.y;
		
		if (Mathf.Abs(deltaX) > Mathf.Abs(deltaY))
		{
			if (Mathf.Sign(deltaX) > 0) direction = Direction.Right;
			else                        direction = Direction.Left;
		}
		else
		{
			if (Mathf.Sign(deltaY) > 0) direction = Direction.Up;
			else                        direction = Direction.Down;
		}
		
		AllBlocksProcess(direction);
	}

	private void AllBlocksProcess(Direction direction)
	{
		if (direction == Direction.Right)
		{
			for (int y = 0; y < blockCnt.y; y++)
			{
				for (int x = (blockCnt.x - 2); x > -1; x--)
				{
					BlockProcess(nodeList[y * blockCnt.x + x], direction);
				}
			}
		}
		else if (direction == Direction.Down)
		{
			for (int y = (blockCnt.y - 2); y > -1; y--)
			{
				for (int x = 0; x < blockCnt.x; x++)
				{
					BlockProcess(nodeList[y * blockCnt.x + x], direction);
				}
			}
		}
		else if (direction == Direction.Left)
		{
			for (int y = 0; y < blockCnt.y; y++)
			{
				for (int x = 1; x < blockCnt.x; x++)
				{
					BlockProcess(nodeList[y * blockCnt.x + x], direction);
				}
			}
		}
		else if (direction == Direction.Up)
		{
			for (int y = 1; y < blockCnt.y; y++)
			{
				for (int x = 0; x < blockCnt.x; x++)
				{
					BlockProcess(nodeList[y * blockCnt.x + x], direction);
				}
			}
		}

		bool isAllTargetNull = true;
		foreach (UI_Block block in blockList)
		{
			if (block.target != null)
			{
				state = State.Process;
				block.StartMove();
				isAllTargetNull = false;
			}
		}
		
		if (isAllTargetNull)
			Managers.Sound.Play(SoundType.SFX, "Collision");
	}

	private void BlockProcess(UI_Node node, Direction direction)
	{
		if (node.placedBlock == null)
			return;

		UI_Node target = node.FindTarget(node, direction);
		if (target != null)
		{
			if (node.placedBlock != null && target.placedBlock != null)
			{
				if (node.placedBlock.Num == target.placedBlock.Num)
					Combine(node, target);
					
			}
			else if (target.placedBlock == null)
			{
				Move(node, target);
			}
		}
	}
	
	private void Combine(UI_Node from, UI_Node to)
	{
		from.placedBlock.CombineToNode(to);
		from.placedBlock = null;
		to.combined = true;
	}
	
	private void Move(UI_Node from, UI_Node to)
	{
		from.placedBlock.MoveToNode(to);

		if (from.placedBlock != null)
		{
			to.placedBlock = from.placedBlock;
			from.placedBlock = null;
		}
	}
	#endregion

	#region Update Process Functions
	public override void OnUpdate()
	{
		base.OnUpdate();

		switch (state)
		{
			case State.Process:
			{
				UpdateProcess();
				break;
			}
		}
	}
	
	private void UpdateProcess()
	{
		bool targetAllNull = true;

		foreach (UI_Block block in blockList)
		{
			if (block.target != null)
			{
				targetAllNull = false;
				break;
			}
		}

		if (targetAllNull == false)
			return;
				
		List<UI_Block> removeBlocks = new List<UI_Block>();
		foreach (UI_Block block in blockList)
		{
			if (block.needDestroy)
				removeBlocks.Add(block);

#if UNITY_ANDROID
			switch (block.Num)
			{
				case 2048:
				{
					Managers.GS.UnlockAchievement(GPGSIds.achievement_make_2048);
					OnGameClear();
					return;
				}
			}
#endif
		}

		removeBlocks.ForEach(block =>
		{
			CurrentScore += block.Num * 2;
			blockList.Remove(block);
			Managers.Resource.Destroy(block.gameObject);
		});

		nodeList.ForEach(node => node.combined = false);
		
		if (SpawnBlockToRandomNode())
		{
			OnGameOver();
			return;
		}
		
		state = State.WaitForInput;
	}
	#endregion

	#region Spawn Functions
	public List<UI_Node> SpawnNodes()
	{
		gridLayoutGroup.cellSize = new Vector2(blockSize, blockSize);
		gridLayoutGroup.spacing = new Vector2(spacing, spacing);
		List<UI_Node> nodes = new List<UI_Node>(blockCnt.x * blockCnt.y);
		
		for (int y = 0; y < blockCnt.y; y++)
		{
			for (int x = 0; x < blockCnt.x; x++)
			{
				GameObject clone = Managers.Resource.Instantiate(nodePrefab);
				clone.transform.SetParent(nodeParentRect, false);
				clone.GetComponent<RectTransform>().sizeDelta = new Vector2(blockSize, blockSize);
				
				Vector2Int point = new Vector2Int(x, y);

				Vector2Int?[] neighborNodes = new Vector2Int?[4];
				
				Vector2Int right = point + Vector2Int.right;
				Vector2Int down  = point + Vector2Int.up;
				Vector2Int left  = point + Vector2Int.left;
				Vector2Int up    = point + Vector2Int.down;

				if (IsValid(right)) neighborNodes[0] = right;
				if (IsValid(down))  neighborNodes[1] = down;
				if (IsValid(left))  neighborNodes[2] = left;
				if (IsValid(up))    neighborNodes[3] = up;

				UI_Node node = clone.GetComponent<UI_Node>();
				node.Setup(this, neighborNodes, point);
				
				nodes.Add(node);
			}
		}

		return nodes;
	}
	
	private bool IsValid(Vector2Int point)
	{
		return !(point.x < 0 || point.x > (blockCnt.x - 1) || 
		         point.y < 0 || point.y > (blockCnt.y - 1));
	}
	
	private bool SpawnBlockToRandomNode()
	{
		List<UI_Node> emptyNodes = nodeList.FindAll(node => node.placedBlock == null);
		
		int index = Random.Range(0, emptyNodes.Count);
		Vector2Int point = emptyNodes[index].point;
		SpawnBlock(point.x, point.y);

		if (emptyNodes.Count == 1)
			return IsGameOver();
		else
			return false;
	}

	private void SpawnBlock(int x, int y)
	{
		if (nodeList[y * blockCnt.x + x].placedBlock != null) 
			return;

		GameObject clone = Managers.Resource.Instantiate(blockPrefab);
		clone.transform.SetParent(blockParentRect, false);
		
		UI_Block block = clone.GetComponent<UI_Block>();
		UI_Node node = nodeList[y * blockCnt.x + x];

		RectTransform rt = clone.GetComponent<RectTransform>();
		rt.sizeDelta = new Vector2(blockSize, blockSize);
		rt.localPosition = node.localPosition;
		
		block.Setup();
		node.placedBlock = block;
		
		blockList.Add(block);
	}
	#endregion

	#region EndGame Functions
	private bool IsGameOver()
	{
		foreach (UI_Node node in nodeList)
		{
			for (int i = 0; i < 2; i++)
			{
				if (node.neighborNodes[i] == null)
					continue;

				Vector2Int p = node.neighborNodes[i].Value;
				UI_Node neighborNode = nodeList[p.y * blockCnt.x + p.x];

				if (node.placedBlock.Num == neighborNode.placedBlock.Num)
					return false;
			}
		}
		
		return true;
	}

	private void OnGameOver()
	{
		Managers.Sound.Play(SoundType.SFX, "Popup");
		SendCurrScore();
		UnlockAchievement();
		UI_EndGame endGame = Managers.UI.ShowPopupUI<UI_EndGame>();
		endGame.gameOverText.text = Managers.Data.GetText(TextID.GameOver);
		
		state = State.Ended;
	}

	private void OnGameClear()
	{
		Managers.Sound.Play(SoundType.SFX, "Match_2048");
		SendCurrScore();
		UnlockAchievement();
		UI_EndGame endGame = Managers.UI.ShowPopupUI<UI_EndGame>();
		endGame.gameOverText.text = Managers.Data.GetText(TextID.GameClear);
		
		state = State.Ended;
	}
	#endregion

	#region Game Service Functions
	private void SendCurrScore()
	{
#if UNITY_ANDROID
		string id = string.Empty;
		switch (blockCnt.x)
		{
			case 3: id = GPGSIds.leaderboard_3x3; break;
			case 4: id = GPGSIds.leaderboard_4x4; break;
			case 5: id = GPGSIds.leaderboard_5x5; break;
			case 6: id = GPGSIds.leaderboard_6x6; break;
		}
		Managers.GS.SendToLeaderboard(CurrentScore, id);
#endif
	}
	
	private void UnlockAchievement()
	{
#if UNITY_ANDROID
		string id = string.Empty;
		switch (blockCnt.x)
		{
			case 3: id = GPGSIds.achievement_play_3x3; break;
			case 4: id = GPGSIds.achievement_play_4x4; break;
			case 5: id = GPGSIds.achievement_play_5x5; break;
			case 6: id = GPGSIds.achievement_play_6x6; break;
		}
		Managers.GS.UnlockAchievement(id);
#endif
	}
	#endregion

	#region UI Functions
	public void OnClickHome()
	{
		Managers.Sound.Play(SoundType.SFX, "Click");
		StartCoroutine(Utils.Timer(Managers.Sound.GetAudioClipLength("Click") / 3.0f * 2.0f, () =>
		{
			Managers.Ads.ShowInterstitialAd();
			Managers.Scene.ChangeScene(SceneType.MenuScene);
		}));
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
	#endregion
}