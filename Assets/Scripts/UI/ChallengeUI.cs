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

        private void Start()
        {
            nextLevelButton?.onClick.AddListener(OnNextLevel);
            challengeMenuButton?.onClick.AddListener(OnReturnToMenu);
            leaderboardBackButton?.onClick.AddListener(() => leaderboardPanel?.SetActive(false));
            showLeaderboardButton?.onClick.AddListener(ShowLeaderboard);
            secondChanceButton?.onClick.AddListener(OnSecondChance);
            HideAll();
        }

        private void OnEnable()
        {
            ChallengeMode.OnChallengeScoreChanged += UpdateScore;
            ChallengeMode.OnLevelComplete += ShowLevelComplete;
            ChallengeMode.OnChallengeGameOver += ShowChallengeOver;
            GameEvents.OnGameStarted += OnGameStarted;
        }

        private void OnDisable()
        {
            ChallengeMode.OnChallengeScoreChanged -= UpdateScore;
            ChallengeMode.OnLevelComplete -= ShowLevelComplete;
            ChallengeMode.OnChallengeGameOver -= ShowChallengeOver;
            GameEvents.OnGameStarted -= OnGameStarted;
        }

        private void OnGameStarted()
        {
            if (!MainMenu.IsChallengeMode) { HideAll(); return; }
            if (scorePanel != null) scorePanel.SetActive(true);
            if (levelCompletePanel != null) levelCompletePanel.SetActive(false);
            if (challengeOverPanel != null) challengeOverPanel.SetActive(false);

            if (nextLevelButton != null)
                nextLevelButton.GetComponentInChildren<Text>().text = Localization.Get("next");
            if (challengeMenuButton != null)
                challengeMenuButton.GetComponentInChildren<Text>().text = "Menu";
            if (showLeaderboardButton != null)
                showLeaderboardButton.GetComponentInChildren<Text>().text = Localization.Get("scores");
        }

        private void HideAll()
        {
            if (scorePanel != null) scorePanel.SetActive(false);
            if (levelCompletePanel != null) levelCompletePanel.SetActive(false);
            if (challengeOverPanel != null) challengeOverPanel.SetActive(false);
            if (leaderboardPanel != null) leaderboardPanel.SetActive(false);
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
            var challenge = FindAnyObjectByType<ChallengeMode>();
            int level = challenge != null ? challenge.CurrentLevel : 1;

            Leaderboard.AddEntry(MainMenu.Player1Name, finalScore, level);

            if (challengeOverPanel != null) challengeOverPanel.SetActive(true);
            if (finalScoreText != null)
                finalScoreText.text = Localization.Get("challengeOver", finalScore);

            // Show second chance button if rewarded ad is ready and not used yet
            if (secondChanceButton != null)
            {
                var adManager = FindAnyObjectByType<Ads.AdManager>();
                bool canShow = !_secondChanceUsed && adManager != null && adManager.IsRewardedReady;
                secondChanceButton.gameObject.SetActive(canShow);
            }
        }

        private void OnSecondChance()
        {
            var adManager = FindAnyObjectByType<Ads.AdManager>();
            if (adManager == null) return;

            adManager.ShowRewarded(() =>
            {
                _secondChanceUsed = true;
                if (challengeOverPanel != null) challengeOverPanel.SetActive(false);
                if (secondChanceButton != null) secondChanceButton.gameObject.SetActive(false);

                // Continue the game — restart current level
                var challenge = FindAnyObjectByType<ChallengeMode>();
                challenge?.StartNextLevel();
            });
        }

        private void OnNextLevel()
        {
            if (levelCompletePanel != null) levelCompletePanel.SetActive(false);
            var challenge = FindAnyObjectByType<ChallengeMode>();
            challenge?.StartNextLevel();
        }

        private void OnReturnToMenu()
        {
            HideAll();
            var menu = FindAnyObjectByType<MainMenu>();
            menu?.ReturnToMenu();
        }

        private void ShowLeaderboard()
        {
            if (leaderboardPanel != null) leaderboardPanel.SetActive(true);
            if (leaderboardText == null) return;

            var entries = Leaderboard.Entries;
            var sb = new System.Text.StringBuilder();
            sb.AppendLine(Localization.Get("leaderboard").ToUpper());
            for (int i = 0; i < entries.Count; i++)
            {
                var e = entries[i];
                string num = (i + 1).ToString().PadLeft(2);
                sb.AppendLine($"{num}. {e.playerName} — {e.score} (Lv.{e.level})");
            }
            if (entries.Count == 0)
                sb.AppendLine(Localization.Get("noScores"));
            leaderboardText.text = sb.ToString();
        }
    }
}
