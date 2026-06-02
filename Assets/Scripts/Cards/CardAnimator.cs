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
        private static readonly int MainTexId = Shader.PropertyToID("_BaseMap");

        private SpriteRenderer _spriteRenderer;

        private void Awake()
        {
            _renderer = GetComponentInChildren<MeshRenderer>();
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            _propBlock = new MaterialPropertyBlock();
            var config = Core.GameManager.Instance?.Config;
            if (config != null) _flipDuration = config.flipDuration;
        }

        private void OnDisable() => StopAllCoroutines();

        public void SetColor(Color color)
        {
            if (_spriteRenderer != null)
            {
                _spriteRenderer.color = color;
                return;
            }
            if (_renderer == null) return;
            _renderer.GetPropertyBlock(_propBlock);
            _propBlock.SetColor(BaseColorId, color);
            _propBlock.SetColor(ColorId, color);
            _propBlock.SetTexture(MainTexId, Texture2D.whiteTexture);
            _renderer.SetPropertyBlock(_propBlock);
        }

        public void ShowJokerSymbol()
        {
            var existing = transform.Find("JokerSymbol");
            if (existing != null) { existing.gameObject.SetActive(true); return; }

            var go = new GameObject("JokerSymbol");
            go.transform.SetParent(transform, false);
            go.transform.localPosition = new Vector3(0f, 0f, -0.02f);
            var tm = go.AddComponent<TextMesh>();
            tm.text = "☠";
            tm.fontSize = 64;
            tm.characterSize = 0.12f;
            tm.anchor = TextAnchor.MiddleCenter;
            tm.alignment = TextAlignment.Center;
            tm.color = Color.red;
        }

        public void HideJokerSymbol()
        {
            var existing = transform.Find("JokerSymbol");
            if (existing != null) existing.gameObject.SetActive(false);
        }

        public void SetSprite(Sprite sprite)
        {
            if (_spriteRenderer == null)
            {
                _spriteRenderer = gameObject.GetComponentInChildren<SpriteRenderer>();
                if (_spriteRenderer == null)
                {
                    var sprObj = new GameObject("FaceSprite");
                    sprObj.transform.SetParent(transform, false);
                    sprObj.transform.localPosition = new Vector3(0f, 0f, -0.01f);
                    _spriteRenderer = sprObj.AddComponent<SpriteRenderer>();
                }
            }

            _spriteRenderer.sprite = sprite;
            _spriteRenderer.color = Color.white;
            _spriteRenderer.drawMode = SpriteDrawMode.Simple;
            // Scale sprite to fill card (1x1 in local space, parent handles card dimensions)
            if (sprite != null)
            {
                float ppu = sprite.pixelsPerUnit;
                float sprW = sprite.rect.width / ppu;
                float sprH = sprite.rect.height / ppu;
                _spriteRenderer.transform.localScale = new Vector3(1f / sprW, 1f / sprH, 1f);
            }
            else
            {
                _spriteRenderer.transform.localScale = Vector3.one;
            }

            if (_renderer != null) _renderer.enabled = false;
        }

        public void HideSprite()
        {
            if (_spriteRenderer != null)
                _spriteRenderer.sprite = null;
            if (_renderer != null) _renderer.enabled = true;
        }

        public void PlayFlip(Color targetColor, Action onComplete, Sprite sprite = null)
        {
            StartCoroutine(FlipCoroutine(targetColor, sprite, onComplete));
        }

        private IEnumerator FlipCoroutine(Color targetColor, Sprite sprite, Action onComplete)
        {
            float half = _flipDuration * 0.5f;

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

            // Swap visual at midpoint
            if (sprite != null)
                SetSprite(sprite);
            else
                SetColor(targetColor);

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

        public void PlayFlipBack(Color backColor, Action onComplete)
        {
            if (!isActiveAndEnabled) return;
            StartCoroutine(FlipBackCoroutine(backColor, onComplete));
        }

        private IEnumerator FlipBackCoroutine(Color backColor, Action onComplete)
        {
            float half = _flipDuration * 0.5f;

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

            HideSprite();
            SetColor(backColor);

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
