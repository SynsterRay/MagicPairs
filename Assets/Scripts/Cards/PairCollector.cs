using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MagicPairs.Core;

namespace MagicPairs.Cards
{
    public class PairCollector : MonoBehaviour
    {
        [SerializeField] private float collectDelay = 0.5f;

        private List<CardController> _player1Cards = new();
        private List<CardController> _player2Cards = new();

        public IReadOnlyList<CardController> Player1Cards => _player1Cards;
        public IReadOnlyList<CardController> Player2Cards => _player2Cards;

        private void OnEnable()
        {
            GameEvents.OnGameStarted += ResetCollections;
            GameEvents.OnPairMatched += OnPairMatched;
        }

        private void OnDisable()
        {
            GameEvents.OnGameStarted -= ResetCollections;
            GameEvents.OnPairMatched -= OnPairMatched;
        }

        private void ResetCollections()
        {
            _player1Cards.Clear();
            _player2Cards.Clear();
        }

        private void OnPairMatched(int playerIndex, int colorIndex)
        {
            var grid = GetComponent<CardGrid>();
            if (grid == null) return;

            var matched = new List<CardController>();
            foreach (var card in grid.Cards)
            {
                if (card != null && card.State == CardState.Matched
                    && card.Data.colorIndex == colorIndex && !IsCollected(card))
                {
                    matched.Add(card);
                }
            }

            if (matched.Count >= 2)
                StartCoroutine(CollectPair(playerIndex, matched[0], matched[1]));
        }

        private bool IsCollected(CardController card)
        {
            return _player1Cards.Contains(card) || _player2Cards.Contains(card);
        }

        private IEnumerator CollectPair(int playerIndex, CardController a, CardController b)
        {
            yield return new WaitForSeconds(collectDelay);

            var list = playerIndex == 0 ? _player1Cards : _player2Cards;
            list.Add(a);
            list.Add(b);

            // Hide matched cards
            a.gameObject.SetActive(false);
            b.gameObject.SetActive(false);
        }

    }
}
