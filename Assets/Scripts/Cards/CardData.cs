using UnityEngine;

namespace MagicPairs.Cards
{
    [System.Serializable]
    public struct CardData
    {
        public int colorIndex;
        public Color faceColor;
        public bool isPiotrus;

        public static CardData CreatePair(int colorIndex, Color color)
        {
            return new CardData { colorIndex = colorIndex, faceColor = color, isPiotrus = false };
        }

        public static CardData CreatePiotrus(Color color)
        {
            return new CardData { colorIndex = -1, faceColor = color, isPiotrus = true };
        }
    }
}
