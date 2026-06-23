using UnityEngine;

namespace MagicPairs.Core
{
    public enum CardTheme { Colors, Princess, Cars, Animals, Dinos, SpaceAnimals }

    [CreateAssetMenu(fileName = "GameConfig", menuName = "MagicPairs/Game Config")]
    public class GameConfig : ScriptableObject
    {
        [Header("Grid")]
        public int gridRows = 4;
        public int gridCols = 4;

        [Header("Theme")]
        public CardTheme theme = CardTheme.Colors;

        [Header("Colors")]
        public Color[] colorPalette = {
            new(0.9f, 0.1f, 0.1f),  // Red
            new(0.1f, 0.4f, 0.9f),  // Blue
            new(0.1f, 0.75f, 0.2f), // Green
            new(1.0f, 0.85f, 0.0f), // Yellow
            new(0.6f, 0.1f, 0.9f),  // Purple
            new(1.0f, 0.5f, 0.0f),  // Orange
            new(0.0f, 0.8f, 0.8f),  // Cyan
            new(1.0f, 0.3f, 0.6f),  // Pink
            new(0.45f, 0.25f, 0.05f), // Brown
            new(0.1f, 0.1f, 0.1f),  // Black
            new(0.55f, 0.0f, 0.0f), // Dark Red (Maroon)
            new(1.0f, 1.0f, 1.0f),  // White
            new(0.5f, 0.5f, 0.5f),  // Gray
            new(0.0f, 0.35f, 0.15f),// Dark Green
            new(0.9f, 0.0f, 0.9f),  // Magenta
        };

        [Header("Piotruś")]
        public Color piotrusColor = new(0.1f, 0.1f, 0.1f); // Black

        [Header("Card Appearance")]
        public Color cardBackColor = new(0.15f, 0.1f, 0.3f); // Dark purple
        public float cardWidth = 1.0f;
        public float cardHeight = 1.4f;
        public float cardSpacing = 0.15f;

        [Header("Timing")]
        public float flipDuration = 0.3f;
        public float mismatchDelay = 1.0f;
        public float piotrusDelay = 1.5f;

        /// <summary>Total card slots in grid.</summary>
        public int TotalSlots => gridRows * gridCols;

        /// <summary>Number of pairs (total slots - 1 for Piotruś, divided by 2).</summary>
        public int PairCount => (TotalSlots - 1) / 2;
    }
}
