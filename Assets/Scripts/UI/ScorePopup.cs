using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using MagicPairs.Core;
using MagicPairs.GameFlow;

namespace MagicPairs.UI
{
    public class ScorePopup : MonoBehaviour
    {
        private Canvas _canvas;

        private int _lastScore;
        private int _lastStreak;

        private void Awake() => _canvas = GetComponent<Canvas>();

        private void OnEnable()
        {
            ChallengeMode.OnChallengeScoreChanged += OnScoreChanged;
            GameEvents.OnGameStarted += ResetLastScore;
        }

        private void OnDisable()
        {
            ChallengeMode.OnChallengeScoreChanged -= OnScoreChanged;
            GameEvents.OnGameStarted -= ResetLastScore;
        }

        private void ResetLastScore()
        {
            _lastScore = 0;
            _lastStreak = 0;
        }

        private void OnScoreChanged(int score, int streak, int level)
        {
            int delta = score - _lastScore;
            if (delta > 0 && _lastScore > 0)
            {
                string text = streak >= 2 ? $"+{delta}  x{Mathf.Min(streak, 5)}!" : $"+{delta}";
                Color color = streak >= 3 ? new Color(1f, 0.7f, 0f) :
                              streak >= 2 ? new Color(0.2f, 0.9f, 0.3f) : Color.white;
                SpawnPopup(text, color);
            }
            else if (delta < 0)
            {
                SpawnPopup($"{delta}", new Color(0.9f, 0.2f, 0.2f));
            }
            _lastScore = score;
            _lastStreak = streak;
        }

        private void SpawnPopup(string text, Color color)
        {
            if (_canvas == null) return;

            var go = new GameObject("ScorePopup");
            go.transform.SetParent(_canvas.transform, false);

            var txt = go.AddComponent<Text>();
            txt.text = text;
            txt.fontSize = 48;
            txt.fontStyle = FontStyle.Bold;
            txt.alignment = TextAnchor.MiddleCenter;
            txt.color = color;
            txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.3f, 0.45f);
            rect.anchorMax = new Vector2(0.7f, 0.55f);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            StartCoroutine(AnimatePopup(go, txt));
        }

        private IEnumerator AnimatePopup(GameObject go, Text txt)
        {
            var rect = go.GetComponent<RectTransform>();
            float duration = 1.2f;
            float t = 0f;
            Vector2 startPos = rect.anchoredPosition;

            while (t < duration)
            {
                t += Time.deltaTime;
                float ratio = t / duration;

                // Float upward
                rect.anchoredPosition = startPos + Vector2.up * (ratio * 80f);

                // Scale punch then shrink
                float scale = ratio < 0.2f ? Mathf.Lerp(0.5f, 1.3f, ratio / 0.2f) :
                              ratio < 0.4f ? Mathf.Lerp(1.3f, 1f, (ratio - 0.2f) / 0.2f) : 1f;
                rect.localScale = Vector3.one * scale;

                // Fade out in last 40%
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
