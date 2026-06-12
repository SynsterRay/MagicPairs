using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

namespace MagicPairs.Core
{
    public class GPGSManager : MonoBehaviour
    {
        public static GPGSManager Instance { get; private set; }
        public bool IsAuthenticated { get; private set; }

        // Leaderboards
        private const string LeaderboardChallengeId = "CgkIxYnu6M4MEAIQAQ";
        private const string LeaderboardTimeAttackId = "CgkIxYnu6M4MEAIQAQ";

        // Achievements — one-time
        private const string AchFirstPair = "CgkIxYnu6M4MEAIQAg";
        private const string AchMemoryMaster = "CgkIxYnu6M4MEAIQAw";
        private const string AchStreakX5 = "CgkIxYnu6M4MEAIQBA";
        private const string AchLevel5 = "CgkIxYnu6M4MEAIQBQ";
        private const string AchLevel10 = "CgkIxYnu6M4MEAIQBg";
        private const string AchSprinter = "CgkIxYnu6M4MEAIQBw";
        private const string AchCollector = "CgkIxYnu6M4MEAIQCA";
        private const string AchPerfectLevel = "CgkIxYnu6M4MEAIQDw";
        private const string AchJokerSurvivor = "CgkIxYnu6M4MEAIQEA";
        private const string AchSpeedDemon = "CgkIxYnu6M4MEAIQEQ";

        // Achievements — incremental
        private const string AchPairHunter = "CgkIxYnu6M4MEAIQCQ";       // 50 pairs
        private const string AchPairMaster = "CgkIxYnu6M4MEAIQCg";       // 200 pairs
        private const string AchMarathonRunner = "CgkIxYnu6M4MEAIQCw";   // 20 games
        private const string AchDedicatedPlayer = "CgkIxYnu6M4MEAIQDA";  // 100 games
        private const string AchLevelClimber = "CgkIxYnu6M4MEAIQDQ";     // 25 levels
        private const string AchWinner = "CgkIxYnu6M4MEAIQDg";           // 10 wins
        private const string AchJokerMagnet = "CgkIxYnu6M4MEAIQEg";      // 20 jokers

        private void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
        }

        private void Start()
        {
            PlayGamesPlatform.Activate();
            SignIn();
        }

        public void SignIn()
        {
            PlayGamesPlatform.Instance.Authenticate(status =>
            {
                IsAuthenticated = status == SignInStatus.Success;
                Debug.Log($"[GPGS] Sign in: {status}");
            });
        }

        // --- Leaderboards ---

        public void PostChallengeScore(int score)
        {
            if (!IsAuthenticated) return;
            Social.ReportScore(score, LeaderboardChallengeId, success =>
                Debug.Log($"[GPGS] Challenge score posted: {success}"));
        }

        public void PostTimeAttackScore(float timeLeft)
        {
            if (!IsAuthenticated) return;
            long ms = (long)(timeLeft * 1000);
            Social.ReportScore(ms, LeaderboardTimeAttackId, success =>
                Debug.Log($"[GPGS] TimeAttack score posted: {success}"));
        }

        public void ShowLeaderboard()
        {
            if (!IsAuthenticated) return;
            PlayGamesPlatform.Instance.ShowLeaderboardUI();
        }

        // --- Achievements: one-time ---

        public void UnlockFirstPair() => Unlock(AchFirstPair);
        public void UnlockMemoryMaster() => Unlock(AchMemoryMaster);
        public void UnlockStreakX5() => Unlock(AchStreakX5);
        public void UnlockLevel5() => Unlock(AchLevel5);
        public void UnlockLevel10() => Unlock(AchLevel10);
        public void UnlockSprinter() => Unlock(AchSprinter);
        public void UnlockCollector() => Unlock(AchCollector);
        public void UnlockPerfectLevel() => Unlock(AchPerfectLevel);
        public void UnlockJokerSurvivor() => Unlock(AchJokerSurvivor);
        public void UnlockSpeedDemon() => Unlock(AchSpeedDemon);

        // --- Achievements: incremental ---

        public void IncrementPairFound()
        {
            Increment(AchPairHunter, 1);
            Increment(AchPairMaster, 1);
        }

        public void IncrementGamePlayed()
        {
            Increment(AchMarathonRunner, 1);
            Increment(AchDedicatedPlayer, 1);
        }

        public void IncrementLevelComplete() => Increment(AchLevelClimber, 1);
        public void IncrementWin() => Increment(AchWinner, 1);
        public void IncrementJokerHit() => Increment(AchJokerMagnet, 1);

        // --- Helpers ---

        private void Unlock(string id)
        {
            if (!IsAuthenticated) return;
            Social.ReportProgress(id, 100.0, success =>
                Debug.Log($"[GPGS] Achievement {id}: {success}"));
        }

        private void Increment(string id, int steps)
        {
            if (!IsAuthenticated) return;
            PlayGamesPlatform.Instance.IncrementAchievement(id, steps, success =>
                Debug.Log($"[GPGS] Achievement increment {id}: {success}"));
        }

        public void ShowAchievements()
        {
            if (!IsAuthenticated) return;
            Social.ShowAchievementsUI();
        }
    }
}
