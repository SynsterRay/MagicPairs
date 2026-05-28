using UnityEngine;
using TMPro;
using MagicPairs.Core;

namespace MagicPairs.UI
{
    public class ScoreDisplay : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI player1ScoreText;
        [SerializeField] private TextMeshProUGUI player2ScoreText;

        private void OnEnable()
        {
            GameEvents.OnGameStarted += ResetDisplay;
            GameEvents.OnScoreChanged += UpdateScore;
        }

        private void OnDisable()
        {
            GameEvents.OnGameStarted -= ResetDisplay;
            GameEvents.OnScoreChanged -= UpdateScore;
        }

        private void ResetDisplay()
        {
            if (player1ScoreText != null) player1ScoreText.text = "Gracz 1: 0";
            if (player2ScoreText != null) player2ScoreText.text = "Gracz 2: 0";
        }

        private void UpdateScore(int playerIndex, int score)
        {
            if (playerIndex == 0 && player1ScoreText != null)
                player1ScoreText.text = $"Gracz 1: {score}";
            else if (playerIndex == 1 && player2ScoreText != null)
                player2ScoreText.text = $"Gracz 2: {score}";
        }
    }
}
