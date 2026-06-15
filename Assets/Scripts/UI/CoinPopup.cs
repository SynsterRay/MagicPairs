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

            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.3f, 0.45f);
            rect.anchorMax = new Vector2(0.7f, 0.55f);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            // Spinning coin disc (golden, flips via scaleX) - vertically centered
            var coinObj = new GameObject("CoinDisc");
            coinObj.transform.SetParent(go.transform, false);
            var coinRect = coinObj.AddComponent<RectTransform>();
            coinRect.anchorMin = new Vector2(0.32f, 0.5f);
            coinRect.anchorMax = new Vector2(0.32f, 0.5f);
            coinRect.pivot = new Vector2(0.5f, 0.5f);
            coinRect.sizeDelta = new Vector2(60f, 60f);
            coinRect.anchoredPosition = Vector2.zero;
            var coinImg = coinObj.AddComponent<Image>();
            coinImg.color = new Color(1f, 0.82f, 0.1f);
            coinImg.sprite = RoundedButtonHelper.GetRoundedSprite();
            coinImg.type = Image.Type.Sliced;

            // Inner coin ring
            var inner = new GameObject("Inner");
            inner.transform.SetParent(coinObj.transform, false);
            var innerRect = inner.AddComponent<RectTransform>();
            innerRect.anchorMin = new Vector2(0.2f, 0.2f);
            innerRect.anchorMax = new Vector2(0.8f, 0.8f);
            innerRect.offsetMin = Vector2.zero;
            innerRect.offsetMax = Vector2.zero;
            var innerImg = inner.AddComponent<Image>();
            innerImg.color = new Color(0.85f, 0.65f, 0f);
            innerImg.sprite = RoundedButtonHelper.GetRoundedSprite();
            innerImg.type = Image.Type.Sliced;
            innerImg.raycastTarget = false;

            // Amount text - centered vertically, right of coin
            var txtObj = new GameObject("Text");
            txtObj.transform.SetParent(go.transform, false);
            var txt = txtObj.AddComponent<Text>();
            txt.text = $"🪙 +{amount}";
            txt.fontSize = 48;
            txt.fontStyle = FontStyle.Bold;
            txt.alignment = TextAnchor.MiddleCenter;
            txt.color = new Color(1f, 0.85f, 0.1f);
            txt.font = UIFactory.GetFont();
            var txtRect = txtObj.GetComponent<RectTransform>();
            txtRect.anchorMin = new Vector2(0.42f, 0f);
            txtRect.anchorMax = new Vector2(0.75f, 1f);
            txtRect.offsetMin = Vector2.zero;
            txtRect.offsetMax = Vector2.zero;

            StartCoroutine(AnimateCoin(go, coinRect, txt));
        }

        private IEnumerator AnimateCoin(GameObject go, RectTransform coinRect, Text txt)
        {
            var rect = go.GetComponent<RectTransform>();
            float t = 0f;
            Vector2 start = rect.anchoredPosition;

            while (t < 1.4f)
            {
                t += Time.deltaTime;
                float ratio = t / 1.4f;

                // Float up
                rect.anchoredPosition = start + Vector2.up * (ratio * 60f);

                // Spin coin (scaleX oscillates = 3D flip)
                float spin = Mathf.Cos(t * 10f);
                coinRect.localScale = new Vector3(Mathf.Abs(spin) * 0.8f + 0.2f, 1f, 1f);

                // Punch on enter
                float scale = ratio < 0.1f ? Mathf.Lerp(0.5f, 1.2f, ratio / 0.1f) :
                              ratio < 0.2f ? Mathf.Lerp(1.2f, 1f, (ratio - 0.1f) / 0.1f) : 1f;
                go.transform.localScale = Vector3.one * scale;

                // Fade out last 30%
                if (ratio > 0.7f)
                {
                    float alpha = Mathf.Lerp(1f, 0f, (ratio - 0.7f) / 0.3f);
                    var c = txt.color; c.a = alpha; txt.color = c;
                    var cc = coinRect.GetComponent<Image>().color; cc.a = alpha;
                    coinRect.GetComponent<Image>().color = cc;
                }

                yield return null;
            }

            Destroy(go);
        }
    }
}
