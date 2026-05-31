using UnityEngine;
using UnityEngine.UI;
using MagicPairs.Core;

namespace MagicPairs.UI
{
    public class ScoreDisplay : MonoBehaviour
    {
        [SerializeField] private Text player1ScoreText;
        [SerializeField] private Text player2ScoreText;

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
            if (UI.MainMenu.IsChallengeMode || UI.MainMenu.IsTimeAttackMode)
            {
                if (player1ScoreText != null) player1ScoreText.transform.parent.gameObject.SetActive(false);
                return;
            }
            if (player1ScoreText != null) player1ScoreText.transform.parent.gameObject.SetActive(true);
            if (player1ScoreText != null)
                player1ScoreText.text = Localization.Get("score", MainMenu.Player1Name, 0);
            if (player2ScoreText != null)
                player2ScoreText.text = Localization.Get("score", MainMenu.Player2Name, 0);
        }

        private void UpdateScore(int playerIndex, int score)
        {
            string name = playerIndex == 0 ? MainMenu.Player1Name : MainMenu.Player2Name;
            if (playerIndex == 0 && player1ScoreText != null)
                player1ScoreText.text = Localization.Get("score", name, score);
            else if (playerIndex == 1 && player2ScoreText != null)
                player2ScoreText.text = Localization.Get("score", name, score);
        }
    }
}
