using UnityEngine;
using UnityEngine.UI;
using MagicPairs.Core;

namespace MagicPairs.UI
{
    public class GameBackground : MonoBehaviour
    {
        private GameObject _bgPanel;
        private Image _bgImage;
        private Sprite _currentSprite;
        private Canvas _canvas;

        private void Awake()
        {
            _canvas = GetComponent<Canvas>() ?? GetComponentInParent<Canvas>();
        }

        private void OnEnable() => GameEvents.OnGameStarted += ShowBackground;
        private void OnDisable() => GameEvents.OnGameStarted -= ShowBackground;

        private void ShowBackground()
        {
            var config = GameManager.Instance?.Config;
            if (config == null) return;

            // Only show for Cars and Princess themes
            string path = config.theme switch
            {
                CardTheme.Cars => "Backgrounds/background_cars",
                CardTheme.Princess => "Backgrounds/background_princess",
                _ => null
            };

            if (path == null)
            {
                Hide();
                return;
            }

            Debug.Log($"[GameBackground] Loading: {path}, canvas={_canvas != null}");

            try
            {
                // Try as Sprite first (if imported as Sprite type)
                var spr = Resources.Load<Sprite>(path);
                Debug.Log($"[GameBackground] Sprite={spr != null}");
                if (spr == null)
                {
                    var tex = Resources.Load<Texture2D>(path);
                    Debug.Log($"[GameBackground] Texture2D={tex != null}");
                    if (tex == null) { Hide(); return; }
                    spr = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                }

                // Use a world-space SpriteRenderer behind the cards (z=1)
                if (_bgPanel == null)
                {
                    _bgPanel = new GameObject("GameBgSprite");
                    var sr = _bgPanel.AddComponent<SpriteRenderer>();
                    sr.sortingOrder = -10;
                }
                _bgPanel.SetActive(true);
                var sprRenderer = _bgPanel.GetComponent<SpriteRenderer>();
                if (_currentSprite != null) Destroy(_currentSprite);
                _currentSprite = spr;
                sprRenderer.sprite = spr;
                sprRenderer.color = new Color(1f, 1f, 1f, 0.5f);

                // Position behind cards, scale to fill camera view
                var cam = UnityEngine.Camera.main;
                if (cam != null)
                {
                    float ortho = cam.orthographicSize;
                    float aspect = cam.aspect;
                    _bgPanel.transform.position = new Vector3(0f, -ortho * 0.18f, 5f);
                    // Scale to fill card area (82% - 8% = 74% of screen height)
                    float worldH = ortho * 2f * 0.74f;
                    float worldW = ortho * 2f * aspect;
                    float sprH = spr.bounds.size.y;
                    float sprW = spr.bounds.size.x;
                    float scale = Mathf.Max(worldW / sprW, worldH / sprH);
                    _bgPanel.transform.localScale = new Vector3(scale, scale, 1f);
                }
                Debug.Log($"[GameBackground] Done! panel active={_bgPanel.activeSelf}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[GameBackground] Error: {e.Message}");
            }
        }

        private void Hide()
        {
            if (_bgPanel != null) _bgPanel.SetActive(false);
        }

        private void CreatePanel()
        {
            _bgPanel = new GameObject("GameBgPanel");
            _bgPanel.transform.SetParent(_canvas.transform, false);

            // Background image — only in card area (below top bar, above bottom)
            var bgObj = new GameObject("BgImage");
            bgObj.transform.SetParent(_bgPanel.transform, false);
            _bgImage = bgObj.AddComponent<Image>();
            _bgImage.preserveAspect = false;
            _bgImage.raycastTarget = false;
            var bgRect = bgObj.GetComponent<RectTransform>();
            bgRect.anchorMin = new Vector2(0f, 0.08f);
            bgRect.anchorMax = new Vector2(1f, 0.82f);
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;

            // Top gradient fade (white → transparent)
            var topFade = new GameObject("TopFade");
            topFade.transform.SetParent(_bgPanel.transform, false);
            var topImg = topFade.AddComponent<Image>();
            topImg.raycastTarget = false;
            topImg.color = Color.white;
            topImg.material = null;
            var topRect = topFade.GetComponent<RectTransform>();
            topRect.anchorMin = new Vector2(0f, 0.72f);
            topRect.anchorMax = new Vector2(1f, 0.82f);
            topRect.offsetMin = Vector2.zero;
            topRect.offsetMax = Vector2.zero;
            // Gradient via CanvasGroup + custom shader not available, use simple fade image
            var topGradTex = CreateGradientTexture(false);
            topImg.sprite = Sprite.Create(topGradTex, new Rect(0, 0, 4, 32), new Vector2(0.5f, 0.5f));

            // Bottom gradient fade (transparent → white)
            var botFade = new GameObject("BotFade");
            botFade.transform.SetParent(_bgPanel.transform, false);
            var botImg = botFade.AddComponent<Image>();
            botImg.raycastTarget = false;
            botImg.color = Color.white;
            var botRect = botFade.GetComponent<RectTransform>();
            botRect.anchorMin = new Vector2(0f, 0.08f);
            botRect.anchorMax = new Vector2(1f, 0.18f);
            botRect.offsetMin = Vector2.zero;
            botRect.offsetMax = Vector2.zero;
            var botGradTex = CreateGradientTexture(true);
            botImg.sprite = Sprite.Create(botGradTex, new Rect(0, 0, 4, 32), new Vector2(0.5f, 0.5f));
        }

        private static Texture2D CreateGradientTexture(bool bottomToTop)
        {
            var tex = new Texture2D(4, 32, TextureFormat.RGBA32, false);
            tex.wrapMode = TextureWrapMode.Clamp;
            for (int y = 0; y < 32; y++)
            {
                float t = (float)y / 31f;
                float alpha = bottomToTop ? t : 1f - t;
                var c = new Color(1f, 1f, 1f, alpha);
                for (int x = 0; x < 4; x++)
                    tex.SetPixel(x, y, c);
            }
            tex.Apply();
            return tex;
        }

        private void OnDestroy()
        {
            if (_currentSprite != null) Destroy(_currentSprite);
        }
    }
}
