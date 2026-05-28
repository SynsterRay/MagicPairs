using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MagicPairs.Cards;
using MagicPairs.Core;

namespace MagicPairs.GameFlow
{
    public class ChallengeMode : MonoBehaviour, IGameMode
    {
        public int CurrentPlayerIndex { get; private set; }

        [SerializeField] private float aiThinkDelay = 1.0f;
        [SerializeField] [Range(0f, 1f)] private float baseAiMemory = 0.3f;

        private CardController _firstPick;
        private bool _waitingForResult;
        private int _pairsFound;
        private int _totalPairs;
        private GameConfig _config;

        private int _currentLevel = 1;
        private int _score;
        private int _streak;
        private float _currentAiMemory;

        private Dictionary<int, List<CardController>> _aiMemory = new();

        public int CurrentLevel => _currentLevel;
        public int Score => _score;
        public int Streak => _streak;

        public static event System.Action<int, int, int> OnChallengeScoreChanged; // score, streak, level
        public static event System.Action<int> OnLevelComplete; // level
        public static event System.Action<int> OnChallengeGameOver; // final score

        private bool _isNextLevel;

        private void OnEnable() => GameEvents.OnGameStarted += OnGameStarted;
        private void OnDisable() => GameEvents.OnGameStarted -= OnGameStarted;

        private void OnGameStarted()
        {
            if (_isNextLevel)
            {
                _isNextLevel = false;
                return; // Don't reset score on next level
            }
            StartGame();
        }

        public void StartGame()
        {
            _currentLevel = 1;
            _score = 0;
            _streak = 0;
            _firstPick = null;
            _waitingForResult = false;
            _aiMemory.Clear();
            CurrentPlayerIndex = 0;
            ApplyLevelConfig();
            GameEvents.FireTurnChanged(CurrentPlayerIndex);
            OnChallengeScoreChanged?.Invoke(_score, _streak, _currentLevel);
        }

        public void StartNextLevel()
        {
            _currentLevel++;
            _firstPick = null;
            _waitingForResult = false;
            _aiMemory.Clear();
            CurrentPlayerIndex = 0;
            ApplyLevelConfig();
            _isNextLevel = true;
            GameEvents.FireGameStarted();
            GameEvents.FireTurnChanged(CurrentPlayerIndex);
            OnChallengeScoreChanged?.Invoke(_score, _streak, _currentLevel);
        }

        private void ApplyLevelConfig()
        {
            _config = GameManager.Instance.Config;
            // Cards per level: start at 5 (2 pairs + piotrus), add 2 each level (one more pair)
            // Max: 5x6=30 → 29 cards (14 pairs) for princess, 17 (8 pairs) for colors
            int maxPairs = _config.theme == CardTheme.Princess ? 14 : 8;
            int pairs = Mathf.Min(2 + (_currentLevel - 1), maxPairs);
            int totalCards = pairs * 2 + 1; // +1 for piotrus

            // Find best grid fit
            GetGridSize(totalCards, out int rows, out int cols);
            _config.gridRows = rows;
            _config.gridCols = cols;

            _totalPairs = pairs;
            _pairsFound = 0;

            // AI difficulty scales with level after max cards reached
            int levelsAfterMax = Mathf.Max(0, _currentLevel - 1 - (maxPairs - 2));
            _currentAiMemory = Mathf.Min(0.95f, baseAiMemory + levelsAfterMax * 0.05f);
        }

        private void GetGridSize(int totalCards, out int rows, out int cols)
        {
            // Find smallest grid that fits totalCards
            if (totalCards <= 6) { rows = 2; cols = 3; return; }
            if (totalCards <= 9) { rows = 3; cols = 3; return; }
            if (totalCards <= 12) { rows = 3; cols = 4; return; }
            if (totalCards <= 15) { rows = 3; cols = 5; return; }
            if (totalCards <= 20) { rows = 4; cols = 5; return; }
            if (totalCards <= 25) { rows = 5; cols = 5; return; }
            rows = 5; cols = 6;
        }

        public void OnCardSelected(CardController card)
        {
            if (CurrentPlayerIndex != 0) return;
            if (_waitingForResult) return;
            if (!card.CanFlip) return;

            card.Flip();
            card.OnFlipComplete += OnCardFlipped;
        }

        private void OnCardFlipped(CardController card)
        {
            card.OnFlipComplete -= OnCardFlipped;
            RememberCard(card);

            if (card.Data.isPiotrus)
            {
                _waitingForResult = true;
                _streak = 0;
                GameEvents.FirePiotrusFlipped(CurrentPlayerIndex);
                OnChallengeScoreChanged?.Invoke(_score, _streak, _currentLevel);
                StartCoroutine(HandlePiotrus(card));
                return;
            }

            if (_firstPick == null)
            {
                _firstPick = card;
                if (CurrentPlayerIndex == 1)
                    StartCoroutine(AIPickSecond(card));
            }
            else
            {
                _waitingForResult = true;
                if (_firstPick.Data.colorIndex == card.Data.colorIndex)
                    StartCoroutine(HandleMatch(_firstPick, card));
                else
                    StartCoroutine(HandleMismatch(_firstPick, card));
            }
        }

        private IEnumerator HandleMatch(CardController a, CardController b)
        {
            a.SetMatched();
            b.SetMatched();
            _pairsFound++;

            if (CurrentPlayerIndex == 0)
            {
                _streak++;
                int multiplier = Mathf.Min(_streak, 5); // max 5x
                _score += 100 * multiplier;
                OnChallengeScoreChanged?.Invoke(_score, _streak, _currentLevel);
            }

            GameEvents.FirePairMatched(CurrentPlayerIndex, a.Data.colorIndex);

            yield return new WaitForSeconds(0.6f);

            _firstPick = null;
            _waitingForResult = false;

            if (_pairsFound >= _totalPairs)
            {
                if (CurrentPlayerIndex == 0)
                {
                    // Player completed the level
                    _score += 500 * _currentLevel;
                    OnChallengeScoreChanged?.Invoke(_score, _streak, _currentLevel);
                    OnLevelComplete?.Invoke(_currentLevel);
                }
                else
                {
                    // AI found last pair — game over
                    OnChallengeGameOver?.Invoke(_score);
                }
            }
            else if (CurrentPlayerIndex == 1)
            {
                StartCoroutine(AITurn());
            }
        }

        private IEnumerator HandleMismatch(CardController a, CardController b)
        {
            yield return new WaitForSeconds(_config.mismatchDelay);

            a.FlipBack(_config.cardBackColor);
            b.FlipBack(_config.cardBackColor);

            if (CurrentPlayerIndex == 0)
                _streak = 0;

            GameEvents.FirePairMismatched();
            OnChallengeScoreChanged?.Invoke(_score, _streak, _currentLevel);
            _firstPick = null;
            _waitingForResult = false;
            SwitchTurn();
        }

        private IEnumerator HandlePiotrus(CardController card)
        {
            yield return new WaitForSeconds(_config.piotrusDelay);
            card.FlipBack(_config.cardBackColor);
            if (_firstPick != null)
            {
                _firstPick.FlipBack(_config.cardBackColor);
                _firstPick = null;
            }
            _waitingForResult = false;
            SwitchTurn();
        }

        private void SwitchTurn()
        {
            CurrentPlayerIndex = 1 - CurrentPlayerIndex;
            GameEvents.FireTurnChanged(CurrentPlayerIndex);
            if (CurrentPlayerIndex == 1)
                StartCoroutine(AITurn());
        }

        private IEnumerator AITurn()
        {
            yield return new WaitForSeconds(aiThinkDelay);
            var grid = FindAnyObjectByType<CardGrid>();
            if (grid == null) yield break;

            var firstCard = AIPickCard(grid, null);
            if (firstCard == null) yield break;

            firstCard.Flip();
            firstCard.OnFlipComplete += OnCardFlipped;
        }

        private IEnumerator AIPickSecond(CardController firstCard)
        {
            yield return new WaitForSeconds(aiThinkDelay * 0.7f);
            var grid = FindAnyObjectByType<CardGrid>();
            if (grid == null) yield break;

            CardController secondCard = null;

            if (Random.value < _currentAiMemory && _aiMemory.ContainsKey(firstCard.Data.colorIndex))
            {
                foreach (var c in _aiMemory[firstCard.Data.colorIndex])
                {
                    if (c != null && c != firstCard && c.CanFlip)
                    { secondCard = c; break; }
                }
            }

            if (secondCard == null)
                secondCard = AIPickCard(grid, firstCard);
            if (secondCard == null) yield break;

            secondCard.Flip();
            secondCard.OnFlipComplete += OnCardFlipped;
        }

        private CardController AIPickCard(CardGrid grid, CardController exclude)
        {
            var available = new List<CardController>();
            foreach (var card in grid.Cards)
                if (card != null && card.CanFlip && card != exclude)
                    available.Add(card);

            if (available.Count == 0) return null;

            if (Random.value < _currentAiMemory)
            {
                foreach (var kvp in _aiMemory)
                {
                    var valid = new List<CardController>();
                    foreach (var c in kvp.Value)
                        if (c != null && c.CanFlip && c != exclude)
                            valid.Add(c);
                    if (valid.Count >= 2) return valid[0];
                }
            }

            return available[Random.Range(0, available.Count)];
        }

        private void RememberCard(CardController card)
        {
            if (card.Data.isPiotrus) return;
            int idx = card.Data.colorIndex;
            if (!_aiMemory.ContainsKey(idx))
                _aiMemory[idx] = new List<CardController>();
            if (!_aiMemory[idx].Contains(card))
                _aiMemory[idx].Add(card);
        }

        /// <summary>Called when player loses (AI finds last pair or player quits).</summary>
        public void EndChallenge()
        {
            OnChallengeGameOver?.Invoke(_score);
        }
    }
}
