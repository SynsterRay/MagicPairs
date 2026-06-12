using UnityEngine;
using MagicPairs.GameFlow;
using MagicPairs.UI;

namespace MagicPairs.Core
{
    public class AchievementTracker : MonoBehaviour
    {
        private const string ThemesPlayedKey = "MagicPairs_ThemesPlayed";

        private int _jokerHitsThisGame;
        private int _mismatchesThisLevel;
        private bool _hadMismatchThisLevel;

        private void OnEnable()
        {
            GameEvents.OnPairMatched += OnPairMatched;
            GameEvents.OnPairMismatched += OnMismatch;
            GameEvents.OnPiotrusFlipped += OnJoker;
            GameEvents.OnGameOver += OnGameOver;
            GameEvents.OnGameStarted += OnGameStarted;
            ChallengeMode.OnChallengeScoreChanged += OnChallengeScore;
            ChallengeMode.OnLevelComplete += OnLevelComplete;
            TimeAttackMode.OnTimeAttackComplete += OnTimeAttackComplete;
        }

        private void OnDisable()
        {
            GameEvents.OnPairMatched -= OnPairMatched;
            GameEvents.OnPairMismatched -= OnMismatch;
            GameEvents.OnPiotrusFlipped -= OnJoker;
            GameEvents.OnGameOver -= OnGameOver;
            GameEvents.OnGameStarted -= OnGameStarted;
            ChallengeMode.OnChallengeScoreChanged -= OnChallengeScore;
            ChallengeMode.OnLevelComplete -= OnLevelComplete;
            TimeAttackMode.OnTimeAttackComplete -= OnTimeAttackComplete;
        }

        private void OnGameStarted()
        {
            _jokerHitsThisGame = 0;
            _mismatchesThisLevel = 0;
            _hadMismatchThisLevel = false;

            // Collector: track themes played
            TrackThemePlayed();

            // Incremental: game played
            GPGSManager.Instance?.IncrementGamePlayed();

            // Events
            GPGSManager.Instance?.EventGameStarted();
            GPGSManager.Instance?.EventThemeSwitch();
        }

        private void OnPairMatched(int playerIndex, int colorIndex)
        {
            if (playerIndex != 0) return; // Only human player

            // First Pair
            GPGSManager.Instance?.UnlockFirstPair();

            // Incremental: pairs found
            GPGSManager.Instance?.IncrementPairFound();

            // Event
            GPGSManager.Instance?.EventPairFound();
            GPGSManager.Instance?.EventCardFlipped();
        }

        private void OnMismatch()
        {
            _hadMismatchThisLevel = true;
            _mismatchesThisLevel++;

            // Event
            GPGSManager.Instance?.EventMismatch();
        }

        private void OnJoker(int playerIndex)
        {
            if (playerIndex != 0) return;
            _jokerHitsThisGame++;

            // Incremental: Joker Magnet
            GPGSManager.Instance?.IncrementJokerHit();

            // Event
            GPGSManager.Instance?.EventJokerHit();
        }

        private void OnGameOver(int winnerIndex)
        {
            // Event: game completed
            GPGSManager.Instance?.EventGameCompleted();

            // Memory Master: win Arcade game
            if (!MainMenu.IsChallengeMode && !MainMenu.IsTimeAttackMode && winnerIndex == 0)
            {
                GPGSManager.Instance?.UnlockMemoryMaster();
                GPGSManager.Instance?.IncrementWin();
                GPGSManager.Instance?.EventArcadeWin();
            }

            // Joker Survivor: hit joker 5+ times and still win
            if (winnerIndex == 0 && _jokerHitsThisGame >= 5)
                GPGSManager.Instance?.UnlockJokerSurvivor();
        }

        private void OnChallengeScore(int score, int streak, int level)
        {
            // Streak x5
            if (streak >= 5)
            {
                GPGSManager.Instance?.UnlockStreakX5();
                GPGSManager.Instance?.EventStreakX5();
            }
        }

        private void OnLevelComplete(int level)
        {
            // Perfect Level: no mismatches this level
            if (!_hadMismatchThisLevel)
            {
                GPGSManager.Instance?.UnlockPerfectLevel();
                GPGSManager.Instance?.EventPerfectLevel();
            }

            // Reset for next level
            _hadMismatchThisLevel = false;
            _mismatchesThisLevel = 0;

            // Level milestones
            if (level >= 5) GPGSManager.Instance?.UnlockLevel5();
            if (level >= 10) GPGSManager.Instance?.UnlockLevel10();

            // Incremental
            GPGSManager.Instance?.IncrementLevelComplete();
            GPGSManager.Instance?.IncrementWin();

            // Event
            GPGSManager.Instance?.EventLevelCompleted();
        }

        private void OnTimeAttackComplete(float timeLeft)
        {
            // Sprinter: complete Time Attack on Easy
            if (MainMenu.SelectedDifficulty == Difficulty.Easy)
                GPGSManager.Instance?.UnlockSprinter();

            // Speed Demon: complete Time Attack on Hard
            if (MainMenu.SelectedDifficulty == Difficulty.Hard)
                GPGSManager.Instance?.UnlockSpeedDemon();

            GPGSManager.Instance?.IncrementWin();

            // Event
            GPGSManager.Instance?.EventTimeAttackWin();
        }

        private void TrackThemePlayed()
        {
            var config = GameManager.Instance?.Config;
            if (config == null) return;

            int themes = PlayerPrefs.GetInt(ThemesPlayedKey, 0);
            int bit = 1 << (int)config.theme;
            if ((themes & bit) == 0)
            {
                themes |= bit;
                PlayerPrefs.SetInt(ThemesPlayedKey, themes);
                PlayerPrefs.Save();
            }

            // All 3 themes: Colors(0), Princess(1), Cars(2) = bits 1+2+4 = 7
            if (themes >= 7)
                GPGSManager.Instance?.UnlockCollector();
        }
    }
}
