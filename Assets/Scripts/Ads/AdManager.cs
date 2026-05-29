using UnityEngine;
using GoogleMobileAds.Api;
using MagicPairs.Core;

namespace MagicPairs.Ads
{
    public class AdManager : MonoBehaviour
    {
#if UNITY_ANDROID
        private const string InterstitialId = "ca-app-pub-3940256099942544/1033173712"; // TEST ID
        private const string RewardedId = "ca-app-pub-3940256099942544/5224354917"; // TEST ID
        // Real IDs (uncomment before release):
        // private const string InterstitialId = "ca-app-pub-4975261609017200/2340336848";
        // private const string RewardedId = "ca-app-pub-4975261609017200/2467799124";
#else
        private const string InterstitialId = "unused";
        private const string RewardedId = "unused";
#endif

        private InterstitialAd _interstitialAd;
        private RewardedAd _rewardedAd;
        private int _gamesPlayed;
        private const int GamesBeforeAd = 3;

        private System.Action _onRewardEarned;

        public bool IsRewardedReady => _rewardedAd != null && _rewardedAd.CanShowAd();

        private void Start()
        {
            MobileAds.Initialize(status =>
            {
                Debug.Log("[AdManager] AdMob initialized");
                LoadInterstitial();
                LoadRewarded();
            });
        }

        private void OnEnable() => GameEvents.OnGameOver += OnGameOver;
        private void OnDisable() => GameEvents.OnGameOver -= OnGameOver;

        private void OnGameOver(int _)
        {
            _gamesPlayed++;
            if (_gamesPlayed >= GamesBeforeAd)
            {
                ShowInterstitial();
                _gamesPlayed = 0;
            }
        }

        // --- Interstitial ---

        private void LoadInterstitial()
        {
            _interstitialAd?.Destroy();
            _interstitialAd = null;

            InterstitialAd.Load(InterstitialId, new AdRequest(), (ad, error) =>
            {
                if (error != null) { Debug.LogWarning($"[AdManager] Interstitial load failed: {error}"); return; }
                _interstitialAd = ad;
                ad.OnAdFullScreenContentClosed += () => LoadInterstitial();
                ad.OnAdFullScreenContentFailed += (e) => LoadInterstitial();
            });
        }

        private void ShowInterstitial()
        {
            if (_interstitialAd != null && _interstitialAd.CanShowAd())
                _interstitialAd.Show();
            else
                LoadInterstitial();
        }

        // --- Rewarded ---

        private void LoadRewarded()
        {
            _rewardedAd?.Destroy();
            _rewardedAd = null;

            RewardedAd.Load(RewardedId, new AdRequest(), (ad, error) =>
            {
                if (error != null) { Debug.LogWarning($"[AdManager] Rewarded load failed: {error}"); return; }
                _rewardedAd = ad;
                ad.OnAdFullScreenContentClosed += () => LoadRewarded();
                ad.OnAdFullScreenContentFailed += (e) => LoadRewarded();
            });
        }

        public void ShowRewarded(System.Action onReward)
        {
            if (_rewardedAd != null && _rewardedAd.CanShowAd())
            {
                _onRewardEarned = onReward;
                _rewardedAd.Show((reward) =>
                {
                    _onRewardEarned?.Invoke();
                    _onRewardEarned = null;
                });
            }
        }

        private void OnDestroy()
        {
            _interstitialAd?.Destroy();
            _rewardedAd?.Destroy();
        }
    }
}
