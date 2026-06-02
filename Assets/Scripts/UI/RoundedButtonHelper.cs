using UnityEngine;
using UnityEngine.UI;

namespace MagicPairs.UI
{
    /// <summary>
    /// Generates a rounded rectangle sprite at runtime for button backgrounds.
    /// Also adds a white gradient overlay to simulate glass-like reflection.
    /// </summary>
    public static class RoundedButtonHelper
    {
        private static Sprite _roundedSprite;
        private static Sprite _shineSprite;

        /// <summary>Gets or creates a rounded rectangle sprite (64x64, radius 16, sliced).</summary>
        public static Sprite GetRoundedSprite()
        {
            if (_roundedSprite != null) return _roundedSprite;

            int size = 64;
            int radius = 16;
            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Bilinear;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float alpha = GetRoundedRectAlpha(x, y, size, size, radius);
                    tex.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
                }
            }
            tex.Apply();

            // Create sliced sprite with border = radius
            _roundedSprite = Sprite.Create(tex,
                new Rect(0, 0, size, size),
                new Vector2(0.5f, 0.5f), 100f, 0,
                SpriteMeshType.FullRect,
                new Vector4(radius, radius, radius, radius));

            return _roundedSprite;
        }

        /// <summary>Gets or creates a shine/reflection overlay sprite (top half white gradient).</summary>
        public static Sprite GetShineSprite()
        {
            if (_shineSprite != null) return _shineSprite;

            int width = 64;
            int height = 64;
            int radius = 16;
            var tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Bilinear;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float roundedAlpha = GetRoundedRectAlpha(x, y, width, height, radius);
                    // Gradient: strong white at top, fading to transparent at ~55% height
                    float normalizedY = (float)y / height;
                    float gradientAlpha = 0f;
                    if (normalizedY > 0.5f)
                        gradientAlpha = Mathf.Lerp(0f, 0.35f, (normalizedY - 0.5f) / 0.5f);

                    tex.SetPixel(x, y, new Color(1f, 1f, 1f, roundedAlpha * gradientAlpha));
                }
            }
            tex.Apply();

            _shineSprite = Sprite.Create(tex,
                new Rect(0, 0, width, height),
                new Vector2(0.5f, 0.5f), 100f, 0,
                SpriteMeshType.FullRect,
                new Vector4(radius, radius, radius, radius));

            return _shineSprite;
        }

        /// <summary>Apply rounded style + shine to an existing button GameObject.</summary>
        public static void ApplyRoundedStyle(GameObject button)
        {
            var img = button.GetComponent<Image>();
            if (img != null)
            {
                img.sprite = GetRoundedSprite();
                img.type = Image.Type.Sliced;
                img.pixelsPerUnitMultiplier = 1f;
            }

            // Add shine overlay if not already present
            if (button.transform.Find("Shine") != null) return;

            var shine = new GameObject("Shine");
            shine.transform.SetParent(button.transform, false);
            // Place shine behind text but on top of button bg
            shine.transform.SetSiblingIndex(0);
            var shineImg = shine.AddComponent<Image>();
            shineImg.sprite = GetShineSprite();
            shineImg.type = Image.Type.Sliced;
            shineImg.color = Color.white;
            shineImg.raycastTarget = false;
            var shineRect = shine.GetComponent<RectTransform>();
            shineRect.anchorMin = Vector2.zero;
            shineRect.anchorMax = Vector2.one;
            shineRect.offsetMin = Vector2.zero;
            shineRect.offsetMax = Vector2.zero;
        }

        private static float GetRoundedRectAlpha(int x, int y, int w, int h, int radius)
        {
            // Check if pixel is inside rounded rectangle with anti-aliasing
            float dx = 0f, dy = 0f;

            if (x < radius) dx = radius - x;
            else if (x > w - 1 - radius) dx = x - (w - 1 - radius);

            if (y < radius) dy = radius - y;
            else if (y > h - 1 - radius) dy = y - (h - 1 - radius);

            if (dx > 0f && dy > 0f)
            {
                float dist = Mathf.Sqrt(dx * dx + dy * dy);
                if (dist > radius) return 0f;
                if (dist > radius - 1.5f) return Mathf.Clamp01((radius - dist) / 1.5f);
            }

            return 1f;
        }
    }
}
