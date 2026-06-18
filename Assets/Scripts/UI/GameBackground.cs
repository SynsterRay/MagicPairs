using UnityEngine;
using MagicPairs.Core;

namespace MagicPairs.UI
{
    public class GameBackground : MonoBehaviour
    {
        private GameObject _bgPanel;
        private Sprite _currentSprite;

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
                CardTheme.Dinos => "Backgrounds/background_waterworld",
                _ => null
            };

            if (path == null)
            {
                Hide();
                return;
            }

            try
            {
                // Try as Sprite first (if imported as Sprite type)
                var spr = Resources.Load<Sprite>(path);
                if (spr == null)
                {
                    var tex = Resources.Load<Texture2D>(path);
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

        private void OnDestroy()
        {
            if (_currentSprite != null) Destroy(_currentSprite);
        }
    }
}
