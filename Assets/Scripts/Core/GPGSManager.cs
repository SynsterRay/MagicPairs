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

        public static event System.Action OnAuthChanged;

        public void SignIn()
        {
            PlayGamesPlatform.Instance.Authenticate(status =>
            {
                IsAuthenticated = status == SignInStatus.Success;
                OnAuthChanged?.Invoke();
            });
        }

        // --- Leaderboards ---

        public void PostChallengeScore(int score)
        {
            if (!IsAuthenticated) return;
            Social.ReportScore(score, LeaderboardChallengeId, success => { });
        }

        public void PostTimeAttackScore(float timeLeft)
        {
            if (!IsAuthenticated) return;
            long ms = (long)(timeLeft * 1000);
            Social.ReportScore(ms, LeaderboardTimeAttackId, success => { });
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

        // --- Events ---

        private const string EvtPairsFound = "CglxYnu6M4MEAIQew";
        private const string EvtGamesStarted = "CglxYnu6M4MEAIQFA";
        private const string EvtGamesCompleted = "CglxYnu6M4MEAIQFQ";
        private const string EvtChallengeFailed = "CglxYnu6M4MEAIQFg";
        private const string EvtJokerHits = "CglxYnu6M4MEAIQFw";
        private const string EvtMismatches = "CglxYnu6M4MEAIQGA";
        private const string EvtLevelsCompleted = "CglxYnu6M4MEAIQGQ";
        private const string EvtStreakX5 = "CglxYnu6M4MEAIQGg";
        private const string EvtRewardedAds = "CglxYnu6M4MEAIQGw";
        private const string EvtTimeAttackTimeouts = "CglxYnu6M4MEAIQHA";
        private const string EvtArcadeWins = "CglxYnu6M4MEAIQHQ";
        private const string EvtCardsFlipped = "CglxYnu6M4MEAIQHg";
        private const string EvtDailyBonus = "CglxYnu6M4MEAIQHw";
        private const string EvtSecondChance = "CglxYnu6M4MEAIQIA";
        private const string EvtPerfectLevels = "CglxYnu6M4MEAIQIQ";
        private const string EvtFreezeUsed = "CglxYnu6M4MEAIQIg";
        private const string EvtThemeSwitches = "CglxYnu6M4MEAIQIw";
        private const string EvtPowerUpsUsed = "CglxYnu6M4MEAIQJA";
        private const string EvtTimeAttackWins = "CglxYnu6M4MEAIQJQ";
        private const string EvtTotalScore = "CglxYnu6M4MEAIQJg";

        public void EventPairFound() => SubmitEvent(EvtPairsFound, 1);
        public void EventGameStarted() => SubmitEvent(EvtGamesStarted, 1);
        public void EventGameCompleted() => SubmitEvent(EvtGamesCompleted, 1);
        public void EventChallengeFailed() => SubmitEvent(EvtChallengeFailed, 1);
        public void EventJokerHit() => SubmitEvent(EvtJokerHits, 1);
        public void EventMismatch() => SubmitEvent(EvtMismatches, 1);
        public void EventLevelCompleted() => SubmitEvent(EvtLevelsCompleted, 1);
        public void EventStreakX5() => SubmitEvent(EvtStreakX5, 1);
        public void EventRewardedAd() => SubmitEvent(EvtRewardedAds, 1);
        public void EventTimeAttackTimeout() => SubmitEvent(EvtTimeAttackTimeouts, 1);
        public void EventArcadeWin() => SubmitEvent(EvtArcadeWins, 1);
        public void EventCardFlipped() => SubmitEvent(EvtCardsFlipped, 1);
        public void EventDailyBonus() => SubmitEvent(EvtDailyBonus, 1);
        public void EventSecondChance() => SubmitEvent(EvtSecondChance, 1);
        public void EventPerfectLevel() => SubmitEvent(EvtPerfectLevels, 1);
        public void EventFreezeUsed() => SubmitEvent(EvtFreezeUsed, 1);
        public void EventThemeSwitch() => SubmitEvent(EvtThemeSwitches, 1);
        public void EventPowerUpUsed() => SubmitEvent(EvtPowerUpsUsed, 1);
        public void EventTimeAttackWin() => SubmitEvent(EvtTimeAttackWins, 1);
        public void EventTotalScore(int amount) => SubmitEvent(EvtTotalScore, amount);

        private void SubmitEvent(string eventId, int count)
        {
            if (!IsAuthenticated) return;
            PlayGamesPlatform.Instance.Events.IncrementEvent(eventId, (uint)count);
        }

        // --- Helpers ---

        private void Unlock(string id)
        {
            if (!IsAuthenticated) return;
            Social.ReportProgress(id, 100.0, success => { });
        }

        private void Increment(string id, int steps)
        {
            if (!IsAuthenticated) return;
            PlayGamesPlatform.Instance.IncrementAchievement(id, steps, success => { });
        }

        public void ShowAchievements()
        {
            if (!IsAuthenticated) return;
            Social.ShowAchievementsUI();
        }
    }
}
