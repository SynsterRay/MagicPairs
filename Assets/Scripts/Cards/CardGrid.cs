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
                if (card != null && card.gameObject != null) Destroy(card.gameObject);
            _cards.Clear();
        }

        private List<CardData> GenerateDeck()
        {
            var deck = new List<CardData>();
            bool skipJoker = GameFlow.TimeAttackMode.IsTimeAttackMode;
            int pairsNeeded = skipJoker ? _config.TotalSlots / 2 : _config.PairCount;

            bool useSpriteTheme = _config.theme == Core.CardTheme.Princess || _config.theme == Core.CardTheme.Cars;
            string folder = _config.theme == Core.CardTheme.Cars ? "CarCards" : "PrincessCards";
            Sprite[] sprites = null;

            if (useSpriteTheme)
            {
                sprites = LoadThemeSprites(folder);
                if (sprites.Length == 0)
                {
                    Debug.LogWarning($"[CardGrid] No sprites found in Resources/{folder}. Falling back to colors.");
                    useSpriteTheme = false;
                }
            }

            if (useSpriteTheme)
            {
                for (int i = 0; i < pairsNeeded; i++)
                {
                    var sprite = sprites[i % sprites.Length];
                    deck.Add(CardData.CreatePairWithSprite(i, sprite));
                    deck.Add(CardData.CreatePairWithSprite(i, sprite));
                }
                if (!skipJoker)
                {
                    var jokerSprite = Resources.Load<Sprite>($"{folder}/joker");
                    if (jokerSprite == null)
                    {
                        var jokerTex = Resources.Load<Texture2D>($"{folder}/joker");
                        if (jokerTex != null)
                            jokerSprite = Sprite.Create(jokerTex, new Rect(0, 0, jokerTex.width, jokerTex.height), new Vector2(0.5f, 0.5f), 100f);
                    }
                    deck.Add(CardData.CreatePiotrusWithSprite(jokerSprite));
                }
            }
            else
            {
                for (int i = 0; i < pairsNeeded; i++)
                {
                    var color = _config.colorPalette != null && _config.colorPalette.Length > 0
                        ? _config.colorPalette[i % _config.colorPalette.Length]
                        : Color.HSVToRGB((float)i / Mathf.Max(1, pairsNeeded), 0.8f, 0.9f);
                    deck.Add(CardData.CreatePair(i, color));
                    deck.Add(CardData.CreatePair(i, color));
                }
                if (!skipJoker)
                    deck.Add(CardData.CreatePiotrus(_config.piotrusColor));
            }

            return deck;
        }

        private Sprite[] LoadPrincessSprites()
        {
            return LoadThemeSprites("PrincessCards");
        }

        private Sprite[] LoadThemeSprites(string folder)
        {
            var all = Resources.LoadAll<Sprite>(folder);
            var cards = new List<Sprite>();
            foreach (var s in all)
            {
                if (!s.name.Contains("joker") && !s.name.Contains("back_card") && !s.name.Contains("back"))
                    cards.Add(s);
            }

            if (cards.Count == 0)
            {
                var textures = Resources.LoadAll<Texture2D>(folder);
                foreach (var tex in textures)
                {
                    if (tex.name.Contains("joker")) continue;
                    if (tex.name.Contains("back")) continue;
                    var sprite = Sprite.Create(tex,
                        new Rect(0, 0, tex.width, tex.height),
                        new Vector2(0.5f, 0.5f), 100f);
                    sprite.name = tex.name;
                    cards.Add(sprite);
                }
            }

            return cards.ToArray();
        }

        private void Shuffle(List<CardData> deck)
        {
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

            // Adjust camera to fit grid, accounting for UI bar taking top 15% of screen
            var cam = UnityEngine.Camera.main;
            if (cam != null)
            {
                float visibleHeight = totalHeight + 1f; // padding
                float visibleWidth = totalWidth + 1f;
                // UI takes top 15%, so game area is 85% of screen
                float neededOrthoForHeight = visibleHeight / (2f * 0.85f);
                float neededOrthoForWidth = visibleWidth / (2f * cam.aspect);
                cam.orthographicSize = Mathf.Max(neededOrthoForHeight, neededOrthoForWidth);
            }

            // Offset grid down so it's centered in the visible game area (below UI bar)
            float ortho = cam != null ? cam.orthographicSize : 5f;
            float gridCenterY = -ortho * 0.15f; // shift down by 15% of view

            Vector3 origin = new(-totalWidth * 0.5f, gridCenterY + totalHeight * 0.5f, 0f);
            Vector3 dealFrom = new(0f, gridCenterY + totalHeight * 0.5f + 2f, 0f);

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
