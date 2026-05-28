using UnityEngine;
using System;
using System.Collections;

namespace MagicPairs.Cards
{
    public class CardAnimator : MonoBehaviour
    {
        private float _flipDuration = 0.3f;
        private Renderer _renderer;
        private MaterialPropertyBlock _propBlock;
        private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
        private static readonly int ColorId = Shader.PropertyToID("_Color");

        private void Awake()
        {
            _renderer = GetComponentInChildren<Renderer>();
            _propBlock = new MaterialPropertyBlock();
            var config = Core.GameManager.Instance?.Config;
            if (config != null) _flipDuration = config.flipDuration;
        }

        public void SetColor(Color color)
        {
            if (_renderer == null) return;
            _renderer.GetPropertyBlock(_propBlock);
            _propBlock.SetColor(BaseColorId, color);
            _propBlock.SetColor(ColorId, color);
            _renderer.SetPropertyBlock(_propBlock);
        }

        public void PlayFlip(Color targetColor, Action onComplete)
        {
            StartCoroutine(FlipCoroutine(targetColor, onComplete));
        }

        private IEnumerator FlipCoroutine(Color targetColor, Action onComplete)
        {
            float half = _flipDuration * 0.5f;

            // First half: scale X from 1 to 0 (card disappears edge-on)
            float t = 0f;
            Vector3 scale = transform.localScale;
            float originalScaleX = Mathf.Abs(scale.x);
            while (t < half)
            {
                t += Time.deltaTime;
                float ratio = Mathf.Clamp01(t / half);
                scale.x = Mathf.Lerp(originalScaleX, 0f, ratio);
                transform.localScale = scale;
                yield return null;
            }

            // Swap color at midpoint
            SetColor(targetColor);

            // Second half: scale X from 0 back to original
            t = 0f;
            while (t < half)
            {
                t += Time.deltaTime;
                float ratio = Mathf.Clamp01(t / half);
                scale.x = Mathf.Lerp(0f, originalScaleX, ratio);
                transform.localScale = scale;
                yield return null;
            }

            scale.x = originalScaleX;
            transform.localScale = scale;
            onComplete?.Invoke();
        }

        public void PlayDealAnimation(Vector3 fromPos, Vector3 toPos, float delay, Action onComplete = null)
        {
            StartCoroutine(DealCoroutine(fromPos, toPos, delay, onComplete));
        }

        private IEnumerator DealCoroutine(Vector3 fromPos, Vector3 toPos, float delay, Action onComplete)
        {
            transform.position = fromPos;
            yield return new WaitForSeconds(delay);

            float duration = 0.4f;
            float t = 0f;
            while (t < duration)
            {
                t += Time.deltaTime;
                float ratio = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(t / duration));
                transform.position = Vector3.Lerp(fromPos, toPos, ratio);
                yield return null;
            }
            transform.position = toPos;
            onComplete?.Invoke();
        }
    }
}
