using System;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdsManager : IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
{
#if UNITY_ANDROID
	private const string gameID = "4891977";
	private const string interstitialPlacement = "Interstitial_Android";
	private const string rewardedPlacement = "Rewarded_Android";
#elif UNITY_IOS
	private const string GameID = "4891976";
	private const string interstitialPlacement = "Interstitial_iOS";
	private const string rewardedPlacement = "Rewarded_iOS";
#endif

	// TODO: 출시때는 유니티 대시보드에서 오버라이드하기
	private bool testMode = true;
	
	private bool interstitialLoaded = false;
	private bool rewardedLoaded = false;

	private Action rewardedCallback;
	
	public void Init()
	{
		// Advertisement.Initialize(gameID, testMode, this);
	}

	public void LoadInterstitialAd()
	{
		// if (Advertisement.isInitialized)
		// 	Advertisement.Load(interstitialPlacement, this);
	}

	public void ShowInterstitialAd()
	{
		// if (interstitialLoaded)
		// {
		// 	interstitialLoaded = false;
		// 	Advertisement.Show(interstitialPlacement, this);
		// }
		// else
		// 	LoadInterstitialAd();
	}
	
	public void LoadRewardedAd()
	{
		// if (Advertisement.isInitialized)
		// 	Advertisement.Load(rewardedPlacement, this);
	}

	public void ShowRewardedAd(Action rewardedCallback)
	{
		// this.rewardedCallback = rewardedCallback;
		//
		// if (rewardedLoaded)
		// {
		// 	rewardedLoaded = false;
		// 	Advertisement.Show(rewardedPlacement, this);
		// }
		// else
		// {
		// 	LoadRewardedAd();
		// }
	}

	public void Clear()
	{
		// rewardedCallback = null;
	}

	#region Interface Implementations
	public void OnInitializationComplete()
	{
		// LoadInterstitialAd();
		// LoadRewardedAd();
	}
	
	public void OnUnityAdsAdLoaded(string placementId)
	{
		// if (interstitialPlacement.Equals(placementId))
		// 	interstitialLoaded = true;
		// else if (rewardedPlacement.Equals(placementId))
		// 	rewardedLoaded = true;
	}
	
	public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
	{
		// if (interstitialPlacement.Equals(placementId))
		// {
		// 	LoadInterstitialAd();
		// }
		// else if (rewardedPlacement.Equals(placementId))
		// {
		// 	LoadRewardedAd();
		// 	
		// 	if (showCompletionState.Equals(UnityAdsCompletionState.COMPLETED))
		// 		rewardedCallback?.Invoke();
		// 	
		// 	rewardedCallback = null;
		// }
	}
	
	public void OnInitializationFailed(UnityAdsInitializationError error, string message)
	{
		Debug.Log($"Init Failed: [{error}]: {message}");
	}
	
	public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
	{
		Debug.Log($"Load Failed: [{error}:{placementId}] {message}");
	}

	public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
	{
		Debug.Log($"OnUnityAdsShowFailure: [{error}]: {message}");
	}

	public void OnUnityAdsShowStart(string placementId)
	{
		Debug.Log($"OnUnityAdsShowStart: {placementId}");
	}

	public void OnUnityAdsShowClick(string placementId)
	{
		Debug.Log($"OnUnityAdsShowClick: {placementId}");
	}
	#endregion
}