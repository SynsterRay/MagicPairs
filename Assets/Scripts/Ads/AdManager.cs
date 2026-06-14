using UnityEngine;
using GoogleMobileAds.Api;
using MagicPairs.Core;

namespace MagicPairs.Ads
{
    public class AdManager : MonoBehaviour
    {
#if UNITY_ANDROID
        private const string InterstitialId = "ca-app-pub-4975261609017200/2340336848";
        private const string RewardedId = "ca-app-pub-4975261609017200/2467799124";
        private const string BannerId = "ca-app-pub-4975261609017200/4058928414";
        // Test IDs (uncomment for development):
        // private const string InterstitialId = "ca-app-pub-3940256099942544/1033173712";
        // private const string RewardedId = "ca-app-pub-3940256099942544/5224354917";
        // private const string BannerId = "ca-app-pub-3940256099942544/9214589741";
#else
        private const string InterstitialId = "unused";
        private const string RewardedId = "unused";
        private const string BannerId = "unused";
#endif

        private const string GamesPlayedKey = "MagicPairs_GamesPlayed";
        private const int GamesBeforeAd = 2;

        private InterstitialAd _interstitialAd;
        private RewardedAd _rewardedAd;
        private BannerView _bannerView;
        private int _gamesPlayed;

        private System.Action _onRewardEarned;
        private System.Action _onRewardFailed;

        public bool IsRewardedReady => _rewardedAd != null && _rewardedAd.CanShowAd();

        /// <summary>Call after a COMPLETED game. Shows interstitial if due.</summary>
        public void TryShowInterstitialAfterGame()
        {
            _gamesPlayed++;
            PlayerPrefs.SetInt(GamesPlayedKey, _gamesPlayed);
            PlayerPrefs.Save();
            if (_gamesPlayed >= GamesBeforeAd)
            {
                _gamesPlayed = 0;
                PlayerPrefs.SetInt(GamesPlayedKey, 0);
                PlayerPrefs.Save();
                ShowInterstitial();
            }
        }

        // Keep old name for backward compat (calls from existing code)
        public void TryShowInterstitialBetweenGames() => TryShowInterstitialAfterGame();

        private void Start()
        {
            _gamesPlayed = PlayerPrefs.GetInt(GamesPlayedKey, 0);

            MobileAds.Initialize(status =>
            {
                Debug.Log("[AdManager] AdMob initialized");
                LoadInterstitial();
                LoadRewarded();
                ShowBanner();
            });
        }

        // --- Banner ---

        public void ShowBanner()
        {
            if (_bannerView != null) { _bannerView.Show(); return; }

            _bannerView = new BannerView(BannerId, AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth), AdPosition.Bottom);
            _bannerView.LoadAd(new AdRequest());
        }

        public void HideBanner()
        {
            _bannerView?.Hide();
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

        /// <summary>Shows rewarded ad. onReward called on success, onFailed if ad not available.</summary>
        public void ShowRewarded(System.Action onReward, System.Action onFailed = null)
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
            else
            {
                onFailed?.Invoke();
            }
        }

        private void OnDestroy()
        {
            _interstitialAd?.Destroy();
            _rewardedAd?.Destroy();
            _bannerView?.Destroy();
        }
    }
}
