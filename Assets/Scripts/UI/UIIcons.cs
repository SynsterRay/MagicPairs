using UnityEngine;
using System.Collections.Generic;

namespace MagicPairs.UI
{
    public static class UIIcons
    {
        private static readonly Dictionary<string, Sprite> _cache = new();

        public static Sprite Get(string name)
        {
            if (_cache.TryGetValue(name, out var cached)) return cached;

            // Load as Sprite directly (requires Texture Type = Sprite in import settings)
            var sprite = Resources.Load<Sprite>($"UIButtons/{name}");
            if (sprite != null)
            {
                _cache[name] = sprite;
                return sprite;
            }

            // Fallback: create from Texture2D if import settings not set
            var tex = Resources.Load<Texture2D>($"UIButtons/{name}");
            if (tex == null) return null;

            sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height),
                new Vector2(0.5f, 0.5f), 100f);
            sprite.name = name;
            _cache[name] = sprite;
            return sprite;
        }
    }
}
