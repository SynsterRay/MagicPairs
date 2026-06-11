using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

namespace MagicPairs.Core
{
    public class GPGSManager : MonoBehaviour
    {
        public static GPGSManager Instance { get; private set; }
        public bool IsAuthenticated { get; private set; }

        // Wstaw swoje ID z Google Play Console
        private const string LeaderboardChallengeId = "CgkIxYnu6M4MEAIQAQ";
        private const string LeaderboardTimeAttackId = "CgkIxYnu6M4MEAIQAQ"; // TODO: zamień na osobne ID gdy utworzysz drugą tablicę

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

        public void PostChallengeScore(int score)
        {
            if (!IsAuthenticated) return;
            Social.ReportScore(score, LeaderboardChallengeId, success =>
                Debug.Log($"[GPGS] Challenge score posted: {success}"));
        }

        public void PostTimeAttackScore(float timeLeft)
        {
            if (!IsAuthenticated) return;
            // Przechowuj jako milisekundy
            long ms = (long)(timeLeft * 1000);
            Social.ReportScore(ms, LeaderboardTimeAttackId, success =>
                Debug.Log($"[GPGS] TimeAttack score posted: {success}"));
        }

        public void ShowLeaderboard()
        {
            if (!IsAuthenticated) return;
            PlayGamesPlatform.Instance.ShowLeaderboardUI();
        }
    }
}
