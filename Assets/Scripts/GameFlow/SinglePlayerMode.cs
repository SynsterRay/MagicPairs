using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MagicPairs.Cards;
using MagicPairs.Core;

namespace MagicPairs.GameFlow
{
    public class SinglePlayerMode : MonoBehaviour, IGameMode
    {
        public int CurrentPlayerIndex { get; private set; }

        [SerializeField] private float aiThinkDelay = 1.0f;
        [SerializeField] [Range(0f, 1f)] private float aiMemoryChance = 0.6f;

        private CardController _firstPick;
        private bool _waitingForResult;
        private int _pairsFound;
        private int _totalPairs;
        private int[] _scores = new int[2];
        private GameConfig _config;

        // AI memory: remembers cards it has seen
        private Dictionary<int, List<CardController>> _aiMemory = new();

        private void OnEnable()
        {
            GameEvents.OnGameStarted += StartGame;
            GameEvents.OnTurnChanged += OnTurnChanged;
        }

        private void OnDisable()
        {
            GameEvents.OnGameStarted -= StartGame;
            GameEvents.OnTurnChanged -= OnTurnChanged;
        }

        public void StartGame()
        {
            _config = GameManager.Instance.Config;
            _totalPairs = _config.PairCount;
            _pairsFound = 0;
            _scores[0] = 0;
            _scores[1] = 0;
            _firstPick = null;
            _waitingForResult = false;
            _aiMemory.Clear();
            CurrentPlayerIndex = 0;
            GameEvents.FireTurnChanged(CurrentPlayerIndex);
        }

        public void OnCardSelected(CardController card)
        {
            // Only accept input from human (player 0)
            if (CurrentPlayerIndex != 0) return;
            if (_waitingForResult) return;
            if (!card.CanFlip) return;

            card.Flip();
            card.OnFlipComplete += OnCardFlipped;
        }

        private void OnTurnChanged(int playerIndex)
        {
            if (playerIndex == 1)
                StartCoroutine(AITurn());
        }

        private IEnumerator AITurn()
        {
            yield return new WaitForSeconds(aiThinkDelay);

            var grid = GetComponentInParent<CardGrid>() ?? FindAnyObjectByType<CardGrid>();
            if (grid == null) yield break;

            // Pick first card
            var firstCard = AIPickCard(grid, null);
            if (firstCard == null) yield break;

            firstCard.Flip();
            firstCard.OnFlipComplete += OnCardFlipped;
        }

        private void OnCardFlipped(CardController card)
        {
            card.OnFlipComplete -= OnCardFlipped;

            // Remember this card
            RememberCard(card);

            // Piotruś check
            if (card.Data.isPiotrus)
            {
                _waitingForResult = true;
                GameEvents.FirePiotrusFlipped(CurrentPlayerIndex);
                StartCoroutine(HandlePiotrus(card));
                return;
            }

            if (_firstPick == null)
            {
                _firstPick = card;

                // If AI's turn, pick second card
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

        private IEnumerator AIPickSecond(CardController firstCard)
        {
            yield return new WaitForSeconds(aiThinkDelay * 0.7f);

            var grid = GetComponentInParent<CardGrid>() ?? FindAnyObjectByType<CardGrid>();
            if (grid == null) yield break;

            CardController secondCard = null;

            // Try to use memory to find a match
            if (Random.value < aiMemoryChance && _aiMemory.ContainsKey(firstCard.Data.colorIndex))
            {
                var remembered = _aiMemory[firstCard.Data.colorIndex];
                foreach (var c in remembered)
                {
                    if (c != null && c != firstCard && c.CanFlip)
                    {
                        secondCard = c;
                        break;
                    }
                }
            }

            // If no memory match, pick random
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
            {
                if (card != null && card.CanFlip && card != exclude)
                    available.Add(card);
            }

            if (available.Count == 0) return null;

            // Check memory for a known pair
            if (Random.value < aiMemoryChance)
            {
                foreach (var kvp in _aiMemory)
                {
                    if (kvp.Value.Count >= 2)
                    {
                        var validPair = new List<CardController>();
                        foreach (var c in kvp.Value)
                        {
                            if (c != null && c.CanFlip && c != exclude)
                                validPair.Add(c);
                        }
                        if (validPair.Count >= 2)
                            return validPair[0];
                    }
                }
            }

            return available[Random.Range(0, available.Count)];
        }

        private void RememberCard(CardController card)
        {
            if (card.Data.isPiotrus) return;
            int colorIdx = card.Data.colorIndex;
            if (!_aiMemory.ContainsKey(colorIdx))
                _aiMemory[colorIdx] = new List<CardController>();
            if (!_aiMemory[colorIdx].Contains(card))
                _aiMemory[colorIdx].Add(card);
        }

        private IEnumerator HandleMatch(CardController a, CardController b)
        {
            a.SetMatched();
            b.SetMatched();
            _scores[CurrentPlayerIndex]++;
            _pairsFound++;

            GameEvents.FirePairMatched(CurrentPlayerIndex, a.Data.colorIndex);
            GameEvents.FireScoreChanged(CurrentPlayerIndex, _scores[CurrentPlayerIndex]);

            yield return new WaitForSeconds(0.6f);

            _firstPick = null;
            _waitingForResult = false;

            if (_pairsFound >= _totalPairs)
            {
                int winner = _scores[0] > _scores[1] ? 0 : _scores[1] > _scores[0] ? 1 : -1;
                GameEvents.FireGameOver(winner);
            }
            else if (CurrentPlayerIndex == 1)
            {
                // AI gets another turn after match
                StartCoroutine(AITurn());
            }
        }

        private IEnumerator HandleMismatch(CardController a, CardController b)
        {
            yield return new WaitForSeconds(_config.mismatchDelay);

            a.FlipBack(_config.cardBackColor);
            b.FlipBack(_config.cardBackColor);

            GameEvents.FirePairMismatched();
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
        }
    }
}
