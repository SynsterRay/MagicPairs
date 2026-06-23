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

            bool useSpriteTheme = _config.theme == Core.CardTheme.Princess || _config.theme == Core.CardTheme.Cars || _config.theme == Core.CardTheme.Dinos || _config.theme == Core.CardTheme.Animals || _config.theme == Core.CardTheme.SpaceAnimals;
            string folder = _config.theme switch
            {
                Core.CardTheme.Cars => "CarCards",
                Core.CardTheme.Dinos => "WaterWorldCards",
                Core.CardTheme.Animals => "AnimalCards",
                Core.CardTheme.SpaceAnimals => "SpaceAnimalsCards",
                _ => "PrincessCards"
            };
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
            int cardCount = deck.Count;
            var cam = UnityEngine.Camera.main;
            float aspect = cam != null ? cam.aspect : 9f / 16f;

            // Available area: full width, 82% height (top 18% = UI bar)
            float screenAspect = aspect / 0.82f;

            // Find best rows x cols for this screen
            FindBestGrid(cardCount, screenAspect, out int rows, out int cols);

            // Card aspect ratio 1:1.4
            float cardAspect = 1f / 1.4f;
            float spacing = 0.12f;

            // Start with ortho=5 to calculate available world space
            float ortho = 5f;
            float availH = ortho * 2f * 0.82f;
            float availW = ortho * 2f * aspect;

            // Max card size that fits
            float maxW = (availW - spacing * (cols + 1)) / cols;
            float maxH = (availH - spacing * (rows + 1)) / rows;

            float cardW, cardH;
            if (maxW / cardAspect <= maxH)
            { cardW = maxW; cardH = cardW / cardAspect; }
            else
            { cardH = maxH; cardW = cardH * cardAspect; }

            // Total grid size
            float totalWidth = cols * cardW + (cols - 1) * spacing;
            float totalHeight = rows * cardH + (rows - 1) * spacing;

            // Fit camera
            if (cam != null)
            {
                float neededH = (totalHeight + spacing * 2) / (2f * 0.82f);
                float neededW = (totalWidth + spacing * 2) / (2f * aspect);
                cam.orthographicSize = Mathf.Max(neededH, neededW);
                ortho = cam.orthographicSize;
            }

            // Center grid below UI
            float gridCenterY = -ortho * 0.18f;
            Vector3 origin = new(-totalWidth * 0.5f, gridCenterY + totalHeight * 0.5f, 0f);
            Vector3 dealFrom = new(0f, gridCenterY + totalHeight * 0.5f + 2f, 0f);

            int index = 0;
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    if (index >= cardCount) break;

                    float x = origin.x + col * (cardW + spacing) + cardW * 0.5f;
                    float y = origin.y - row * (cardH + spacing) - cardH * 0.5f;
                    Vector3 targetPos = new(x, y, 0f);

                    var go = Instantiate(cardPrefab, dealFrom, Quaternion.identity, transform);
                    go.name = $"Card_{row}_{col}";
                    go.transform.localScale = new Vector3(cardW, cardH, 1f);

                    var controller = go.GetComponent<CardController>();
                    controller.Initialize(deck[index], _config.cardBackColor);

                    var animator = go.GetComponent<CardAnimator>();
                    animator.PlayDealAnimation(dealFrom, targetPos, index * 0.05f);

                    _cards.Add(controller);
                    index++;
                }
            }
        }

        private void FindBestGrid(int cardCount, float areaAspect, out int bestRows, out int bestCols)
        {
            bestRows = 1;
            bestCols = cardCount;
            float bestCardArea = 0f;
            float cardAspect = 1f / 1.4f;

            for (int r = 1; r <= cardCount; r++)
            {
                int c = Mathf.CeilToInt((float)cardCount / r);
                if (r * c - cardCount > c) continue; // skip if more than 1 empty row

                // Calculate max card size that fits in normalized 1×1 area with this aspect
                float availW = areaAspect; // normalized width
                float availH = 1f;          // normalized height

                float maxW = availW / c;
                float maxH = availH / r;

                float w, h;
                if (maxW / cardAspect <= maxH)
                { w = maxW; h = w / cardAspect; }
                else
                { h = maxH; w = h * cardAspect; }

                float cardArea = w * h;
                float waste = 1f - (float)cardCount / (r * c);

                // Penalize waste but prioritize card size
                float score = cardArea * (1f - waste * 0.3f);

                if (score > bestCardArea)
                {
                    bestCardArea = score;
                    bestRows = r;
                    bestCols = c;
                }
            }
        }
    }
}
