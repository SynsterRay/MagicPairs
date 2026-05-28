using UnityEngine;

namespace MagicPairs.Cards
{
    [System.Serializable]
    public struct CardData
    {
        public int colorIndex;
        public Color faceColor;
        public bool isPiotrus;
        public Sprite faceSprite;

        public bool HasSprite => faceSprite != null;

        public static CardData CreatePair(int colorIndex, Color color)
        {
            return new CardData { colorIndex = colorIndex, faceColor = color, isPiotrus = false };
        }

        public static CardData CreatePiotrus(Color color)
        {
            return new CardData { colorIndex = -1, faceColor = color, isPiotrus = true };
        }

        public static CardData CreatePairWithSprite(int colorIndex, Sprite sprite)
        {
            return new CardData { colorIndex = colorIndex, faceColor = Color.white, isPiotrus = false, faceSprite = sprite };
        }

        public static CardData CreatePiotrusWithSprite(Sprite sprite)
        {
            return new CardData { colorIndex = -1, faceColor = Color.white, isPiotrus = true, faceSprite = sprite };
        }
    }
}
