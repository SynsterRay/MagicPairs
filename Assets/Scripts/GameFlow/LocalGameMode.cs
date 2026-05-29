using UnityEngine;
using System.Collections;
using MagicPairs.Cards;
using MagicPairs.Core;

namespace MagicPairs.GameFlow
{
    public class LocalGameMode : MonoBehaviour, IGameMode
    {
        public int CurrentPlayerIndex { get; private set; }

        private CardController _firstPick;
        private bool _waitingForResult;
        private int _pairsFound;
        private int _totalPairs;
        private int[] _scores = new int[2];
        private GameConfig _config;

        private void OnEnable() => GameEvents.OnGameStarted += StartGame;
        private void OnDisable()
        {
            GameEvents.OnGameStarted -= StartGame;
            StopAllCoroutines();
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
            CurrentPlayerIndex = 0;
            GameEvents.FireTurnChanged(CurrentPlayerIndex);
        }

        public void OnCardSelected(CardController card)
        {
            if (_waitingForResult) return;
            if (!card.CanFlip) return;

            card.Flip();
            card.OnFlipComplete += OnCardFlipped;
        }

        private void OnCardFlipped(CardController card)
        {
            card.OnFlipComplete -= OnCardFlipped;

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
