using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using MagicPairs.Core;

namespace MagicPairs.UI
{
    public class CoinPopup : MonoBehaviour
    {
        private Canvas _canvas;
        private int _lastCoins;

        private void Awake() => _canvas = GetComponent<Canvas>() ?? GetComponentInParent<Canvas>();

        private void OnEnable()
        {
            _lastCoins = PlayerWallet.Coins;
            PlayerWallet.OnCoinsChanged += OnCoinsChanged;
        }

        private void OnDisable() => PlayerWallet.OnCoinsChanged -= OnCoinsChanged;

        private void OnCoinsChanged(int newCoins)
        {
            int delta = newCoins - _lastCoins;
            if (delta > 0)
                SpawnCoinAnimation(delta);
            _lastCoins = newCoins;
        }

        private void SpawnCoinAnimation(int amount)
        {
            if (_canvas == null) return;

            var go = new GameObject("CoinPopup");
            go.transform.SetParent(_canvas.transform, false);

            // Coin icon (emoji text)
            var txt = go.AddComponent<Text>();
            txt.text = $"🪙 +{amount}";
            txt.fontSize = 42;
            txt.fontStyle = FontStyle.Bold;
            txt.alignment = TextAnchor.MiddleCenter;
            txt.color = new Color(1f, 0.85f, 0.1f);
            txt.font = UIFactory.GetFont();

            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.3f, 0.6f);
            rect.anchorMax = new Vector2(0.7f, 0.7f);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            StartCoroutine(AnimateCoin(go, txt));
        }

        private IEnumerator AnimateCoin(GameObject go, Text txt)
        {
            var rect = go.GetComponent<RectTransform>();
            float t = 0f;
            Vector2 start = rect.anchoredPosition;

            // Punch scale + float up + rotate feel
            while (t < 1.2f)
            {
                t += Time.deltaTime;
                float ratio = t / 1.2f;

                // Float up
                rect.anchoredPosition = start + Vector2.up * (ratio * 80f);

                // Scale punch
                float scale = ratio < 0.15f ? Mathf.Lerp(0.3f, 1.4f, ratio / 0.15f) :
                              ratio < 0.3f ? Mathf.Lerp(1.4f, 1f, (ratio - 0.15f) / 0.15f) : 1f;
                rect.localScale = Vector3.one * scale;

                // Fade out last 40%
                if (ratio > 0.6f)
                {
                    var c = txt.color;
                    c.a = Mathf.Lerp(1f, 0f, (ratio - 0.6f) / 0.4f);
                    txt.color = c;
                }

                yield return null;
            }

            Destroy(go);
        }
    }
}
