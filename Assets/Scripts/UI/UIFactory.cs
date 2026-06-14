using UnityEngine;
using UnityEngine.UI;

namespace MagicPairs.UI
{
    public static class UIFactory
    {
        private static Font _cachedFont;

        public static Font GetFont()
        {
            if (_cachedFont == null)
                _cachedFont = Resources.Load<Font>("Fonts/FredokaOne-Regular");
            if (_cachedFont == null)
                _cachedFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            return _cachedFont;
        }

        public static Text CreateText(string name, string text, Transform parent,
            Vector2 anchorMin, Vector2 anchorMax, TextAnchor alignment = TextAnchor.MiddleCenter, int fontSize = 28)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var txt = go.AddComponent<Text>();
            txt.text = text;
            txt.fontSize = fontSize;
            txt.fontStyle = FontStyle.Bold;
            txt.alignment = alignment;
            txt.color = Color.white;
            txt.font = GetFont();
            txt.resizeTextForBestFit = true;
            txt.resizeTextMinSize = Mathf.Max(14, fontSize / 2);
            txt.resizeTextMaxSize = fontSize;
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            return txt;
        }

        public static Button CreateButton(string name, string label, Transform parent,
            Vector2 anchorMin, Vector2 anchorMax, Color color)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var img = go.AddComponent<Image>();
            img.color = color;
            img.sprite = RoundedButtonHelper.GetRoundedSprite();
            img.type = Image.Type.Sliced;
            var btn = go.AddComponent<Button>();
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            // Shine overlay
            var shine = new GameObject("Shine");
            shine.transform.SetParent(go.transform, false);
            var shineImg = shine.AddComponent<Image>();
            shineImg.sprite = RoundedButtonHelper.GetShineSprite();
            shineImg.type = Image.Type.Sliced;
            shineImg.color = Color.white;
            shineImg.raycastTarget = false;
            var shineRect = shine.GetComponent<RectTransform>();
            shineRect.anchorMin = Vector2.zero;
            shineRect.anchorMax = Vector2.one;
            shineRect.offsetMin = Vector2.zero;
            shineRect.offsetMax = Vector2.zero;

            // Text
            var txtObj = new GameObject("Text");
            txtObj.transform.SetParent(go.transform, false);
            var txt = txtObj.AddComponent<Text>();
            txt.text = label;
            txt.fontSize = 52;
            txt.fontStyle = FontStyle.Bold;
            txt.alignment = TextAnchor.MiddleCenter;
            txt.color = Color.white;
            txt.font = GetFont();
            txt.resizeTextForBestFit = true;
            txt.resizeTextMinSize = 20;
            txt.resizeTextMaxSize = 52;
            txt.horizontalOverflow = HorizontalWrapMode.Wrap;
            var tr = txtObj.GetComponent<RectTransform>();
            tr.anchorMin = Vector2.zero;
            tr.anchorMax = Vector2.one;
            tr.offsetMin = new Vector2(8f, 4f);
            tr.offsetMax = new Vector2(-8f, -4f);

            return btn;
        }

        public static GameObject CreatePanel(string name, Transform parent,
            Vector2 anchorMin, Vector2 anchorMax, Color color)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var img = go.AddComponent<Image>();
            img.color = color;
            img.sprite = RoundedButtonHelper.GetRoundedSprite();
            img.type = Image.Type.Sliced;
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            return go;
        }
    }
}
