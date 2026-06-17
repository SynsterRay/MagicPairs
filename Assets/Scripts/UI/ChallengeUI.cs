using UnityEngine;
using UnityEngine.UI;
using MagicPairs.Core;
using MagicPairs.GameFlow;

namespace MagicPairs.UI
{
    public class ChallengeUI : MonoBehaviour
    {
        [SerializeField] private GameObject scorePanel;
        [SerializeField] private Text scoreText;
        [SerializeField] private Text streakText;
        [SerializeField] private Text levelText;
        [SerializeField] private GameObject levelCompletePanel;
        [SerializeField] private Text levelCompleteText;
        [SerializeField] private Button nextLevelButton;
        [SerializeField] private GameObject challengeOverPanel;
        [SerializeField] private Text finalScoreText;
        [SerializeField] private Button challengeMenuButton;
        [SerializeField] private GameObject leaderboardPanel;
        [SerializeField] private Text leaderboardText;
        [SerializeField] private Button leaderboardBackButton;
        [SerializeField] private Button showLeaderboardButton;
        [SerializeField] private Button secondChanceButton;

        private bool _secondChanceUsed;

        // Power-up UI (created dynamically)
        private GameObject _powerUpPanel;
        private Button _peekBtn;
        private Button _shuffleBtn;
        private Button _freezeBtn;
        private Button _adPowerUpBtn;
        private Text _peekText;
        private Text _shuffleText;
        private Text _freezeText;
        private Text _adPowerUpText;

        // Cached references
        private PowerUpManager _powerUpManager;
        private ChallengeMode _challengeMode;
        private Ads.AdManager _adManager;

        private void Start()
        {
            _powerUpManager = FindAnyObjectByType<PowerUpManager>();
            _challengeMode = FindAnyObjectByType<ChallengeMode>();
            _adManager = FindAnyObjectByType<Ads.AdManager>();

            nextLevelButton?.onClick.AddListener(OnNextLevel);
            challengeMenuButton?.onClick.AddListener(OnReturnToMenu);
            leaderboardBackButton?.onClick.AddListener(() => leaderboardPanel?.SetActive(false));
            showLeaderboardButton?.onClick.AddListener(ShowLeaderboard);
            secondChanceButton?.onClick.AddListener(OnSecondChance);
            CreatePowerUpUI();
            HideAll();
        }

        private void OnEnable()
        {
            ChallengeMode.OnChallengeScoreChanged += UpdateScore;
            ChallengeMode.OnLevelComplete += ShowLevelComplete;
            ChallengeMode.OnChallengeGameOver += ShowChallengeOver;
            GameEvents.OnGameStarted += OnGameStarted;
            PowerUpManager.OnPowerUpsChanged += UpdatePowerUpUI;
        }

        private void OnDisable()
        {
            ChallengeMode.OnChallengeScoreChanged -= UpdateScore;
            ChallengeMode.OnLevelComplete -= ShowLevelComplete;
            ChallengeMode.OnChallengeGameOver -= ShowChallengeOver;
            GameEvents.OnGameStarted -= OnGameStarted;
            PowerUpManager.OnPowerUpsChanged -= UpdatePowerUpUI;
        }

        private void CreatePowerUpUI()
        {
            var canvas = GetComponent<Canvas>();
            if (canvas == null) canvas = GetComponentInParent<Canvas>();
            if (canvas == null) return;

            _powerUpPanel = new GameObject("PowerUpPanel");
            _powerUpPanel.transform.SetParent(canvas.transform, false);
            var rect = _powerUpPanel.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.02f, 0.93f);
            rect.anchorMax = new Vector2(0.86f, 0.99f);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            _peekBtn = CreatePowerUpButton("peek", 0f, 0.23f, _powerUpPanel.transform);
            _shuffleBtn = CreatePowerUpButton("shuffle", 0.25f, 0.48f, _powerUpPanel.transform);
            _freezeBtn = CreatePowerUpButton("freeze", 0.5f, 0.73f, _powerUpPanel.transform);
            _adPowerUpBtn = CreatePowerUpButton("add reward", 0.75f, 1f, _powerUpPanel.transform);

            _peekText = _peekBtn.GetComponentInChildren<Text>();
            _shuffleText = _shuffleBtn.GetComponentInChildren<Text>();
            _freezeText = _freezeBtn.GetComponentInChildren<Text>();
            _adPowerUpText = _adPowerUpBtn.GetComponentInChildren<Text>();

            _peekBtn.onClick.AddListener(OnPeek);
            _shuffleBtn.onClick.AddListener(OnShuffle);
            _freezeBtn.onClick.AddListener(OnFreeze);
            _adPowerUpBtn.onClick.AddListener(OnAdPowerUp);

            _powerUpPanel.SetActive(false);
        }

        private Button CreatePowerUpButton(string iconName, float xMin, float xMax, Transform parent)
        {
            var go = new GameObject(iconName);
            go.transform.SetParent(parent, false);
            var img = go.AddComponent<Image>();
            var sprite = UIIcons.Get(iconName);
            if (sprite != null)
            {
                img.sprite = sprite;
                img.preserveAspect = true;
            }
            img.color = Color.white;
            go.AddComponent<Button>();
            var r = go.GetComponent<RectTransform>();
            r.anchorMin = new Vector2(xMin, 0f);
            r.anchorMax = new Vector2(xMax, 1f);
            r.offsetMin = Vector2.zero;
            r.offsetMax = Vector2.zero;

            // Count text (small badge at bottom)
            var txtObj = new GameObject("Text");
            txtObj.transform.SetParent(go.transform, false);
            var txt = txtObj.AddComponent<Text>();
            txt.text = "";
            txt.fontSize = 24;
            txt.fontStyle = FontStyle.Bold;
            txt.alignment = TextAnchor.LowerCenter;
            txt.color = Color.white;
            txt.font = Resources.Load<Font>("Fonts/FredokaOne-Regular") ?? Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            var tr = txtObj.GetComponent<RectTransform>();
            tr.anchorMin = Vector2.zero;
            tr.anchorMax = Vector2.one;
            tr.offsetMin = Vector2.zero;
            tr.offsetMax = Vector2.zero;

            return go.GetComponent<Button>();
        }

        private void OnPeek()
        {
            _powerUpManager?.UsePeek();
        }

        private void OnShuffle()
        {
            _powerUpManager?.UseShuffle();
        }

        private void OnFreeze()
        {
            _powerUpManager?.UseFreeze();
        }

        private void OnAdPowerUp()
        {
            if (_adManager == null || !_adManager.IsRewardedReady)
            {
                ShowAdNotReadyFeedback();
                return;
            }

            _adManager.ShowRewarded(() =>
            {
                if (_powerUpManager == null) return;
                var type = (PowerUpType)Random.Range(0, 3);
                _powerUpManager.AddPowerUp(type);
                GPGSManager.Instance?.EventRewardedAd();
            }, ShowAdNotReadyFeedback);
        }

        private void ShowAdNotReadyFeedback()
        {
            if (_adPowerUpText != null)
            {
                _adPowerUpText.text = Localization.Get("adNotReady");
                Invoke(nameof(RestoreAdButtonText), 2f);
            }
        }

        private void RestoreAdButtonText()
        {
            if (_adPowerUpText != null)
                _adPowerUpText.text = $"▶ {Localization.Get("adPowerUp")}";
        }

        private void UpdatePowerUpUI()
        {
            if (_powerUpManager == null || _powerUpPanel == null) return;

            if (_peekText != null) _peekText.text = $"{Localization.Get("peek")} ({_powerUpManager.PeekCount})";
            if (_shuffleText != null) _shuffleText.text = $"{Localization.Get("shuffle")} ({_powerUpManager.ShuffleCount})";
            if (_freezeText != null) _freezeText.text = $"{Localization.Get("freeze")} ({_powerUpManager.FreezeCount})";
            if (_adPowerUpText != null) _adPowerUpText.text = $"▶ {Localization.Get("adPowerUp")}";

            if (_peekBtn != null) _peekBtn.interactable = true;
            if (_shuffleBtn != null) _shuffleBtn.interactable = true;
            if (_freezeBtn != null) _freezeBtn.interactable = true;
        }

        private void OnGameStarted()
        {
            if (!MainMenu.IsChallengeMode) { HideAll(); return; }
            if (scorePanel != null) scorePanel.SetActive(true);
            if (levelCompletePanel != null) levelCompletePanel.SetActive(false);
            if (challengeOverPanel != null) challengeOverPanel.SetActive(false);
            if (_powerUpPanel != null) _powerUpPanel.SetActive(true);

            if (nextLevelButton != null)
            {
                var txt = nextLevelButton.GetComponentInChildren<Text>();
                if (txt != null) txt.text = Localization.Get("next");
            }
            if (challengeMenuButton != null)
            {
                var txt = challengeMenuButton.GetComponentInChildren<Text>();
                if (txt != null) txt.text = "Menu";
            }
            if (showLeaderboardButton != null)
            {
                var txt = showLeaderboardButton.GetComponentInChildren<Text>();
                if (txt != null) txt.text = Localization.Get("scores");
            }

            UpdatePowerUpUI();
        }

        private void HideAll()
        {
            if (scorePanel != null) scorePanel.SetActive(false);
            if (levelCompletePanel != null) levelCompletePanel.SetActive(false);
            if (challengeOverPanel != null) challengeOverPanel.SetActive(false);
            if (leaderboardPanel != null) leaderboardPanel.SetActive(false);
            if (_powerUpPanel != null) _powerUpPanel.SetActive(false);
        }

        private void UpdateScore(int score, int streak, int level)
        {
            if (scoreText != null) scoreText.text = score.ToString();
            if (streakText != null)
            {
                if (streak >= 2)
                {
                    streakText.text = $"x{Mathf.Min(streak, 5)}";
                    streakText.color = new Color(0.9f, 0.5f, 0f);
                }
                else
                {
                    streakText.text = "";
                }
            }
            if (levelText != null)
                levelText.text = $"{Localization.Get("level")} {level}";
        }

        private void ShowLevelComplete(int level)
        {
            if (levelCompletePanel != null) levelCompletePanel.SetActive(true);
            if (levelCompleteText != null)
                levelCompleteText.text = Localization.Get("levelComplete", level);
        }

        private void ShowChallengeOver(int finalScore)
        {
            int level = _challengeMode != null ? _challengeMode.CurrentLevel : 1;

            Leaderboard.AddEntry(MainMenu.Player1Name, finalScore, level);
            GPGSManager.Instance?.PostChallengeScore(finalScore);

            if (challengeOverPanel != null) challengeOverPanel.SetActive(true);
            if (finalScoreText != null)
                finalScoreText.text = Localization.Get("challengeOver", finalScore);

            // Show second chance button if rewarded ad is ready and not used yet
            if (secondChanceButton != null)
            {
                bool canShow = !_secondChanceUsed && _adManager != null && _adManager.IsRewardedReady;
                secondChanceButton.gameObject.SetActive(canShow);
            }
        }

        private void OnSecondChance()
        {
            if (_adManager == null) return;

            _adManager.ShowRewarded(() =>
            {
                _secondChanceUsed = true;
                if (challengeOverPanel != null) challengeOverPanel.SetActive(false);
                if (secondChanceButton != null) secondChanceButton.gameObject.SetActive(false);

                // Continue the game — restart current level (not next!)
                _challengeMode?.RestartCurrentLevel();
                GPGSManager.Instance?.EventSecondChance();
                GPGSManager.Instance?.EventRewardedAd();
            }, () =>
            {
                // Ad not ready — hide button, show feedback
                if (secondChanceButton != null) secondChanceButton.gameObject.SetActive(false);
            });
        }

        private void OnNextLevel()
        {
            if (levelCompletePanel != null) levelCompletePanel.SetActive(false);

            // Show interstitial every 3 levels
            if (_challengeMode != null && _challengeMode.CurrentLevel % 3 == 0)
                _adManager?.TryShowInterstitialBetweenGames();

            _challengeMode?.StartNextLevel();
        }

        private void OnReturnToMenu()
        {
            HideAll();
            _adManager?.TryShowInterstitialAfterGame();
            var menu = FindAnyObjectByType<MainMenu>();
            menu?.ReturnToMenu();
        }

        private void ShowLeaderboard()
        {
            if (leaderboardPanel != null) leaderboardPanel.SetActive(true);
            if (leaderboardText == null) return;

            var entries = Leaderboard.Entries;
            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"<size=32><b><color=#D4AF37>{Localization.Get("leaderboard").ToUpper()}</color></b></size>");
            sb.AppendLine();
            for (int i = 0; i < entries.Count; i++)
            {
                var e = entries[i];
                sb.AppendLine($" {i + 1}. {e.playerName} — {e.score} (Lv.{e.level})");
            }
            if (entries.Count == 0)
                sb.AppendLine(Localization.Get("noScores"));
            leaderboardText.text = sb.ToString();
        }
    }
}
