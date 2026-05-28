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
            if (resultText != null)
            {
                resultText.text = winnerIndex == -1
                    ? "Remis!"
                    : $"Wygrywa Gracz {winnerIndex + 1}!";
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
