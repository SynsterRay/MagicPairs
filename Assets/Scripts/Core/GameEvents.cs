using System;

namespace MagicPairs.Core
{
    public static class GameEvents
    {
        public static event Action OnGameStarted;
        public static event Action<int> OnTurnChanged;           // playerIndex
        public static event Action<int, int> OnPairMatched;      // playerIndex, pairColorIndex
        public static event Action OnPairMismatched;
        public static event Action<int> OnPiotrusFlipped;        // playerIndex who flipped it
        public static event Action<int, int> OnScoreChanged;     // playerIndex, newScore
        public static event Action<int> OnGameOver;              // winnerIndex (-1 = draw)

        public static void FireGameStarted() => OnGameStarted?.Invoke();
        public static void FireTurnChanged(int player) => OnTurnChanged?.Invoke(player);
        public static void FirePairMatched(int player, int colorIdx) => OnPairMatched?.Invoke(player, colorIdx);
        public static void FirePairMismatched() => OnPairMismatched?.Invoke();
        public static void FirePiotrusFlipped(int player) => OnPiotrusFlipped?.Invoke(player);
        public static void FireScoreChanged(int player, int score) => OnScoreChanged?.Invoke(player, score);
        public static void FireGameOver(int winner) => OnGameOver?.Invoke(winner);

        public static void ClearAll()
        {
            OnGameStarted = null;
            OnTurnChanged = null;
            OnPairMatched = null;
            OnPairMismatched = null;
            OnPiotrusFlipped = null;
            OnScoreChanged = null;
            OnGameOver = null;
        }
    }
}
