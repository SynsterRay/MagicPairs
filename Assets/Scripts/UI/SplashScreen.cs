using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace MagicPairs.UI
{
    public class SplashScreen : MonoBehaviour
    {
        private GameObject _panel;
        private Image _logoImage;
        private Canvas _canvas;

        private void Awake()
        {
            _canvas = GetComponent<Canvas>() ?? GetComponentInParent<Canvas>();
        }

        private void Start()
        {
            ShowSplash();
        }

        private void ShowSplash()
        {
            if (_canvas == null) return;

            _panel = new GameObject("SplashPanel");
            _panel.transform.SetParent(_canvas.transform, false);
            var bg = _panel.AddComponent<Image>();
            bg.color = Color.white;
            var rect = _panel.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            _panel.transform.SetAsLastSibling();

            // Logo
            var logoObj = new GameObject("Logo");
            logoObj.transform.SetParent(_panel.transform, false);
            _logoImage = logoObj.AddComponent<Image>();
            _logoImage.sprite = Resources.Load<Sprite>("UIButtons/developer_logo");
            _logoImage.preserveAspect = true;
            _logoImage.raycastTarget = false;
            var logoRect = logoObj.GetComponent<RectTransform>();
            logoRect.anchorMin = new Vector2(0.2f, 0.3f);
            logoRect.anchorMax = new Vector2(0.8f, 0.7f);
            logoRect.offsetMin = Vector2.zero;
            logoRect.offsetMax = Vector2.zero;

            // Hide menu during splash
            var menu = FindAnyObjectByType<MainMenu>();
            if (menu != null)
            {
                var menuPanel = menu.transform.Find("MenuPanel");
                if (menuPanel == null)
                {
                    // menuPanel is on canvas, find via MainMenu's parent
                    foreach (Transform child in _canvas.transform)
                        if (child.name == "MenuPanel") { menuPanel = child; break; }
                }
                if (menuPanel != null) menuPanel.gameObject.SetActive(false);
            }

            StartCoroutine(AnimateSplash());
        }

        private IEnumerator AnimateSplash()
        {
            var logoRect = _logoImage.GetComponent<RectTransform>();
            float duration = 1.5f;
            float holdTime = 0.5f;
            float fadeTime = 0.4f;

            // Zoom in animation
            float t = 0f;
            while (t < duration)
            {
                t += Time.deltaTime;
                float ratio = Mathf.Clamp01(t / duration);
                float scale = Mathf.Lerp(0.5f, 1f, Mathf.SmoothStep(0f, 1f, ratio));
                logoRect.localScale = Vector3.one * scale;

                // Fade in
                float alpha = Mathf.Clamp01(t / 0.3f);
                _logoImage.color = new Color(1f, 1f, 1f, alpha);
                yield return null;
            }

            // Hold
            yield return new WaitForSeconds(holdTime);

            // Fade out
            t = 0f;
            var bg = _panel.GetComponent<Image>();
            while (t < fadeTime)
            {
                t += Time.deltaTime;
                float alpha = 1f - Mathf.Clamp01(t / fadeTime);
                _logoImage.color = new Color(1f, 1f, 1f, alpha);
                bg.color = new Color(1f, 1f, 1f, alpha);
                yield return null;
            }

            // Show menu and destroy splash
            var menu = FindAnyObjectByType<MainMenu>();
            if (menu != null) menu.SendMessage("ShowStartPanel", SendMessageOptions.DontRequireReceiver);

            Destroy(_panel);
            Destroy(this);
        }
    }
}
