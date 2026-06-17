using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace MagicPairs.UI
{
    public class LoadingScreen : MonoBehaviour
    {
        private GameObject _panel;
        private Image[] _dots;
        private Sprite _activeDot;
        private Sprite _inactiveDot;
        private Canvas _canvas;
        private Coroutine _anim;

        private void Awake()
        {
            _canvas = GetComponent<Canvas>() ?? GetComponentInParent<Canvas>();
            _activeDot = LoadSprite("loading purple");
            _inactiveDot = LoadSprite("loading gray");
            CreatePanel();
        }

        private Sprite LoadSprite(string name)
        {
            var tex = Resources.Load<Texture2D>($"UIButtons/{name}");
            if (tex == null) return null;
            return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100f);
        }

        private void CreatePanel()
        {
            _panel = new GameObject("LoadingPanel");
            _panel.transform.SetParent(_canvas.transform, false);
            var bg = _panel.AddComponent<Image>();
            bg.color = new Color(1f, 1f, 1f, 0.95f);
            var rect = _panel.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            // 4 dots centered
            _dots = new Image[4];
            for (int i = 0; i < 4; i++)
            {
                var dot = new GameObject($"Dot{i}");
                dot.transform.SetParent(_panel.transform, false);
                var dotImg = dot.AddComponent<Image>();
                dotImg.sprite = _inactiveDot;
                dotImg.preserveAspect = true;
                var dr = dot.GetComponent<RectTransform>();
                float x = 0.35f + i * 0.08f;
                dr.anchorMin = new Vector2(x, 0.46f);
                dr.anchorMax = new Vector2(x + 0.06f, 0.54f);
                dr.offsetMin = Vector2.zero;
                dr.offsetMax = Vector2.zero;
                _dots[i] = dotImg;
            }

            _panel.SetActive(false);
        }

        public void Show(float duration = 0.5f, System.Action onComplete = null)
        {
            _panel.SetActive(true);
            _panel.transform.SetAsLastSibling();
            if (_anim != null) StopCoroutine(_anim);
            _anim = StartCoroutine(AnimateAndHide(duration, onComplete));
        }

        private IEnumerator AnimateAndHide(float duration, System.Action onComplete)
        {
            float elapsed = 0f;
            int frame = 0;
            while (elapsed < duration)
            {
                // Animate dots: cycle which one is active
                int active = frame % 4;
                for (int i = 0; i < 4; i++)
                    _dots[i].sprite = i == active ? _activeDot : _inactiveDot;

                frame++;
                yield return new WaitForSeconds(0.15f);
                elapsed += 0.15f;
            }

            _panel.SetActive(false);
            onComplete?.Invoke();
        }
    }
}
