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
            Hide();
        }

        private void Show(int winnerIndex)
        {
            if (panel == null) return;
            panel.SetActive(true);
            if (playAgainText != null)
                playAgainText.text = Localization.Get("playAgain");
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
            GameManager.Instance.StartGame();
        }
    }
}
