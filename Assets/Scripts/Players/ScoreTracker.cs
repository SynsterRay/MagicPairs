using UnityEngine;
using MagicPairs.Core;

namespace MagicPairs.Players
{
    public class ScoreTracker : MonoBehaviour
    {
        public PlayerData[] Players { get; private set; }

        private void OnEnable()
        {
            GameEvents.OnGameStarted += ResetScores;
            GameEvents.OnScoreChanged += UpdateScore;
        }

        private void OnDisable()
        {
            GameEvents.OnGameStarted -= ResetScores;
            GameEvents.OnScoreChanged -= UpdateScore;
        }

        private void ResetScores()
        {
            Players = new[]
            {
                new PlayerData(0, UI.MainMenu.Player1Name),
                new PlayerData(1, UI.MainMenu.Player2Name)
            };
        }

        private void UpdateScore(int playerIndex, int newScore)
        {
            if (Players == null || playerIndex < 0 || playerIndex >= Players.Length) return;
            Players[playerIndex].score = newScore;
        }
    }
}
