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
                menuButton.GetComponentInChildren<Text>().text = "Menu";
            if (resultText != null)
            {
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
            var adManager = FindAnyObjectByType<Ads.AdManager>();
            adManager?.TryShowInterstitialBetweenGames();
            GameManager.Instance.StartGame();
        }

        private void OnMenu()
        {
            Hide();
            var adManager = FindAnyObjectByType<Ads.AdManager>();
            adManager?.TryShowInterstitialAfterGame();
            var menu = FindAnyObjectByType<MainMenu>();
            menu?.ReturnToMenu();
        }
    }
}
