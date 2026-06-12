using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MagicPairs.Cards;
using MagicPairs.Core;

namespace MagicPairs.GameFlow
{
    public enum PowerUpType { Peek, Shuffle, Freeze }

    public class PowerUpManager : MonoBehaviour
    {
        private int _peekCount;
        private int _shuffleCount;
        private int _freezeCount;
        private bool _freezeActive;

        public int PeekCount => _peekCount;
        public int ShuffleCount => _shuffleCount;
        public int FreezeCount => _freezeCount;
        public bool FreezeActive => _freezeActive;

        public static event System.Action OnPowerUpsChanged;

        private ChallengeMode _challenge;
        private CardGrid _grid;

        private void OnEnable()
        {
            GameEvents.OnGameStarted += OnGameStarted;
            ChallengeMode.OnLevelComplete += OnLevelComplete;
            ChallengeMode.OnChallengeScoreChanged += OnScoreChanged;
        }

        private void OnDisable()
        {
            GameEvents.OnGameStarted -= OnGameStarted;
            ChallengeMode.OnLevelComplete -= OnLevelComplete;
            ChallengeMode.OnChallengeScoreChanged -= OnScoreChanged;
            StopAllCoroutines();
        }

        private void OnGameStarted()
        {
            _challenge = GetComponent<ChallengeMode>();
            _grid = FindAnyObjectByType<CardGrid>();
            if (_challenge == null || !_challenge.enabled) return;

            // Reset on new game (not on next level)
            if (_challenge.CurrentLevel == 1)
            {
                _peekCount = 1; // Start with 1 peek
                _shuffleCount = 0;
                _freezeCount = 0;
            }
            _freezeActive = false;
            OnPowerUpsChanged?.Invoke();
        }

        private void OnLevelComplete(int level)
        {
            // Award a power-up every 3 levels
            if (level % 3 == 0)
            {
                // Rotate: peek, shuffle, freeze
                int type = (level / 3) % 3;
                switch (type)
                {
                    case 0: _peekCount++; break;
                    case 1: _shuffleCount++; break;
                    case 2: _freezeCount++; break;
                }
                OnPowerUpsChanged?.Invoke();
            }
        }

        private int _lastStreak;
        private void OnScoreChanged(int score, int streak, int level)
        {
            // Award random power-up on reaching streak x5
            if (streak >= 5 && _lastStreak < 5)
            {
                int type = Random.Range(0, 3);
                switch (type)
                {
                    case 0: _peekCount++; break;
                    case 1: _shuffleCount++; break;
                    case 2: _freezeCount++; break;
                }
                OnPowerUpsChanged?.Invoke();
            }
            _lastStreak = streak;
        }

        public bool UsePeek()
        {
            if (_peekCount <= 0) return false;
            if (_challenge == null || _challenge.CurrentPlayerIndex != 0) return false;
            _peekCount--;
            OnPowerUpsChanged?.Invoke();
            Core.GPGSManager.Instance?.EventPowerUpUsed();
            StartCoroutine(PeekCoroutine());
            return true;
        }

        public bool UseShuffle()
        {
            if (_shuffleCount <= 0) return false;
            if (_challenge == null || _challenge.CurrentPlayerIndex != 0) return false;
            _shuffleCount--;
            OnPowerUpsChanged?.Invoke();
            Core.GPGSManager.Instance?.EventPowerUpUsed();
            ShuffleCards();
            return true;
        }

        public bool UseFreeze()
        {
            if (_freezeCount <= 0) return false;
            if (_challenge == null) return false;
            _freezeCount--;
            _freezeActive = true;
            OnPowerUpsChanged?.Invoke();
            Core.GPGSManager.Instance?.EventFreezeUsed();
            Core.GPGSManager.Instance?.EventPowerUpUsed();
            return true;
        }

        /// <summary>Called by ChallengeMode before AI turn. Returns true if AI should skip.</summary>
        public bool ConsumeFreeze()
        {
            if (!_freezeActive) return false;
            _freezeActive = false;
            return true;
        }

        public void AddPowerUp(PowerUpType type)
        {
            switch (type)
            {
                case PowerUpType.Peek: _peekCount++; break;
                case PowerUpType.Shuffle: _shuffleCount++; break;
                case PowerUpType.Freeze: _freezeCount++; break;
            }
            OnPowerUpsChanged?.Invoke();
        }

        private IEnumerator PeekCoroutine()
        {
            if (_grid == null) yield break;
            var config = GameManager.Instance.Config;

            // Find 2 random face-down cards that form a pair
            var available = new List<CardController>();
            foreach (var card in _grid.Cards)
                if (card != null && card.CanFlip && !card.Data.isPiotrus)
                    available.Add(card);

            if (available.Count < 2) yield break;

            // Pick 2 random cards
            int i1 = Random.Range(0, available.Count);
            var c1 = available[i1];
            available.RemoveAt(i1);
            int i2 = Random.Range(0, available.Count);
            var c2 = available[i2];

            // Flip them face-up briefly
            c1.PeekReveal();
            c2.PeekReveal();

            yield return new WaitForSeconds(1.0f);

            // Flip back
            if (c1 != null && c1.State == CardState.FaceUp)
                c1.FlipBack(config.cardBackColor);
            if (c2 != null && c2.State == CardState.FaceUp)
                c2.FlipBack(config.cardBackColor);
        }

        private void ShuffleCards()
        {
            if (_grid == null) return;

            // Collect face-down cards and their positions
            var faceDown = new List<CardController>();
            var positions = new List<Vector3>();

            foreach (var card in _grid.Cards)
            {
                if (card != null && card.CanFlip)
                {
                    faceDown.Add(card);
                    positions.Add(card.transform.position);
                }
            }

            // Shuffle positions
            for (int i = positions.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                (positions[i], positions[j]) = (positions[j], positions[i]);
            }

            // Assign new positions
            for (int i = 0; i < faceDown.Count; i++)
                faceDown[i].transform.position = positions[i];

            // Clear AI memory (cards moved)
            _challenge?.ClearAIMemory();
        }
    }
}
