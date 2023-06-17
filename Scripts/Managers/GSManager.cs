#if UNITY_ANDROID
using GooglePlayGames;
using GooglePlayGames.BasicApi;
#endif

public class GSManager
{
	public void Init()
	{
#if UNITY_ANDROID
		PlayGamesPlatform.Activate();
		PlayGamesPlatform.Instance.Authenticate(status =>
	{
		if (status != SignInStatus.Success)
			PlayGamesPlatform.Instance.ManuallyAuthenticate(null);
	});
#endif
	}

	public void UnlockAchievement(string id)
	{
#if UNITY_ANDROID
		if (PlayGamesPlatform.Instance.IsAuthenticated() == false)
		{
			PlayGamesPlatform.Instance.Authenticate((status =>
			{
				if (status == SignInStatus.Success)
					PlayGamesPlatform.Instance.ReportProgress(id, 100.0f, null);
			}));
		}
		else
		{
			PlayGamesPlatform.Instance.ReportProgress(id, 100.0f, null);
		}
#endif
	}

	public void ShowAchievement()
	{
#if UNITY_ANDROID
		if (PlayGamesPlatform.Instance.IsAuthenticated() == false)
		{
			PlayGamesPlatform.Instance.Authenticate(status =>
			{
				if (status == SignInStatus.Success)
				{
					PlayGamesPlatform.Instance.ShowAchievementsUI();
				}
				else
				{
					PlayGamesPlatform.Instance.ManuallyAuthenticate(inStatus =>
					{
						if (inStatus == SignInStatus.Success)
							PlayGamesPlatform.Instance.ShowAchievementsUI();
					});
				}
			});
		}
		else
		{
			PlayGamesPlatform.Instance.ShowAchievementsUI();
		}
#endif
	}

	public void SendToLeaderboard(long score, string id)
	{
#if UNITY_ANDROID
		if (PlayGamesPlatform.Instance.IsAuthenticated() == false)
		{
			PlayGamesPlatform.Instance.Authenticate((status =>
			{
				if (status == SignInStatus.Success)
					PlayGamesPlatform.Instance.ReportScore(score, id, null);
			}));
		}
		else
		{
			PlayGamesPlatform.Instance.ReportScore(score, id, null);
		}
#endif
	}

	public void ShowLeaderboard()
	{
#if UNITY_ANDROID
		if (PlayGamesPlatform.Instance.IsAuthenticated() == false)
		{
			PlayGamesPlatform.Instance.Authenticate(status =>
			{
				if (status == SignInStatus.Success)
				{
					PlayGamesPlatform.Instance.ShowLeaderboardUI();
				}
				else
				{
					PlayGamesPlatform.Instance.ManuallyAuthenticate(inStatus =>
					{
						if (inStatus == SignInStatus.Success)
							PlayGamesPlatform.Instance.ShowLeaderboardUI();
					});
				}
			});
		}
		else
		{
			PlayGamesPlatform.Instance.ShowLeaderboardUI();
		}
#endif
	}

	public void UpdateBestScore(string id)
	{
#if UNITY_ANDROID
		if (PlayGamesPlatform.Instance.IsAuthenticated() == false)
		{
			PlayGamesPlatform.Instance.Authenticate((status =>
			{
				if (status == SignInStatus.Success)
					GetFromLeaderboard(id);
			}));
		}
		else
		{
			GetFromLeaderboard(id);
		}
#endif
	}
	
	private void GetFromLeaderboard(string id)
	{
#if UNITY_ANDROID
		PlayGamesPlatform.Instance.LoadScores(id, LeaderboardStart.PlayerCentered, 1, LeaderboardCollection.Public, LeaderboardTimeSpan.AllTime,
			data => Managers.UI.SceneUI.GetComponent<UI_InGame>().bestScoreNum.text = $"{data.PlayerScore.value}");
#endif
	}
	
	public void Clear() { }
}