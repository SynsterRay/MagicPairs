using UnityEngine;
using UnityEngine.UI;
using MagicPairs.Core;

namespace MagicPairs.UI
{
    public class GameOverPanel : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private Text resultText;
        [SerializeField] private Button playAgainButton;
        [SerializeField] private Text playAgainText;
        [SerializeField] private Button menuButton;

        private Ads.AdManager _cachedAdManager;
        private MainMenu _cachedMenu;

        private void OnEnable()
        {
            GameEvents.OnGameStarted += Hide;
            GameEvents.OnGameOver += Show;
        }

        private void OnDisable()
        {
            GameEvents.OnGameStarted -= Hide;
            GameEvents.OnGameOver -= Show;
        }

        private void Start()
        {
            playAgainButton?.onClick.AddListener(OnPlayAgain);
            menuButton?.onClick.AddListener(OnMenu);
            Hide();
        }

        private void Show(int winnerIndex)
        {
            if (MainMenu.IsChallengeMode) return;
            if (panel == null) return;
            panel.SetActive(true);
            if (playAgainText != null)
                playAgainText.text = Localization.Get("playAgain");
            if (menuButton != null)
            {
                var t = menuButton.GetComponentInChildren<Text>();
                if (t != null) t.text = "Menu";
            }
            if (resultText != null)
            {
                resultText.resizeTextForBestFit = true;
                resultText.resizeTextMinSize = 24;
                resultText.resizeTextMaxSize = resultText.fontSize;
                if (winnerIndex == -1)
                    resultText.text = Localization.Get("draw");
                else
                {
                    string name = winnerIndex == 0 ? MainMenu.Player1Name : MainMenu.Player2Name;
                    resultText.text = Localization.Get("wins", name);
                }
            }
        }

        private void Hide()
        {
            if (panel != null) panel.SetActive(false);
        }

        private void OnPlayAgain()
        {
            if (_cachedAdManager == null) _cachedAdManager = FindAnyObjectByType<Ads.AdManager>();
            _cachedAdManager?.TryShowInterstitialBetweenGames();
            GameManager.Instance.StartGame();
        }

        private void OnMenu()
        {
            Hide();
            if (_cachedAdManager == null) _cachedAdManager = FindAnyObjectByType<Ads.AdManager>();
            _cachedAdManager?.TryShowInterstitialAfterGame();
            if (_cachedMenu == null) _cachedMenu = FindAnyObjectByType<MainMenu>();
            _cachedMenu?.ReturnToMenu();
        }
    }
}
