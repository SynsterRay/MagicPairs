using UnityEngine;
using System.Collections.Generic;

namespace MagicPairs.Cards
{
    public class CardGrid : MonoBehaviour
    {
        [SerializeField] private GameObject cardPrefab;

        private List<CardController> _cards = new();
        private Core.GameConfig _config;

        public IReadOnlyList<CardController> Cards => _cards;

        private void OnEnable() => Core.GameEvents.OnGameStarted += BuildGrid;
        private void OnDisable() => Core.GameEvents.OnGameStarted -= BuildGrid;

        public void BuildGrid()
        {
            ClearGrid();
            _config = Core.GameManager.Instance.Config;
            var deck = GenerateDeck();
            Shuffle(deck);
            SpawnCards(deck);
        }

        private void ClearGrid()
        {
            foreach (var card in _cards)
                if (card != null) Destroy(card.gameObject);
            _cards.Clear();
        }

        private List<CardData> GenerateDeck()
        {
            var deck = new List<CardData>();
            int pairsNeeded = _config.PairCount;

            for (int i = 0; i < pairsNeeded; i++)
            {
                var color = _config.colorPalette[i % _config.colorPalette.Length];
                deck.Add(CardData.CreatePair(i, color));
                deck.Add(CardData.CreatePair(i, color));
            }

            // Add Piotruś
            deck.Add(CardData.CreatePiotrus(_config.piotrusColor));

            // If deck is smaller than grid, trim grid (shouldn't happen with proper config)
            return deck;
        }

        private void Shuffle(List<CardData> deck)
        {
            // Fisher-Yates
            for (int i = deck.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                (deck[i], deck[j]) = (deck[j], deck[i]);
            }
        }

        private void SpawnCards(List<CardData> deck)
        {
            float totalWidth = _config.gridCols * (_config.cardWidth + _config.cardSpacing) - _config.cardSpacing;
            float totalHeight = _config.gridRows * (_config.cardHeight + _config.cardSpacing) - _config.cardSpacing;
            Vector3 origin = new(-totalWidth * 0.5f, totalHeight * 0.5f, 0f);

            // Deal from position (above grid)
            Vector3 dealFrom = new(0f, totalHeight * 0.5f + 2f, 0f);

            int index = 0;
            for (int row = 0; row < _config.gridRows; row++)
            {
                for (int col = 0; col < _config.gridCols; col++)
                {
                    if (index >= deck.Count) return;

                    float x = origin.x + col * (_config.cardWidth + _config.cardSpacing) + _config.cardWidth * 0.5f;
                    float y = origin.y - row * (_config.cardHeight + _config.cardSpacing) - _config.cardHeight * 0.5f;
                    Vector3 targetPos = new(x, y, 0f);

                    var go = Instantiate(cardPrefab, dealFrom, Quaternion.identity, transform);
                    go.name = $"Card_{row}_{col}";

                    var controller = go.GetComponent<CardController>();
                    controller.Initialize(deck[index], _config.cardBackColor);

                    // Deal animation
                    var animator = go.GetComponent<CardAnimator>();
                    float delay = index * 0.05f;
                    animator.PlayDealAnimation(dealFrom, targetPos, delay);

                    _cards.Add(controller);
                    index++;
                }
            }
        }
    }
}
